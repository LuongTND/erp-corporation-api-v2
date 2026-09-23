namespace Application;

public sealed record CreateApplicationCommand(
    Guid JobPostingId,
    string FullName,
    Gender? Gender,
    DateOnly? DateOfBirth,
    string? Province,
    string? Ward,
    string? Address,
    RecruitmentChannel SourceChannel,
    string? Notes,
    IEnumerable<string> Phones,
    IEnumerable<string> Emails,
    IEnumerable<ApplicantEducationInput> Educations,
    IEnumerable<ApplicantExperienceInput> Experiences
) : IRequest<Guid>;

public sealed record ApplicantEducationInput(
    EducationLevel EducationLevel,
    string? SchoolName,
    string? Major,
    int? GraduationYear
);

public sealed record ApplicantExperienceInput(
    string CompanyName,
    string? RecentWorkplace,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? Position,
    string? Description
);
