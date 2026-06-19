using Application.Common.Models;
using Application.DTOs.Attendances;

namespace Application.Interfaces.Services.Attendances;

public interface IAttendanceService
{
    Task<AttendanceLocationDto> CreateLocationAsync(CreateAttendanceLocationRequest request, CancellationToken ct = default);
    Task<AttendanceLocationDto> UpdateLocationAsync(Guid id, UpdateAttendanceLocationRequest request, CancellationToken ct = default);
    Task<AttendanceLocationDto> GetLocationByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<AttendanceLocationDto>> GetLocationsPagedAsync(AttendanceQuery query, CancellationToken ct = default);
    
    Task<CheckInResultDto> CheckInAsync(CheckInQuery location, CheckInRequest request, CancellationToken ct = default);
    Task<PaginatedResult<AttendanceLogDto>> GetLogsPagedAsync(AttendanceQuery query, CancellationToken ct = default);
    Task<AttendanceDto?> GetTodayAttendanceAsync(CancellationToken ct = default);
}
