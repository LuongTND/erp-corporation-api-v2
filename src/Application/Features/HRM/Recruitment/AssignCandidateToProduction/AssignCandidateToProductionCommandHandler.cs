namespace Application;

public sealed class AssignCandidateToProductionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignCandidateToProductionCommand, Unit>
{
    public async Task<Unit> Handle(AssignCandidateToProductionCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage != ApplicationStage.Screening)
            throw new BadRequestException("Chỉ có thể chuyển sang phỏng vấn từ giai đoạn Screening.");

        application.Stage = ApplicationStage.Interview;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
