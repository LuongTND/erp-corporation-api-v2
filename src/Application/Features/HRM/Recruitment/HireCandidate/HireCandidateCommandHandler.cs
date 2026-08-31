namespace Application;

public sealed class HireCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<HireCandidateCommand, Unit>
{
    public async Task<Unit> Handle(HireCandidateCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage is not (CandidateStage.ProductionInterview or CandidateStage.StoreInterview))
            throw new BadRequestException("Chỉ có thể tuyển ứng viên đang ở giai đoạn phỏng vấn.");

        candidate.Stage = CandidateStage.Hired;
        candidate.TrialStartDate = cmd.TrialStartDate;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
