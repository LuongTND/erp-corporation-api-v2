namespace Application;

public sealed class GetApplicationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetApplicationsQuery, IEnumerable<ApplicationSummaryResponse>>
{
    public async Task<IEnumerable<ApplicationSummaryResponse>> Handle(GetApplicationsQuery q, CancellationToken ct)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-3);

        var query =
            from a in unitOfWork.Repository<Domain.Application>().Query()
            join ap in unitOfWork.Repository<Applicant>().Query() on a.ApplicantId equals ap.Id
            where a.JobPostingId == q.JobPostingId
            select new { a, ap };

        if (!string.IsNullOrWhiteSpace(q.Stage) && Enum.TryParse<ApplicationStage>(q.Stage, out var stage))
            query = query.Where(x => x.a.Stage == stage);

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var kw = q.Search.Trim().ToLower();
            query = query.Where(x =>
                x.ap.FullName.ToLower().Contains(kw) ||
                x.ap.Phones.Any(p => p.Phone.Contains(kw)) ||
                x.ap.Emails.Any(e => e.Email.ToLower().Contains(kw)));
        }

        return await query
            .OrderByDescending(x => x.a.CreatedAt)
            .Select(x => new ApplicationSummaryResponse
            {
                Id = x.a.Id,
                ApplicantId = x.ap.Id,
                FullName = x.ap.FullName,
                PrimaryPhone = x.ap.Phones.Where(p => p.IsPrimary).Select(p => p.Phone).FirstOrDefault(),
                PrimaryEmail = x.ap.Emails.Where(e => e.IsPrimary).Select(e => e.Email).FirstOrDefault(),
                TopEducationLevel = x.ap.Educations.OrderByDescending(e => e.EducationLevel).Select(e => e.EducationLevel.ToString()).FirstOrDefault(),
                RecentCompany = x.ap.Experiences.OrderByDescending(e => e.StartDate).Select(e => e.RecentWorkplace ?? e.CompanyName).FirstOrDefault(),
                SourceChannel = x.a.SourceChannel.ToString(),
                Stage = x.a.Stage.ToString(),
                AverageScore = x.a.Evaluations.Any() ? x.a.Evaluations.Average(e => e.Score) : 0,
                AppliedAt = x.a.CreatedAt,
                IsNew = x.a.CreatedAt >= cutoff,
            })
            .ToListAsync(ct);
    }
}
