namespace Application;

public sealed class AssignUserLabelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignUserLabelCommand, Unit>
{
    public async Task<Unit> Handle(AssignUserLabelCommand cmd, CancellationToken ct)
    {
        var userExists = await unitOfWork.Repository<User>().AnyAsync(u => u.Id == cmd.UserId && u.IsActive, ct);
        if (!userExists) throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        var label = await unitOfWork.Repository<Label>().FindAsync(l => l.Id == cmd.LabelId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Label", cmd.LabelId));
        if (!label.IsActive) throw new BadRequestException("Nhãn không còn hoạt động");

        var already = await unitOfWork.Repository<UserLabel>()
            .AnyAsync(ul => ul.UserId == cmd.UserId && ul.LabelId == cmd.LabelId, ct);
        if (already) return Unit.Value;

        await unitOfWork.Repository<UserLabel>().AddAsync(
            new UserLabel { Id = Guid.NewGuid(), UserId = cmd.UserId, LabelId = cmd.LabelId });
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
