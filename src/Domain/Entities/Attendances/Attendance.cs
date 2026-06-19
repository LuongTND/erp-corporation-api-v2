using System;
using Domain.Base;
using Domain.Entities.Users;

namespace Domain.Entities.Attendances;

public class Attendance : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public Guid UserId { get; private set; }
    public virtual User User { get; private set; } = null!;

    public DateOnly Date { get; private set; }

    // Check-in details
    public DateTime? CheckInTime { get; private set; }
    public Guid? CheckInLocationId { get; private set; }
    public virtual AttendanceLocation? CheckInLocation { get; private set; }
    public double? CheckInLatitude { get; private set; }
    public double? CheckInLongitude { get; private set; }
    public double? CheckInDistanceInMeters { get; private set; }

    // Check-out details
    public DateTime? CheckOutTime { get; private set; }
    public Guid? CheckOutLocationId { get; private set; }
    public virtual AttendanceLocation? CheckOutLocation { get; private set; }
    public double? CheckOutLatitude { get; private set; }
    public double? CheckOutLongitude { get; private set; }
    public double? CheckOutDistanceInMeters { get; private set; }

    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    private Attendance() : base()
    {
    }

    public static Attendance Create(Guid userId, DateOnly date)
    {
        return new Attendance
        {
            UserId = userId,
            Date = date,
            IsActive = true
        };
    }

    public void UpdateCheckIn(DateTime time, Guid? locationId, double latitude, double longitude, double? distance)
    {
        CheckInTime = time;
        CheckInLocationId = locationId;
        CheckInLatitude = latitude;
        CheckInLongitude = longitude;
        CheckInDistanceInMeters = distance;
    }

    public void UpdateCheckOut(DateTime time, Guid? locationId, double latitude, double longitude, double? distance)
    {
        CheckOutTime = time;
        CheckOutLocationId = locationId;
        CheckOutLatitude = latitude;
        CheckOutLongitude = longitude;
        CheckOutDistanceInMeters = distance;
    }
}
