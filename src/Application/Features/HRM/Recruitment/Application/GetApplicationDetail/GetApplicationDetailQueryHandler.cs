namespace Application;

public sealed class GetApplicationDetailQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApplicationDetailQuery, ApplicationDetailResponse>
{
    public async Task<ApplicationDetailResponse> Handle(GetApplicationDetailQuery q, CancellationToken ct)
    {
        var result = await (
            from a in unitOfWork.Repository<Domain.Application>().Query()
            join ap in unitOfWork.Repository<Applicant>().Query() on a.ApplicantId equals ap.Id
            where a.Id == q.ApplicationId
            select new ApplicationDetailResponse
            {
                Id = a.Id,
                ApplicantId = ap.Id,
                FullName = ap.FullName,
                Gender = ap.Gender.HasValue ? ap.Gender.Value.ToString() : null,
                DateOfBirth = ap.DateOfBirth,
                Province = ap.Province,
                Ward = ap.Ward,
                Address = ap.Address,
                Notes = ap.Notes,
                SourceChannel = a.SourceChannel.ToString(),
                Stage = a.Stage.ToString(),
                RejectionReason = a.RejectionReason,
                TrialStartDate = a.TrialStartDate,
                AppliedAt = a.CreatedAt,
                Phones = ap.Phones.OrderByDescending(p => p.IsPrimary).Select(p => p.Phone),
                Emails = ap.Emails.OrderByDescending(e => e.IsPrimary).Select(e => e.Email),
                Educations = ap.Educations.Select(e => new ApplicantEducationResponse
                {
                    Id = e.Id,
                    EducationLevel = e.EducationLevel.ToString(),
                    SchoolName = e.SchoolName,
                    Major = e.Major,
                    GraduationYear = e.GraduationYear,
                }),
                Experiences = ap.Experiences.OrderByDescending(e => e.StartDate).Select(e => new ApplicantExperienceResponse
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName,
                    RecentWorkplace = e.RecentWorkplace,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Position = e.Position,
                    Description = e.Description,
                }),
                Documents = ap.Documents.Select(d => new ApplicantDocumentResponse
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType.ToString(),
                    FileName = d.FileName,
                    FileUrl = d.FileUrl,
                    UploadedAt = d.CreatedAt,
                }),
                StageHistory = a.StageHistory.OrderBy(h => h.ChangedAt).Select(h => new StageHistoryResponse
                {
                    FromStage = h.FromStage.ToString(),
                    ToStage = h.ToStage.ToString(),
                    ChangedBy = h.ChangedBy != null ? h.ChangedBy.FullName : string.Empty,
                    ChangedAt = h.ChangedAt,
                    Note = h.Note,
                }),
                InterviewSchedules = a.InterviewSchedules.OrderBy(s => s.ScheduledAt).Select(s => new InterviewScheduleSummaryResponse
                {
                    Id = s.Id,
                    ScheduledAt = s.ScheduledAt,
                    LocationNote = s.LocationNote,
                    Notes = s.Notes,
                    Status = s.Status.ToString(),
                    InterviewerId = s.InterviewerId,
                    InterviewerName = s.Interviewer != null ? s.Interviewer.FullName : null,
                    InterviewResult = s.InterviewResult,
                    CompletedAt = s.CompletedAt,
                }),
                Evaluations = a.Evaluations.Select(e => new ApplicationEvaluationResponse
                {
                    Id = e.Id,
                    Evaluator = e.Evaluator != null ? e.Evaluator.FullName : string.Empty,
                    Score = e.Score,
                    StrengthNotes = e.StrengthNotes,
                    WeaknessNotes = e.WeaknessNotes,
                    Recommendation = e.Recommendation.ToString(),
                }),
            }
        ).FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", q.ApplicationId));

        return result;
    }
}
