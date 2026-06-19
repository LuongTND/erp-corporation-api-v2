using Domain.Entities.Users;

namespace Domain.Entities.Attendances;

public class AttendanceLocationUser
{
    public Guid AttendanceLocationId { get; private set; }
    public virtual AttendanceLocation AttendanceLocation { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    private AttendanceLocationUser()
    {
    }

    public static AttendanceLocationUser Create(Guid attendanceLocationId, Guid userId)
    {
        return new AttendanceLocationUser
        {
            AttendanceLocationId = attendanceLocationId,
            UserId = userId
        };
    }
}
