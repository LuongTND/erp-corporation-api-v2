namespace Domain;

public class RecruitmentApproverConfig : EntityBase<Guid>
{
    public Guid ApproverId { get; set; }
    public User? Approver { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public string? Note { get; set; }
}
