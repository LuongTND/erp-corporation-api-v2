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
            throw new BadRequestException("Chỉ có thể tạo tin tuyển dụng cho phiếu đề xuất đã được duyệt.");

        if (cmd.AssignedToUserId.HasValue)
            _ = await unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == cmd.AssignedToUserId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.AssignedToUserId.Value));

        var posting = new JobPosting
        {
            Id = Guid.NewGuid(),
            RecruitmentRequestId = cmd.RecruitmentRequestId,
            Channel = cmd.Channel,
            Title = cmd.Title,
            WorkLocation = cmd.WorkLocation,
            PostUrl = cmd.PostUrl,
            Requirements = cmd.Requirements,
            AssignedToUserId = cmd.AssignedToUserId,
            ExpiresAt = cmd.ExpiresAt,
            Status = JobPostingStatus.Draft,
        };

        await unitOfWork.Repository<Domain.JobPosting>().AddAsync(posting);
        await unitOfWork.EnsureSaveAsync(ct);
        return posting.Id;
    }
}
