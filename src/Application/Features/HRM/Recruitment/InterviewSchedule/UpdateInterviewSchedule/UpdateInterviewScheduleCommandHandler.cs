namespace Application;

public sealed class UpdateInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateInterviewScheduleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var schedule = await unitOfWork.Repository<Domain.InterviewSchedule>()
            .FindTrackedAsync(s => s.Id == cmd.ScheduleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("InterviewSchedule", cmd.ScheduleId));

        if (schedule.Status == InterviewScheduleStatus.Completed)
            throw new BadRequestException("Không thể sửa lịch phỏng vấn đã hoàn thành.");

        if (cmd.InterviewerId.HasValue && cmd.InterviewerId != schedule.InterviewerId)
            _ = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.InterviewerId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.InterviewerId.Value));

        schedule.ScheduledAt = cmd.ScheduledAt;
        schedule.LocationNote = cmd.LocationNote;
        schedule.Notes = cmd.Notes;
        schedule.InterviewerId = cmd.InterviewerId;
        schedule.Status = cmd.Status;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
