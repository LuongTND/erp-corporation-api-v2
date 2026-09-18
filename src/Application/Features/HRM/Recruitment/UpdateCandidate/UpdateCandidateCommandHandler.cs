namespace Application;

public sealed class UpdateCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCandidateCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCandidateCommand cmd, CancellationToken ct)
    {
        var candidate = await unitOfWork.Repository<Candidate>()
            .FindAsync(c => c.Id == cmd.CandidateId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Candidate", cmd.CandidateId));

        if (!Enum.TryParse<RecruitmentChannel>(cmd.SourceChannel, ignoreCase: true, out var sourceChannel))
            throw new BadRequestException($"SourceChannel không hợp lệ: {cmd.SourceChannel}");

        candidate.FullName = cmd.FullName;
        candidate.Email = cmd.Email;
        candidate.Phone = cmd.Phone;
        candidate.SourceChannel = sourceChannel;
        candidate.Notes = cmd.Notes;
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
