using API.Base;
using API.Filters;
using Application.Common.Models;
using Application.DTOs.Attendances;
using Application.Interfaces.Services.Attendances;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Attendances;

[Route("api/attendances")]
public class AttendanceController : BaseApiController
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("locations")]
    [AuthorizePermission("attendances.location.manage")]
    public async Task<ActionResult<AttendanceLocationDto>> CreateLocation([FromBody] CreateAttendanceLocationRequest request, CancellationToken ct)
    {
        var result = await _attendanceService.CreateLocationAsync(request, ct);
        return CreatedAtAction(nameof(GetLocationById), new { id = result.Id }, result);
    }

    [HttpPut("locations/{id:guid}")]
    [AuthorizePermission("attendances.location.manage")]
    public async Task<ActionResult<AttendanceLocationDto>> UpdateLocation(Guid id, [FromBody] UpdateAttendanceLocationRequest request, CancellationToken ct)
    {
        var result = await _attendanceService.UpdateLocationAsync(id, request, ct);
        return Ok(result);
    }

    [HttpGet("locations/{id:guid}")]
    [AuthorizePermission("attendances.location.manage")]
    public async Task<ActionResult<AttendanceLocationDto>> GetLocationById(Guid id, CancellationToken ct)
    {
        var result = await _attendanceService.GetLocationByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpGet("locations")]
    [AuthorizePermission("attendances.location.manage")]
    public async Task<ActionResult<PaginatedResult<AttendanceLocationDto>>> GetLocations([FromQuery] AttendanceQuery query, CancellationToken ct)
    {
        var result = await _attendanceService.GetLocationsPagedAsync(query, ct);
        return Ok(result);
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<CheckInResultDto>> CheckIn([FromQuery] CheckInQuery location, [FromBody] CheckInRequest request, CancellationToken ct)
    {
        var result = await _attendanceService.CheckInAsync(location, request, ct);
        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<ActionResult<AttendanceDto>> GetTodayAttendance(CancellationToken ct)
    {
        var result = await _attendanceService.GetTodayAttendanceAsync(ct);
        return Ok(result);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<PaginatedResult<AttendanceLogDto>>> GetLogs([FromQuery] AttendanceQuery query, CancellationToken ct)
    {
        var result = await _attendanceService.GetLogsPagedAsync(query, ct);
        return Ok(result);
    }
}
