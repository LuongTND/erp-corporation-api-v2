using Application.Common.Exceptions;
using Application.Common.Mapping;
using Application.Common.Models;
using Application.DTOs.Tasks;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Tasks;
using Application.Interfaces.Services.Tasks;
using Application.Interfaces.Services.Auth;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Tasks;
using Domain.Enums.Tasks;
using Domain.Enums.JobLevels;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using TaskStatus = Domain.Enums.Tasks.TaskStatus;

namespace Infrastructure.Implementations.Services.Tasks;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDataScopeService _dataScopeService;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public TaskService(
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IDataScopeService dataScopeService,
        IMapper mapper,
        AppDbContext context)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dataScopeService = dataScopeService;
        _mapper = mapper;
        _context = context;
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
                var isSubordinateRelated = task.Assignees.Any(a => a.User != null && a.User.ManagerId == currentUserId) ||
                                           task.Followers.Any(f => f.User != null && f.User.ManagerId == currentUserId);
                if (isSubordinateRelated)
                    hasAccess = true;
            }
            else if (scopeContext.Scope == ScopeType.Department)
            {
                var isDeptRelated = task.Assignees.Any(a => a.User != null && scopeContext.AccessibleDepartmentIds.Contains(a.User.DepartmentId)) ||
                                    task.Followers.Any(f => f.User != null && scopeContext.AccessibleDepartmentIds.Contains(f.User.DepartmentId));
                if (isDeptRelated)
                    hasAccess = true;
            }

            if (!hasAccess)
                throw new ForbiddenException("Bạn không có quyền truy cập công việc này.");
        }

        var dto = _mapper.Map<TaskDto>(task);
        await PopulateTaskCommentsAndLogsAsync(id, dto, ct);
        return dto;
    }

    private async Task PopulateTaskCommentsAndLogsAsync(Guid taskId, TaskDto dto, CancellationToken ct)
    {
        var comments = await _context.TaskComments
            .AsNoTracking()
            .Where(c => c.TaskID == taskId && c.ParentCommentID == null)
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .OrderBy(c => c.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(ct);

        var logs = await _context.TaskActivityLogs
            .AsNoTracking()
            .Where(l => l.TaskID == taskId)
            .Include(l => l.User)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync(ct);

        dto.Comments = _mapper.Map<List<TaskCommentDto>>(comments);
        foreach (var commentDto in dto.Comments)
        {
            if (commentDto.Replies.Count > 0)
                commentDto.Replies = commentDto.Replies.OrderBy(r => r.CreatedAt).ToList();
        }

        dto.ActivityLogs = _mapper.Map<List<TaskActivityLogDto>>(logs);
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
            .Include(t => t.Subtasks.Where(s => s.IsActive))
            .Where(t => t.IsActive && t.ParentTaskID == null)
            .AsNoTracking()
            .AsSplitQuery();

        if (scopeContext.Scope != ScopeType.All)
        {
            queryable = scopeContext.Scope switch
            {
                ScopeType.Own => queryable.Where(t => t.CreatedBy == currentUserId || 
                                                     t.Assignees.Any(a => a.UserID == currentUserId) || 
                                                     t.Followers.Any(f => f.UserID == currentUserId)),
                ScopeType.Team => queryable.Where(t => t.CreatedBy == currentUserId || 
                                                      t.Assignees.Any(a => a.UserID == currentUserId || (a.User != null && a.User.ManagerId == currentUserId)) || 
                                                      t.Followers.Any(f => f.UserID == currentUserId || (f.User != null && f.User.ManagerId == currentUserId))),
                ScopeType.Department => queryable.Where(t => t.CreatedBy == currentUserId || 
                                                            t.Assignees.Any(a => a.UserID == currentUserId || (a.User != null && scopeContext.AccessibleDepartmentIds.Contains(a.User.DepartmentId))) || 
                                                            t.Followers.Any(f => f.UserID == currentUserId || (f.User != null && scopeContext.AccessibleDepartmentIds.Contains(f.User.DepartmentId)))),
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
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        TaskItem? parentTask = null;
        if (request.ParentTaskID.HasValue)
        {
            parentTask = await _taskRepository.GetByIdWithDetailsAsync(request.ParentTaskID.Value, ct);
            if (parentTask == null || !parentTask.IsActive)
                throw new NotFoundException("Không tìm thấy công việc cha.");
            if (parentTask.ParentTaskID.HasValue)
                throw new DomainException("Không thể tạo subtask cho một subtask.");
        }

        var assigneeIds = request.AssigneeIds.ToList();
        var primaryAssigneeId = request.PrimaryAssigneeId;
        if (parentTask != null && assigneeIds.Count == 0)
        {
            assigneeIds = parentTask.Assignees.Select(a => a.UserID).ToList();
            primaryAssigneeId ??= parentTask.Assignees.FirstOrDefault(a => a.IsPrimaryAssignee)?.UserID;
        }

        var now = DateTime.UtcNow;
        var prefix = $"TASK-{now:yyyyMM}-";
        var count = await _taskRepository.GetQueryable().CountAsync(t => t.TaskCode.StartsWith(prefix), ct);
        var taskCode = $"{prefix}{(count + 1):D3}";

        var task = TaskItem.Create(
            taskCode,
            request.Title,
            request.Description,
            request.TaskType,
            parentTask?.Priority ?? request.Priority,
            request.StartDate ?? parentTask?.StartDate,
            request.DueDate ?? parentTask?.DueDate,
            request.EstimatedHours,
            request.IsRecurring,
            request.RecurringPattern,
            request.ParentTaskID
        );

        task.CreatedBy = currentUserId;

        foreach (var userId in assigneeIds)
        {
            var isPrimary = primaryAssigneeId == userId;
            task.Assignees.Add(TaskAssignee.Create(task.Id, userId, currentUserId, isPrimary));
        }

        if (primaryAssigneeId.HasValue && !assigneeIds.Contains(primaryAssigneeId.Value))
        {
            task.Assignees.Add(TaskAssignee.Create(task.Id, primaryAssigneeId.Value, currentUserId, true));
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

        await _taskRepository.AddAsync(task, ct);
        await _context.TaskActivityLogs.AddAsync(
            TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Created, null, request.Title),
            ct);

        if (request.ParentTaskID.HasValue)
        {
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(request.ParentTaskID.Value, currentUserId, TaskActivityAction.SubtaskAdded, null, request.Title),
                ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        if (request.ParentTaskID.HasValue)
        {
            await RecalculateParentProgressAsync(request.ParentTaskID.Value, currentUserId, ct);
        }

        return await GetByIdAsync(task.Id, ct);
    }

    public async Task<TaskDto> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        var task = await _taskRepository.GetByIdAsync(id, ct);
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
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.StatusChanged, originalStatus.ToString(), task.Status.ToString()),
                ct);
            if (task.Status == TaskStatus.Done)
            {
                await _context.TaskActivityLogs.AddAsync(
                    TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Completed, null, null),
                    ct);
            }
            else if (task.Status == TaskStatus.Cancelled)
            {
                await _context.TaskActivityLogs.AddAsync(
                    TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.Cancelled, null, null),
                    ct);
            }
        }
        if (originalProgress != task.Progress)
        {
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.ProgressUpdated, originalProgress.ToString(), task.Progress.ToString()),
                ct);
        }
        if (originalDueDate != task.DueDate)
        {
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(task.Id, currentUserId, TaskActivityAction.DueDateChanged, originalDueDate?.ToString("yyyy-MM-dd HH:mm:ss"), task.DueDate?.ToString("yyyy-MM-dd HH:mm:ss")),
                ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);

        if (task.ParentTaskID.HasValue)
        {
            await RecalculateParentProgressAsync(task.ParentTaskID.Value, currentUserId, ct);
        }

        return await GetByIdAsync(task.Id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var task = await _taskRepository.GetByIdAsync(id, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc cần xóa.");

        var subtasks = await _taskRepository.GetQueryable()
            .Where(t => t.ParentTaskID == id && t.IsActive)
            .ToListAsync(ct);

        foreach (var subtask in subtasks)
        {
            subtask.IsActive = false;
        }

        task.IsActive = false;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task RecalculateParentProgressAsync(Guid parentTaskId, Guid currentUserId, CancellationToken ct)
    {
        var subtasks = await _taskRepository.GetQueryable()
            .AsNoTracking()
            .Where(t => t.ParentTaskID == parentTaskId && t.IsActive)
            .Select(t => t.Status)
            .ToListAsync(ct);

        if (subtasks.Count == 0) return;

        var parent = await _taskRepository.GetByIdAsync(parentTaskId, ct);
        if (parent == null || !parent.IsActive) return;

        var originalProgress = parent.Progress;
        var originalStatus = parent.Status;
        var doneCount = subtasks.Count(s => s == TaskStatus.Done);

        parent.ApplySubtaskProgress(doneCount, subtasks.Count);

        if (originalProgress != parent.Progress)
        {
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(parent.Id, currentUserId, TaskActivityAction.ProgressUpdated, originalProgress.ToString(), parent.Progress.ToString()),
                ct);
        }

        if (originalStatus != parent.Status)
        {
            await _context.TaskActivityLogs.AddAsync(
                TaskActivityLog.Create(parent.Id, currentUserId, TaskActivityAction.StatusChanged, originalStatus.ToString(), parent.Status.ToString()),
                ct);
            if (parent.Status == TaskStatus.Done)
            {
                await _context.TaskActivityLogs.AddAsync(
                    TaskActivityLog.Create(parent.Id, currentUserId, TaskActivityAction.Completed, null, null),
                    ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TaskCommentDto> AddCommentAsync(Guid taskId, CreateCommentRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        var task = await _taskRepository.GetByIdAsync(taskId, ct);
        if (task == null || !task.IsActive)
            throw new NotFoundException("Không tìm thấy công việc để thêm bình luận.");

        var comment = TaskComment.Create(taskId, currentUserId, request.Content, request.ParentCommentID);
        await _context.TaskComments.AddAsync(comment, ct);

        var log = TaskActivityLog.Create(taskId, currentUserId, TaskActivityAction.CommentAdded, null, request.Content.Length > 50 ? request.Content.Substring(0, 50) + "..." : request.Content);
        await _context.TaskActivityLogs.AddAsync(log, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        var commentDb = await _context.TaskComments
            .AsNoTracking()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == comment.Id, ct);

        return _mapper.Map<TaskCommentDto>(commentDb);
    }

    public async Task<IReadOnlyList<TaskCommentDto>> GetCommentsAsync(Guid taskId, CancellationToken ct = default)
    {
        var exists = await _taskRepository.GetQueryable()
            .AsNoTracking()
            .AnyAsync(t => t.Id == taskId && t.IsActive, ct);
        if (!exists)
            throw new NotFoundException("Không tìm thấy công việc.");

        var comments = await _context.TaskComments
            .AsNoTracking()
            .Where(c => c.TaskID == taskId && c.ParentCommentID == null)
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .OrderBy(c => c.CreatedAt)
            .AsSplitQuery()
            .ToListAsync(ct);

        var result = _mapper.Map<List<TaskCommentDto>>(comments);
        foreach (var dto in result)
        {
            if (dto.Replies.Count > 0)
                dto.Replies = dto.Replies.OrderBy(r => r.CreatedAt).ToList();
        }

        return result;
    }
}
