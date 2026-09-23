namespace Application;

public sealed class GetInterviewSchedulesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetInterviewSchedulesQuery, IEnumerable<InterviewScheduleListItemResponse>>
{
    public async Task<IEnumerable<InterviewScheduleListItemResponse>> Handle(GetInterviewSchedulesQuery q, CancellationToken ct)
    {
        var query =
            from s in unitOfWork.Repository<Domain.InterviewSchedule>().Query()
            join a in unitOfWork.Repository<Domain.Application>().Query() on s.ApplicationId equals a.Id
            join ap in unitOfWork.Repository<Applicant>().Query() on a.ApplicantId equals ap.Id
            join u in unitOfWork.Repository<User>().Query() on s.InterviewerId equals u.Id into uj
            from u in uj.DefaultIfEmpty()
            where a.JobPostingId == q.JobPostingId
            select new { s, ap, u };

        if (q.From.HasValue)
        {
            var fromOffset = q.From.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
            query = query.Where(x => x.s.ScheduledAt >= fromOffset);
        }

        if (q.To.HasValue)
        {
            var toOffset = q.To.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
            query = query.Where(x => x.s.ScheduledAt <= toOffset);
        }

        if (q.InterviewerId.HasValue)
            query = query.Where(x => x.s.InterviewerId == q.InterviewerId);

        return await query
            .OrderBy(x => x.s.ScheduledAt)
            .Select(x => new InterviewScheduleListItemResponse
            {
                Id = x.s.Id,
                ApplicationId = x.s.ApplicationId,
                ApplicantName = x.ap.FullName,
                ScheduledAt = x.s.ScheduledAt,
                LocationNote = x.s.LocationNote,
                Notes = x.s.Notes,
                Status = x.s.Status.ToString(),
                InterviewerId = x.s.InterviewerId,
                InterviewerName = x.u != null ? x.u.FullName : null,
            })
            .ToListAsync(ct);
    }
}
