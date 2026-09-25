namespace Application;

public sealed class CompleteInterviewScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteInterviewScheduleCommand, Unit>
{
    public async Task<Unit> Handle(CompleteInterviewScheduleCommand cmd, CancellationToken ct)
    {
        var schedule = await unitOfWork.Repository<Domain.InterviewSchedule>()
            .FindAsync(s => s.Id == cmd.ScheduleId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("InterviewSchedule", cmd.ScheduleId));

        if (schedule.Status != InterviewScheduleStatus.Scheduled)
            throw new BadRequestException("Chỉ có thể hoàn thành lịch phỏng vấn đang ở trạng thái Scheduled.");

        schedule.Status = InterviewScheduleStatus.Completed;
        schedule.InterviewResult = cmd.InterviewResult;
        schedule.CompletedAt = DateTimeOffset.UtcNow;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
