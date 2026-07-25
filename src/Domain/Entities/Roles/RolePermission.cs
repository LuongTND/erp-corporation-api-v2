namespace Domain;

public class RolePermission : EntityBase<Guid>
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }

    public DateTimeOffset AssignedAt { get; set; }
    public Guid? AssignedBy { get; set; }
}
