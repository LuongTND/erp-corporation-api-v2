namespace Application;

public sealed class RejectCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectCandidateCommand, Unit>
{
    public async Task<Unit> Handle(RejectCandidateCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage is ApplicationStage.Hired or ApplicationStage.Rejected)
            throw new BadRequestException("Ứng viên đã được tuyển hoặc đã từ chối.");

        application.Stage = ApplicationStage.Rejected;
        application.RejectionReason = cmd.RejectionReason;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
