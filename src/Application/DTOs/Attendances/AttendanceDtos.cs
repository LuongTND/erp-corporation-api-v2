using System;
using System.Collections.Generic;
using Application.Common.Models;
using Domain.Enums.Attendances;

namespace Application.DTOs.Attendances;

public class AttendanceLocationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }

    public List<Guid> AssignedUserIds { get; set; } = [];
    public List<Guid> AssignedDepartmentIds { get; set; } = [];
}

public class CreateAttendanceLocationRequest
{
    public string Name { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; }
    public List<Guid> AssignedUserIds { get; set; } = [];
    public List<Guid> AssignedDepartmentIds { get; set; } = [];
}

public class UpdateAttendanceLocationRequest
{
    public string Name { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> AssignedUserIds { get; set; } = [];
    public List<Guid> AssignedDepartmentIds { get; set; } = [];
}

public class CheckInQuery
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CheckInRequest
{
    public AttendanceType Type { get; set; }
}

public class CheckInResultDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = null!;
    public double? DistanceInMeters { get; set; }
    public string? LocationName { get; set; }
    public DateTime CheckTime { get; set; }
}

public class AttendanceLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public string UserEmployeeCode { get; set; } = null!;
    public Guid? AttendanceLocationId { get; set; }
    public string? AttendanceLocationName { get; set; }
    public DateTime CheckTime { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? DistanceInMeters { get; set; }
    public bool IsSuccess { get; set; }
    public AttendanceType Type { get; set; }
    public string? FailureReason { get; set; }
}

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string? CheckInLocationName { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? CheckOutLocationName { get; set; }
}

public class AttendanceQuery : PaginationQuery
{
    public string? Search { get; set; }
    public Guid? UserId { get; set; }
    public bool? IsSuccess { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
