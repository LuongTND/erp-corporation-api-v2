namespace Application;

public sealed class RejectCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RejectCandidateCommand, Unit>
{
    public async Task<Unit> Handle(RejectCandidateCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage is CandidateStage.Hired or CandidateStage.Rejected)
            throw new BadRequestException("Ứng viên đã được tuyển hoặc đã từ chối.");

        candidate.Stage = CandidateStage.Rejected;
        candidate.RejectionReason = cmd.RejectionReason;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
