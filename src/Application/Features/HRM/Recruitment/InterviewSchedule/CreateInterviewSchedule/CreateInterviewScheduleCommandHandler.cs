namespace Application;

public sealed class CreateInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInterviewScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage is not (CandidateStage.Screening or CandidateStage.StoreInterview or CandidateStage.ProductionInterview))
            throw new BadRequestException("Ứng viên phải qua sơ loại CV trước khi hẹn lịch phỏng vấn.");

        var schedule = new Domain.InterviewSchedule
        {
            Id = Guid.NewGuid(),
            CandidateId = cmd.CandidateId,
            InterviewerId = cmd.InterviewerId,
            ScheduledAt = cmd.ScheduledAt,
            Location = cmd.Location,
            LocationNote = cmd.LocationNote,
            Notes = cmd.Notes,
            Status = InterviewScheduleStatus.Scheduled,
        };

        await unitOfWork.Repository<Domain.InterviewSchedule>().AddAsync(schedule);
        await unitOfWork.EnsureSaveAsync(ct);
        return schedule.Id;
    }
}
