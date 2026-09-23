namespace Application;

public sealed class CreateInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateInterviewScheduleCommand, Guid>
{
    public async Task<Guid> Handle(CreateInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindTrackedAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage == ApplicationStage.Hired || application.Stage == ApplicationStage.Rejected)
            throw new BadRequestException("Không thể đặt lịch phỏng vấn cho hồ sơ đã kết thúc.");

        if (cmd.InterviewerId.HasValue)
            _ = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.InterviewerId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.InterviewerId.Value));

        var schedule = new Domain.InterviewSchedule
        {
            Id = Guid.NewGuid(),
            ApplicationId = cmd.ApplicationId,
            ScheduledAt = cmd.ScheduledAt,
            LocationNote = cmd.LocationNote,
            Notes = cmd.Notes,
            InterviewerId = cmd.InterviewerId,
            Status = InterviewScheduleStatus.Scheduled,
        };

        await unitOfWork.Repository<Domain.InterviewSchedule>().AddAsync(schedule);

        // Tự chuyển stage nếu đang New
        if (application.Stage == ApplicationStage.New)
            application.Stage = ApplicationStage.Scheduled;

        await unitOfWork.EnsureSaveAsync(ct);
        return schedule.Id;
    }
}
