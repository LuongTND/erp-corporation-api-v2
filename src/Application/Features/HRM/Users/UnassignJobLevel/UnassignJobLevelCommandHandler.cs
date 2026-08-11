namespace Application;

public sealed class UnassignJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UnassignJobLevelCommand, Unit>
{
    public async Task<Unit> Handle(UnassignJobLevelCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        user.JobLevelId = null;
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
