namespace Application;

public sealed class CreateJobPostingCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobPostingCommand, Guid>
{
    public async Task<Guid> Handle(CreateJobPostingCommand cmd, CancellationToken ct)
    {
        var request = await unitOfWork.Repository<RecruitmentRequest>()
            .FindAsync(r => r.Id == cmd.RecruitmentRequestId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("RecruitmentRequest", cmd.RecruitmentRequestId));

        if (request.Status != RecruitmentRequestStatus.Approved)
            throw new BadRequestException("Chỉ có thể tạo JobPosting khi phiếu đã được duyệt.");

        var costStatus = cmd.EstimatedCost.HasValue && cmd.EstimatedCost > 0
            ? JobPostingCostStatus.PendingApproval
            : JobPostingCostStatus.NotRequired;

        if (!Enum.TryParse<RecruitmentChannel>(cmd.Channel, ignoreCase: true, out var channel))
            throw new BadRequestException($"Channel không hợp lệ: {cmd.Channel}");

        var posting = new JobPosting
        {
            Id = Guid.NewGuid(),
            RecruitmentRequestId = cmd.RecruitmentRequestId,
            Title = cmd.Title,
            Channel = channel,
            PostUrl = cmd.PostUrl,
            EstimatedCost = cmd.EstimatedCost,
            CostStatus = costStatus,
            PostedAt = cmd.PostedAt,
            ExpiresAt = cmd.ExpiresAt
        };

        await unitOfWork.Repository<JobPosting>().AddAsync(posting);
        await unitOfWork.EnsureSaveAsync(ct);
        return posting.Id;
    }
}
