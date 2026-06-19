using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Mapping;
using Application.Common.Models;
using Application.DTOs.Attendances;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Attendances;
using Application.Interfaces.Repositories.Users;
using Application.Interfaces.Services.Attendances;
using Application.Interfaces.Services.Auth;
using AutoMapper;
using Domain.Entities.Attendances;
using Domain.Entities.Users;
using Domain.Enums.Attendances;
using Domain.Enums.JobLevels;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Services.Attendances;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceLocationRepository _attendanceLocationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataScopeService _dataScopeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public AttendanceService(
        IAttendanceLocationRepository attendanceLocationRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IDataScopeService dataScopeService,
        ICurrentUserService currentUserService,
        IAuthorizationService authorizationService,
        IMapper mapper,
        AppDbContext context)
    {
        _attendanceLocationRepository = attendanceLocationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _dataScopeService = dataScopeService;
        _currentUserService = currentUserService;
        _authorizationService = authorizationService;
        _mapper = mapper;
        _context = context;
    }

    public async Task<AttendanceLocationDto> CreateLocationAsync(CreateAttendanceLocationRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        await _authorizationService.EnsurePermissionAsync(currentUserId, "attendances.location.manage", ct);

        // Validate assignments based on the current user's data scope
        await ValidateAssignmentsAsync(currentUserId, request.AssignedUserIds, request.AssignedDepartmentIds, ct);

        var location = AttendanceLocation.Create(request.Name, request.Latitude, request.Longitude, request.RadiusInMeters);
        location.CreatedBy = currentUserId;

        foreach (var userId in request.AssignedUserIds)
        {
            location.AssignedUsers.Add(AttendanceLocationUser.Create(location.Id, userId));
        }

        foreach (var deptId in request.AssignedDepartmentIds)
        {
            location.AssignedDepartments.Add(AttendanceLocationDepartment.Create(location.Id, deptId));
        }

        await _attendanceLocationRepository.AddAsync(location, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<AttendanceLocationDto>(location);
    }

    public async Task<AttendanceLocationDto> UpdateLocationAsync(Guid id, UpdateAttendanceLocationRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        await _authorizationService.EnsurePermissionAsync(currentUserId, "attendances.location.manage", ct);

        var location = await _attendanceLocationRepository.GetByIdWithDetailsAsync(id, ct);
        if (location == null)
            throw new NotFoundException("Không tìm thấy vị trí chấm công cần cập nhật.");

        // Validate data scope for existing assignments of this location
        await ValidateExistingAssignmentsScopeAsync(currentUserId, location, ct);

        // Validate data scope for new assignments
        await ValidateAssignmentsAsync(currentUserId, request.AssignedUserIds, request.AssignedDepartmentIds, ct);

        location.Update(request.Name, request.Latitude, request.Longitude, request.RadiusInMeters, request.IsActive);
        location.UpdatedBy = currentUserId;

        // Clear existing assignments
        _context.AttendanceLocationUsers.RemoveRange(location.AssignedUsers);
        _context.AttendanceLocationDepartments.RemoveRange(location.AssignedDepartments);
        location.AssignedUsers.Clear();
        location.AssignedDepartments.Clear();

        // Add new assignments
        foreach (var userId in request.AssignedUserIds)
        {
            location.AssignedUsers.Add(AttendanceLocationUser.Create(location.Id, userId));
        }

        foreach (var deptId in request.AssignedDepartmentIds)
        {
            location.AssignedDepartments.Add(AttendanceLocationDepartment.Create(location.Id, deptId));
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<AttendanceLocationDto>(location);
    }

    public async Task<AttendanceLocationDto> GetLocationByIdAsync(Guid id, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        await _authorizationService.EnsurePermissionAsync(currentUserId, "attendances.location.manage", ct);

        var location = await _attendanceLocationRepository.GetByIdWithDetailsAsync(id, ct);
        if (location == null)
            throw new NotFoundException("Không tìm thấy vị trí chấm công.");

        return _mapper.Map<AttendanceLocationDto>(location);
    }

    public async Task<PaginatedResult<AttendanceLocationDto>> GetLocationsPagedAsync(AttendanceQuery query, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        await _authorizationService.EnsurePermissionAsync(currentUserId, "attendances.location.manage", ct);

        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);
        var queryable = _attendanceLocationRepository.GetQueryable()
            .Include(al => al.AssignedUsers)
            .Include(al => al.AssignedDepartments)
            .AsNoTracking();

        if (scopeContext.Scope != ScopeType.All)
        {
            if (scopeContext.Scope == ScopeType.Department)
            {
                var deptIds = scopeContext.AccessibleDepartmentIds;
                queryable = queryable.Where(al => al.CreatedBy == currentUserId || 
                                                  al.AssignedDepartments.Any(ad => deptIds.Contains(ad.DepartmentId)) ||
                                                  al.AssignedUsers.Any(au => _context.Users.Any(u => u.Id == au.UserId && deptIds.Contains(u.DepartmentId))));
            }
            else if (scopeContext.Scope == ScopeType.Team)
            {
                queryable = queryable.Where(al => al.CreatedBy == currentUserId ||
                                                  al.AssignedUsers.Any(au => _context.Users.Any(u => u.Id == au.UserId && u.ManagerId == currentUserId)));
            }
            else // ScopeType.Own
            {
                queryable = queryable.Where(al => al.CreatedBy == currentUserId);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            queryable = queryable.Where(al => al.Name.ToLower().Contains(search));
        }

        queryable = queryable.OrderBy(al => al.Name);

        var paginatedResult = await queryable.ToPaginatedResultAsync(query, ct);
        return PaginationMapper.Map<AttendanceLocation, AttendanceLocationDto>(paginatedResult, _mapper);
    }

    public async Task<CheckInResultDto> CheckInAsync(CheckInQuery location, CheckInRequest request, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        var user = await _userRepository.GetByIdAsync(currentUserId, ct);
        if (user == null)
            throw new NotFoundException("Không tìm thấy nhân viên.");

        var locations = await _attendanceLocationRepository.GetActiveLocationsForUserAsync(currentUserId, user.DepartmentId, ct);
        if (locations.Count == 0)
        {
            var failLog = AttendanceLog.Create(
                currentUserId,
                null,
                DateTime.UtcNow,
                location.Latitude,
                location.Longitude,
                null,
                false,
                request.Type,
                "Nhân viên chưa được cấu hình vị trí chấm công.");
            _context.AttendanceLogs.Add(failLog);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CheckInResultDto
            {
                IsSuccess = false,
                Message = "Bạn chưa được cấu hình vị trí chấm công.",
                CheckTime = failLog.CheckTime
            };
        }

        AttendanceLocation? targetLocation = null;
        double? minDistance = null;
        bool isSuccess = false;

        foreach (var loc in locations)
        {
            var distance = CalculateDistanceInMeters(location.Latitude, location.Longitude, loc.Latitude, loc.Longitude);
            if (distance <= loc.RadiusInMeters)
            {
                targetLocation = loc;
                minDistance = distance;
                isSuccess = true;
                break;
            }

            if (minDistance == null || distance < minDistance.Value)
            {
                minDistance = distance;
                targetLocation = loc;
            }
        }

        var checkTime = DateTime.UtcNow;

        if (isSuccess)
        {
            var localNow = DateTime.UtcNow.AddHours(7);
            var todayDate = DateOnly.FromDateTime(localNow);

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.UserId == currentUserId && a.Date == todayDate, ct);

            if (attendance == null)
            {
                attendance = Attendance.Create(currentUserId, todayDate);
                _context.Attendances.Add(attendance);
            }

            if (request.Type == AttendanceType.CheckIn)
            {
                attendance.UpdateCheckIn(checkTime, targetLocation?.Id, location.Latitude, location.Longitude, minDistance);
            }
            else if (request.Type == AttendanceType.CheckOut)
            {
                attendance.UpdateCheckOut(checkTime, targetLocation?.Id, location.Latitude, location.Longitude, minDistance);
            }
        }

        var log = AttendanceLog.Create(
            currentUserId,
            targetLocation?.Id,
            checkTime,
            location.Latitude,
            location.Longitude,
            minDistance,
            isSuccess,
            request.Type,
            isSuccess ? null : "Vượt quá bán kính cho phép.");

        _context.AttendanceLogs.Add(log);
        await _unitOfWork.SaveChangesAsync(ct);

        return new CheckInResultDto
        {
            IsSuccess = isSuccess,
            Message = isSuccess 
                ? $"Chấm công thành công tại {targetLocation!.Name}." 
                : $"Chấm công thất bại. Khoảng cách gần nhất là {minDistance:F1}m đến vị trí {targetLocation!.Name} (bán kính cho phép: {targetLocation.RadiusInMeters}m).",
            DistanceInMeters = minDistance,
            LocationName = targetLocation?.Name,
            CheckTime = checkTime
        };
    }

    public async Task<PaginatedResult<AttendanceLogDto>> GetLogsPagedAsync(AttendanceQuery query, CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);
        var queryable = _context.AttendanceLogs
            .Include(l => l.User)
            .Include(l => l.AttendanceLocation)
            .AsNoTracking();

        // Check if user has logs reading permission. Standard users can read their own logs anyway.
        var hasLogsPermission = await _authorizationService.HasPermissionAsync(currentUserId, "attendances.log.read", ct);

        if (!hasLogsPermission)
        {
            // If they don't have read-all permission, they are restricted to their OWN logs only
            queryable = queryable.Where(l => l.UserId == currentUserId);
        }
        else if (scopeContext.Scope != ScopeType.All)
        {
            if (scopeContext.Scope == ScopeType.Department)
            {
                var deptIds = scopeContext.AccessibleDepartmentIds;
                queryable = queryable.Where(l => l.UserId == currentUserId || deptIds.Contains(l.User.DepartmentId));
            }
            else if (scopeContext.Scope == ScopeType.Team)
            {
                queryable = queryable.Where(l => l.UserId == currentUserId || l.User.ManagerId == currentUserId);
            }
            else // ScopeType.Own
            {
                queryable = queryable.Where(l => l.UserId == currentUserId);
            }
        }

        // Apply filters
        if (query.UserId.HasValue)
        {
            // If a manager or admin queries specific user, it still respects the scope filters above
            queryable = queryable.Where(l => l.UserId == query.UserId.Value);
        }

        if (query.IsSuccess.HasValue)
        {
            queryable = queryable.Where(l => l.IsSuccess == query.IsSuccess.Value);
        }

        if (query.StartDate.HasValue)
        {
            var startDateTime = query.StartDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            queryable = queryable.Where(l => l.CheckTime >= startDateTime);
        }

        if (query.EndDate.HasValue)
        {
            var endDateTime = query.EndDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            queryable = queryable.Where(l => l.CheckTime <= endDateTime);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            queryable = queryable.Where(l => l.User.FullName.ToLower().Contains(search) || 
                                             l.User.EmployeeCode.ToLower().Contains(search) ||
                                             (l.AttendanceLocation != null && l.AttendanceLocation.Name.ToLower().Contains(search)));
        }

        queryable = queryable.OrderByDescending(l => l.CheckTime);

        var paginatedResult = await queryable.ToPaginatedResultAsync(query, ct);
        return PaginationMapper.Map<AttendanceLog, AttendanceLogDto>(paginatedResult, _mapper);
    }

    public async Task<AttendanceDto?> GetTodayAttendanceAsync(CancellationToken ct = default)
    {
        var currentUserId = _currentUserService.UserId ?? Guid.Empty;
        if (currentUserId == Guid.Empty)
            throw new ForbiddenException("Không xác định được người dùng hiện tại.");

        var localNow = DateTime.UtcNow.AddHours(7);
        var todayDate = DateOnly.FromDateTime(localNow);

        var attendance = await _context.Attendances
            .Include(a => a.User)
            .Include(a => a.CheckInLocation)
            .Include(a => a.CheckOutLocation)
            .FirstOrDefaultAsync(a => a.UserId == currentUserId && a.Date == todayDate, ct);

        if (attendance == null)
            return null;

        return _mapper.Map<AttendanceDto>(attendance);
    }

    private async Task ValidateAssignmentsAsync(Guid currentUserId, List<Guid> assignedUserIds, List<Guid> assignedDepartmentIds, CancellationToken ct)
    {
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);

        if (assignedUserIds.Any())
        {
            if (scopeContext.Scope != ScopeType.All)
            {
                var allowedUserQuery = await _dataScopeService.ApplyUserScopeAsync(_userRepository.GetQueryable(), currentUserId, ct);
                var allowedUserIds = await allowedUserQuery.Select(u => u.Id).ToListAsync(ct);
                
                var unauthorizedUserIds = assignedUserIds.Where(uid => !allowedUserIds.Contains(uid)).ToList();
                if (unauthorizedUserIds.Any())
                {
                    throw new ForbiddenException("Bạn không có quyền cấu hình vị trí chấm công cho một số nhân sự ngoài phạm vi quản lý.");
                }
            }
        }

        if (assignedDepartmentIds.Any())
        {
            if (scopeContext.Scope == ScopeType.All)
            {
                // Allowed
            }
            else if (scopeContext.Scope == ScopeType.Department)
            {
                var unauthorizedDeptIds = assignedDepartmentIds.Where(did => !scopeContext.AccessibleDepartmentIds.Contains(did)).ToList();
                if (unauthorizedDeptIds.Any())
                {
                    throw new ForbiddenException("Bạn không có quyền cấu hình vị trí chấm công cho một số phòng ban ngoài phạm vi quản lý.");
                }
            }
            else
            {
                throw new ForbiddenException("Phạm vi quản trị của bạn không được phép cấu hình vị trí chấm công theo phòng ban.");
            }
        }
    }

    private async Task ValidateExistingAssignmentsScopeAsync(Guid currentUserId, AttendanceLocation location, CancellationToken ct)
    {
        var scopeContext = await _dataScopeService.GetUserScopeContextAsync(currentUserId, ct);
        if (scopeContext.Scope == ScopeType.All)
            return;

        var existingUserIds = location.AssignedUsers.Select(u => u.UserId).ToList();
        if (existingUserIds.Any())
        {
            var allowedUserQuery = await _dataScopeService.ApplyUserScopeAsync(_userRepository.GetQueryable(), currentUserId, ct);
            var allowedUserIds = await allowedUserQuery.Select(u => u.Id).ToListAsync(ct);
            if (existingUserIds.Any(uid => !allowedUserIds.Contains(uid)))
            {
                throw new ForbiddenException("Bạn không có quyền cập nhật vị trí này vì nó đang được gán cho nhân sự ngoài phạm vi quản lý.");
            }
        }

        var existingDeptIds = location.AssignedDepartments.Select(d => d.DepartmentId).ToList();
        if (existingDeptIds.Any())
        {
            if (scopeContext.Scope == ScopeType.Department)
            {
                if (existingDeptIds.Any(did => !scopeContext.AccessibleDepartmentIds.Contains(did)))
                {
                    throw new ForbiddenException("Bạn không có quyền cập nhật vị trí này vì nó đang được gán cho phòng ban ngoài phạm vi quản lý.");
                }
            }
            else
            {
                throw new ForbiddenException("Bạn không có quyền cập nhật vị trí này vì nó đang được gán cho phòng ban.");
            }
        }
    }

    private static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusInMeters = 6371000;
        
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                
        var c = 2 * Math.Asin(Math.Sqrt(a));
        
        return EarthRadiusInMeters * c;
    }

    private static double ToRadians(double angleInDegrees)
    {
        return (Math.PI / 180) * angleInDegrees;
    }
}
