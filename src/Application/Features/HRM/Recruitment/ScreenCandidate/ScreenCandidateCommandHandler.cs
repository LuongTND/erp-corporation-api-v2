namespace Application;

public sealed class ScreenCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ScreenCandidateCommand, Unit>
{
    public async Task<Unit> Handle(ScreenCandidateCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (candidate.Stage != CandidateStage.New)
            throw new BadRequestException("Chỉ có thể sàng lọc ứng viên ở giai đoạn New.");

        candidate.Stage = CandidateStage.Screening;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
