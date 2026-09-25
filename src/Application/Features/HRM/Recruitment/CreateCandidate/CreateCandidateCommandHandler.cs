namespace Application;

public sealed class CreateCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCandidateCommand, Guid>
{
    public async Task<Guid> Handle(CreateCandidateCommand cmd, CancellationToken ct)
    {
        if (cmd.RecruitmentRequestId.HasValue)
        {
            var request = await unitOfWork.Repository<RecruitmentRequest>()
                .FindAsync(r => r.Id == cmd.RecruitmentRequestId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RecruitmentRequestId.Value));

            if (request.Status != RecruitmentRequestStatus.Approved)
                throw new BadRequestException("Phiếu tuyển dụng chưa được duyệt.");
        }

        if (!Enum.TryParse<RecruitmentChannel>(cmd.SourceChannel, ignoreCase: true, out var sourceChannel))
            throw new BadRequestException($"SourceChannel không hợp lệ: {cmd.SourceChannel}");

        var candidate = new Candidate
        {
            Id = Guid.NewGuid(),
            RecruitmentRequestId = cmd.RecruitmentRequestId,
            FullName = cmd.FullName,
            Email = cmd.Email,
            Phone = cmd.Phone,
            SourceChannel = sourceChannel,
            Notes = cmd.Notes,
            Stage = CandidateStage.New
        };

        await unitOfWork.Repository<Candidate>().AddAsync(candidate);
        await unitOfWork.EnsureSaveAsync(ct);
        return candidate.Id;
    }
}
