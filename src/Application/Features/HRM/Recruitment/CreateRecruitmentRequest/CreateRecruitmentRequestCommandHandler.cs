namespace Application;

public sealed class CreateRecruitmentRequestCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext currentUser)
    : IRequestHandler<CreateRecruitmentRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateRecruitmentRequestCommand cmd, CancellationToken ct)
    {
        if (cmd.RequestContext == RecruitmentRequestContext.Department)
        {
            if (!cmd.DepartmentId.HasValue)
                throw new BadRequestException("DepartmentId bắt buộc khi RequestContext = Department.");

            _ = await unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == cmd.DepartmentId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Department", cmd.DepartmentId.Value));
        }
        else
        {
            if (!cmd.StoreId.HasValue)
                throw new BadRequestException("StoreId bắt buộc khi RequestContext = Store.");

            _ = await unitOfWork.Repository<Store>()
                .FindAsync(s => s.Id == cmd.StoreId.Value, ct)
                ?? throw new NotFoundException(ExceptionMessages.NotFound("Store", cmd.StoreId.Value));
        }

        var request = new RecruitmentRequest
        {
            Id = Guid.NewGuid(),
            RequestCode = $"RQ-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            RequestContext = cmd.RequestContext,
            DepartmentId = cmd.DepartmentId,
            StoreId = cmd.StoreId,
            PositionTitle = cmd.PositionTitle,
            RequestedByUserId = currentUser.UserId,
            Headcount = cmd.Headcount,
            Reason = cmd.Reason,
            JobDescription = cmd.JobDescription,
            RequiredByDate = cmd.RequiredByDate,
            Status = RecruitmentRequestStatus.Draft
        };

        await unitOfWork.Repository<RecruitmentRequest>().AddAsync(request);
        await unitOfWork.EnsureSaveAsync(ct);
        return request.Id;
    }
}
