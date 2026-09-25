namespace Contract;

public sealed class ApplicationSummaryResponse
{
    public Guid Id { get; init; }
    public Guid ApplicantId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? PrimaryPhone { get; init; }
    public string? PrimaryEmail { get; init; }
    public string? TopEducationLevel { get; init; }
    public string? RecentCompany { get; init; }
    public string SourceChannel { get; init; } = string.Empty;
    public string Stage { get; init; } = string.Empty;
    public double AverageScore { get; init; }
    public DateTimeOffset AppliedAt { get; init; }
    public bool IsNew { get; init; }
}

public sealed class ApplicationDetailResponse
{
    public Guid Id { get; init; }
    public Guid ApplicantId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Gender { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Province { get; init; }
    public string? Ward { get; init; }
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public string SourceChannel { get; init; } = string.Empty;
    public string Stage { get; init; } = string.Empty;
    public string? RejectionReason { get; init; }
    public DateOnly? TrialStartDate { get; init; }
    public DateTimeOffset AppliedAt { get; init; }
    public IEnumerable<string> Phones { get; init; } = [];
    public IEnumerable<string> Emails { get; init; } = [];
    public IEnumerable<ApplicantEducationResponse> Educations { get; init; } = [];
    public IEnumerable<ApplicantExperienceResponse> Experiences { get; init; } = [];
    public IEnumerable<ApplicantDocumentResponse> Documents { get; init; } = [];
    public IEnumerable<StageHistoryResponse> StageHistory { get; init; } = [];
    public IEnumerable<InterviewScheduleSummaryResponse> InterviewSchedules { get; init; } = [];
    public IEnumerable<ApplicationEvaluationResponse> Evaluations { get; init; } = [];
}

public sealed class ApplicantEducationResponse
{
    public Guid Id { get; init; }
    public string EducationLevel { get; init; } = string.Empty;
    public string? SchoolName { get; init; }
    public string? Major { get; init; }
    public int? GraduationYear { get; init; }
}

public sealed class ApplicantExperienceResponse
{
    public Guid Id { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string? RecentWorkplace { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Position { get; init; }
    public string? Description { get; init; }
}

public sealed class ApplicantDocumentResponse
{
    public Guid Id { get; init; }
    public string DocumentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public DateTimeOffset UploadedAt { get; init; }
}

public sealed class StageHistoryResponse
{
    public string FromStage { get; init; } = string.Empty;
    public string ToStage { get; init; } = string.Empty;
    public string ChangedBy { get; init; } = string.Empty;
    public DateTimeOffset ChangedAt { get; init; }
    public string? Note { get; init; }
}

public sealed class ApplicationEvaluationResponse
{
    public Guid Id { get; init; }
    public string Evaluator { get; init; } = string.Empty;
    public int Score { get; init; }
    public string? StrengthNotes { get; init; }
    public string? WeaknessNotes { get; init; }
    public string Recommendation { get; init; } = string.Empty;
}
