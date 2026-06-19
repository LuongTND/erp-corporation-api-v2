using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces.Repositories.Attendances;
using Domain.Entities.Attendances;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Attendances;

public class AttendanceLocationRepository : GenericRepository<AttendanceLocation>, IAttendanceLocationRepository
{
    public AttendanceLocationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<AttendanceLocation?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .Include(al => al.AssignedUsers)
            .Include(al => al.AssignedDepartments)
            .FirstOrDefaultAsync(al => al.Id == id, ct);
    }

    public async Task<List<AttendanceLocation>> GetActiveLocationsForUserAsync(Guid userId, Guid departmentId, CancellationToken ct = default)
    {
        return await DbSet
            .Where(al => al.IsActive)
            .Where(al => al.AssignedUsers.Any(au => au.UserId == userId) || 
                         al.AssignedDepartments.Any(ad => ad.DepartmentId == departmentId))
            .ToListAsync(ct);
    }
}
