using TaskStatus = global::Domain.TaskStatus;

namespace Infrastructure;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDataScopeService _dataScopeService;
    private readonly IMapper _mapper;

    public TaskService(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDataScopeService dataScopeService,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dataScopeService = dataScopeService;
        _mapper = mapper;
    }

    public async Task<TaskDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);

        var task = await _taskRepository.GetByIdWithDetailsAsync(id, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc hoặc công việc đã bị xóa.");

        if (scopeContext.Scope != ScopeType.All)
        {
            var hasAccess = false;
            var isRelated = task.CreatedBy == currentUserId ||
                            task.Assignees.Any(a => a.UserID == currentUserId) ||
                            task.Followers.Any(f => f.UserID == currentUserId);

            if (isRelated)
            {
                hasAccess = true;
            }
            else if (scopeContext.Scope == ScopeType.Team)
            {
                var isSubordinateRelated =
                    task.Assignees.Any(a => a.User != null && a.User.ManagerId == currentUserId) ||
                    task.Followers.Any(f => f.User != null && f.User.ManagerId == currentUserId);
                if (isSubordinateRelated)
                    hasAccess = true;
            }
            else if (scopeContext.Scope == ScopeType.Department)
            {
                var isDeptRelated = task.Assignees.Any(a =>
                                        a.User != null &&
                                        scopeContext.AccessibleDepartmentIds.Contains(a.User.DepartmentId)) ||
                                    task.Followers.Any(f =>
                                        f.User != null &&
                                        scopeContext.AccessibleDepartmentIds.Contains(f.User.DepartmentId));
                if (isDeptRelated)
                    hasAccess = true;
            }

            if (!hasAccess)
                throw new ForbiddenException("Bạn không có quyền truy cập công việc này.");
        }

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<PaginatedResult<TaskDto>> GetPagedAsync(TaskQuery query, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);

        var queryable = _taskRepository.GetQueryable()
            .Include(t => t.Assignees)
            .ThenInclude(a => a.User)
            .Include(t => t.Followers)
            .ThenInclude(f => f.User)
            .Include(t => t.TaskKpis)
            .Include(t => t.TaskLmsCourses)
            .Where(t => t.IsActive)
            .AsNoTracking();

        if (scopeContext.Scope != ScopeType.All)
        {
            queryable = scopeContext.Scope switch
            {
                ScopeType.Own => queryable.Where(t => t.CreatedBy == currentUserId ||
                                                      t.Assignees.Any(a => a.UserID == currentUserId) ||
                                                      t.Followers.Any(f => f.UserID == currentUserId)),
                ScopeType.Team => queryable.Where(t => t.CreatedBy == currentUserId ||
                                                       t.Assignees.Any(a =>
                                                           a.UserID == currentUserId || (a.User != null &&
                                                               a.User.ManagerId == currentUserId)) ||
                                                       t.Followers.Any(f =>
                                                           f.UserID == currentUserId || (f.User != null &&
                                                               f.User.ManagerId == currentUserId))),
                ScopeType.Department => queryable.Where(t => t.CreatedBy == currentUserId ||
                                                             t.Assignees.Any(a =>
                                                                 a.UserID == currentUserId ||
                                                                 (a.User != null &&
                                                                  scopeContext.AccessibleDepartmentIds.Contains(
                                                                      a.User.DepartmentId))) ||
                                                             t.Followers.Any(f =>
                                                                 f.UserID == currentUserId ||
                                                                 (f.User != null &&
                                                                  scopeContext.AccessibleDepartmentIds.Contains(
                                                                      f.User.DepartmentId)))),
                _ => queryable.Where(t => t.CreatedBy == currentUserId ||
                                          t.Assignees.Any(a => a.UserID == currentUserId) ||
                                          t.Followers.Any(f => f.UserID == currentUserId))
            };
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            queryable = queryable.Where(t => t.Title.ToLower().Contains(search) ||
                                             t.TaskCode.ToLower().Contains(search) ||
                                             (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            if (Enum.TryParse<TaskStatus>(query.Status, true, out var status))
            {
                queryable = queryable.Where(t => t.Status == status);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Priority))
        {
            if (Enum.TryParse<TaskPriority>(query.Priority, true, out var priority))
            {
                queryable = queryable.Where(t => t.Priority == priority);
            }
        }

        if (query.AssigneeId.HasValue)
        {
            queryable = queryable.Where(t => t.Assignees.Any(a => a.UserID == query.AssigneeId.Value));
        }

        queryable = queryable.OrderByDescending(t => t.DueDate).ThenByDescending(t => t.CreatedAt);

        var paginatedResult = await queryable.ToPaginatedResultAsync(query, ct);
        return PaginationMapper.Map<TaskItem, TaskDto>(paginatedResult, _mapper);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;

        var now = DateTime.UtcNow;
        var prefix = $"TASK-{now:yyyyMM}-";
        var count = await _taskRepository.GetQueryable().CountAsync(t => t.TaskCode.StartsWith(prefix), ct);
        var taskCode = $"{prefix}{(count + 1):D3}";

        var task = TaskItem.Create(
            taskCode,
            request.Title,
            request.Description,
            request.TaskType,
            request.Priority,
            request.StartDate,
            request.DueDate,
            request.EstimatedHours,
            request.IsRecurring,
            request.RecurringPattern,
            request.ParentTaskID
        );

        task.CreatedBy = currentUserId;

        foreach (var userId in request.AssigneeIds)
        {
            var isPrimary = request.PrimaryAssigneeId == userId;
            task.Assignees.Add(TaskAssignee.Create(task.Id, userId, currentUserId, isPrimary));
        }

        if (request.PrimaryAssigneeId.HasValue && !request.AssigneeIds.Contains(request.PrimaryAssigneeId.Value))
        {
            task.Assignees.Add(TaskAssignee.Create(task.Id, request.PrimaryAssigneeId.Value, currentUserId, true));
        }

        foreach (var userId in request.FollowerIds)
        {
            task.Followers.Add(TaskFollower.Create(task.Id, userId));
        }

        foreach (var kpiId in request.KpiIds)
        {
            task.TaskKpis.Add(TaskKpi.Create(task.Id, kpiId));
        }

        foreach (var courseId in request.LmsCourseIds)
        {
            task.TaskLmsCourses.Add(TaskLmsCourse.Create(task.Id, courseId));
        }

        task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Created, null,
            request.Title));

        await _taskRepository.AddAsync(task, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(task.Id, ct);
    }

    public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var task = await _taskRepository.GetByIdWithDetailsAsync(id, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc cần cập nhật.");

        var originalStatus = task.Status;
        var originalProgress = task.Progress;
        var originalDueDate = task.DueDate;

        task.Update(
            request.Title,
            request.Description,
            request.TaskType,
            request.Priority,
            request.StartDate,
            request.DueDate,
            request.EstimatedHours,
            request.IsRecurring,
            request.RecurringPattern,
            request.ParentTaskID
        );

        task.UpdateStatus(request.Status);
        task.UpdateProgress(request.Progress);
        task.SetActualHours(request.ActualHours);

        if (originalStatus != task.Status)
        {
            task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.StatusChanged,
                originalStatus.ToString(), task.Status.ToString()));
            if (task.Status == TaskStatus.Done)
            {
                task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Completed, null,
                    null));
            }
            else if (task.Status == TaskStatus.Cancelled)
            {
                task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Cancelled, null,
                    null));
            }
        }

        if (originalProgress != task.Progress)
        {
            task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.ProgressUpdated,
                originalProgress.ToString(), task.Progress.ToString()));
        }

        if (originalDueDate != task.DueDate)
        {
            task.ActivityLogs.Add(TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.DueDateChanged,
                originalDueDate?.ToString("yyyy-MM-dd HH:mm:ss"), task.DueDate?.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(task.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc cần xóa.");

        task.IsActive = false;
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TaskCommentDto> AddCommentAsync(Guid taskId, CreateCommentRequest request,
        CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var task = await _taskRepository.GetByIdAsync(taskId, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc để thêm bình luận.");

        var comment = TaskComment.Create(taskId, currentUserId, request.Content, request.ParentCommentID);
        task.Comments.Add(comment);

        task.ActivityLogs.Add(TaskActivityLog.Create(taskId, currentUserId, TaskActivityAction.CommentAdded, null,
            request.Content.Length > 50 ? request.Content.Substring(0, 50) + "..." : request.Content));

        await _unitOfWork.SaveChangesAsync(ct);

        var commentDb = await _taskRepository.GetQueryable()
            .SelectMany(t => t.Comments)
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == comment.Id, ct);

        return _mapper.Map<TaskCommentDto>(commentDb);
    }

    public async Task<IReadOnlyList<TaskCommentDto>> GetCommentsAsync(Guid taskId, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc.");

        var comments = await _taskRepository.GetQueryable()
            .Where(t => t.Id == taskId)
            .SelectMany(t => t.Comments)
            .Include(c => c.User)
            .Include(c => c.Replies)
            .ThenInclude(r => r.User)
            .Where(c => c.ParentCommentID == null)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        return _mapper.Map<List<TaskCommentDto>>(comments);
    }
}