namespace Domain;

public class EmployeeProfile : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Gender? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PermanentAddress { get; set; }
    public string? CurrentAddress { get; set; }
}
