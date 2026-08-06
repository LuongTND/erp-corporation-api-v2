namespace Domain;

public class EmployeeIdentity : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public string? IdentityCardNumber { get; set; }
    public DateOnly? IdentityCardIssuedDate { get; set; }
    public string? IdentityCardIssuedPlace { get; set; }
    public string? PassportNumber { get; set; }
    public DateOnly? PassportExpiryDate { get; set; }
}
