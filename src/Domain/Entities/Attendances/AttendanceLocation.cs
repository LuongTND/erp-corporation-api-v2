using Domain.Base;

namespace Domain.Entities.Attendances;

public class AttendanceLocation : BaseEntity, IAuditable, ICreationTracked, IModificationTracked
{
    public string Name { get; private set; } = null!;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double RadiusInMeters { get; private set; }

    public bool IsActive { get; set; } = true;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<AttendanceLocationUser> AssignedUsers { get; private set; } = [];
    public virtual ICollection<AttendanceLocationDepartment> AssignedDepartments { get; private set; } = [];

    private AttendanceLocation() : base()
    {
    }

    public static AttendanceLocation Create(
        string name,
        double latitude,
        double longitude,
        double radiusInMeters)
    {
        return new AttendanceLocation
        {
            Name = name,
            Latitude = latitude,
            Longitude = longitude,
            RadiusInMeters = radiusInMeters,
            IsActive = true
        };
    }

    public void Update(
        string name,
        double latitude,
        double longitude,
        double radiusInMeters,
        bool isActive)
    {
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        RadiusInMeters = radiusInMeters;
        IsActive = isActive;
    }
}
