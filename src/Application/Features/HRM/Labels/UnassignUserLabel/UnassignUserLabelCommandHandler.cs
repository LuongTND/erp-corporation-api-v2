namespace Application;

public sealed class UnassignUserLabelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UnassignUserLabelCommand, Unit>
{
    public async Task<Unit> Handle(UnassignUserLabelCommand cmd, CancellationToken ct)
    {
        var userLabel = await unitOfWork.Repository<UserLabel>()
            .FindTrackedAsync(ul => ul.UserId == cmd.UserId && ul.LabelId == cmd.LabelId, ct)
            ?? throw new NotFoundException("Nhãn chưa được gán cho nhân sự này");

        await unitOfWork.Repository<UserLabel>().RemoveAsync(userLabel);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
