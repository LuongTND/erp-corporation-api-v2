namespace Application;

public sealed class CreateInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInterviewScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage is not (ApplicationStage.Screening or ApplicationStage.Interview))
            throw new BadRequestException("Ứng viên phải qua sơ loại CV trước khi hẹn lịch phỏng vấn.");

        var schedule = new Domain.InterviewSchedule
        {
            Id = Guid.NewGuid(),
            ApplicationId = cmd.ApplicationId,
            InterviewerId = cmd.InterviewerId,
            ScheduledAt = cmd.ScheduledAt,
            Location = cmd.Location,
            LocationNote = cmd.LocationNote,
            Notes = cmd.Notes,
            Status = InterviewScheduleStatus.Scheduled
        };

        await unitOfWork.Repository<Domain.InterviewSchedule>().AddAsync(schedule);
        await unitOfWork.EnsureSaveAsync(ct);
        return schedule.Id;
    }
}
