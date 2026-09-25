namespace Domain;

public class ApplicantEducation : EntityBase<Guid>
{
    public Guid ApplicantId { get; set; }
    public Applicant? Applicant { get; set; }

    public EducationLevel EducationLevel { get; set; }
    public string? SchoolName { get; set; }
    public string? Major { get; set; }
    public int? GraduationYear { get; set; }
}
