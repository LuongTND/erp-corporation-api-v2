namespace Application;

public sealed class AssignCandidateToStoreCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignCandidateToStoreCommand, Unit>
{
    public async Task<Unit> Handle(AssignCandidateToStoreCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage != CandidateStage.Screening)
            throw new BadRequestException("Chỉ có thể chuyển sang phỏng vấn cửa hàng từ giai đoạn Screening.");

        candidate.Stage = CandidateStage.StoreInterview;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
