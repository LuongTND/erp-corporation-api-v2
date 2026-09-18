namespace Application;

public sealed class ScreenCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ScreenCandidateCommand, Unit>
{
    public async Task<Unit> Handle(ScreenCandidateCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage != ApplicationStage.New)
            throw new BadRequestException("Chỉ có thể sàng lọc ứng viên ở giai đoạn New.");

        application.Stage = ApplicationStage.Screening;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
