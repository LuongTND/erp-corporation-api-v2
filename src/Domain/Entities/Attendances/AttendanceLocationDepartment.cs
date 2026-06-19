using Domain.Entities.Departments;

namespace Domain.Entities.Attendances;

public class AttendanceLocationDepartment
{
    public Guid AttendanceLocationId { get; private set; }
    public virtual AttendanceLocation AttendanceLocation { get; private set; } = null!;

    public Guid DepartmentId { get; private set; }
    public virtual Department Department { get; private set; } = null!;

    private AttendanceLocationDepartment()
    {
    }

    public static AttendanceLocationDepartment Create(Guid attendanceLocationId, Guid departmentId)
    {
        return new AttendanceLocationDepartment
        {
            AttendanceLocationId = attendanceLocationId,
            DepartmentId = departmentId
        };
    }
}
