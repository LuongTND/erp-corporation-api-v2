namespace Application;

public sealed class AssignCandidateToProductionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignCandidateToProductionCommand, Unit>
{
    public async Task<Unit> Handle(AssignCandidateToProductionCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage != CandidateStage.Screening)
            throw new BadRequestException("Chỉ có thể chuyển sang phỏng vấn sản xuất từ giai đoạn Screening.");

        candidate.Stage = CandidateStage.ProductionInterview;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
