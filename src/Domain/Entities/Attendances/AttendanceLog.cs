using Domain.Base;
using Domain.Entities.Users;
using Domain.Enums.Attendances;

namespace Domain.Entities.Attendances;

public class AttendanceLog : BaseEntity, ICreationTracked
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public Guid? AttendanceLocationId { get; private set; }
    public virtual AttendanceLocation? AttendanceLocation { get; private set; }

    public DateTime CheckTime { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double? DistanceInMeters { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? FailureReason { get; private set; }
    public AttendanceType Type { get; private set; }

    public Guid? CreatedBy { get; set; }

    private AttendanceLog() : base()
    {
    }

    public static AttendanceLog Create(
        Guid userId,
        Guid? attendanceLocationId,
        DateTime checkTime,
        double latitude,
        double longitude,
        double? distanceInMeters,
        bool isSuccess,
        AttendanceType type,
        string? failureReason = null)
    {
        return new AttendanceLog
        {
            UserId = userId,
            AttendanceLocationId = attendanceLocationId,
            CheckTime = checkTime,
            Latitude = latitude,
            Longitude = longitude,
            DistanceInMeters = distanceInMeters,
            IsSuccess = isSuccess,
            Type = type,
            FailureReason = failureReason
        };
    }
}

