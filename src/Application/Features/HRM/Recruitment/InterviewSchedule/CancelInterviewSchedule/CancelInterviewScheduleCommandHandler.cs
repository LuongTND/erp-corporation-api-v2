namespace Application;

public sealed class CancelInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelInterviewScheduleCommand, Unit>
{
    public async Task<Unit> Handle(CancelInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var schedule = await unitOfWork.Repository<Domain.InterviewSchedule>()
            .FindAsync(s => s.Id == cmd.ScheduleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("InterviewSchedule", cmd.ScheduleId));

        if (schedule.Status != InterviewScheduleStatus.Scheduled)
            throw new BadRequestException("Chỉ có thể huỷ lịch phỏng vấn đang ở trạng thái Scheduled.");

        schedule.Status = InterviewScheduleStatus.Cancelled;
        if (cmd.Reason != null)
            schedule.Notes = cmd.Reason;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
