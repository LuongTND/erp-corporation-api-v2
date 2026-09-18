namespace Application;

public sealed class HireCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<HireCandidateCommand, Unit>
{
    public async Task<Unit> Handle(HireCandidateCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage != ApplicationStage.Interview)
            throw new BadRequestException("Chỉ có thể tuyển ứng viên đang ở giai đoạn phỏng vấn.");

        application.Stage = ApplicationStage.Hired;
        application.TrialStartDate = cmd.TrialStartDate;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
