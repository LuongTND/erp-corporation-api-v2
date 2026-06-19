using Domain.Entities.Attendances;

namespace Application.Interfaces.Repositories.Attendances;

public interface IAttendanceLocationRepository : IGenericRepository<AttendanceLocation>
{
    Task<AttendanceLocation?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<List<AttendanceLocation>> GetActiveLocationsForUserAsync(Guid userId, Guid departmentId, CancellationToken ct = default);
}
