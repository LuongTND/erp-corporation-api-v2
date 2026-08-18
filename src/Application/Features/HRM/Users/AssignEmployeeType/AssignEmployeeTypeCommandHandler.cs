namespace Application;

public sealed class AssignEmployeeTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AssignEmployeeTypeCommand, Unit>
{
    public async Task<Unit> Handle(AssignEmployeeTypeCommand cmd, CancellationToken ct)
    {
        var user = await unitOfWork.Repository<User>()
            .FindTrackedAsync(u => u.Id == cmd.UserId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", cmd.UserId));

        if (cmd.EmployeeTypeId.HasValue)
        {
            var typeExists = await unitOfWork.Repository<EmployeeType>()
                .AnyAsync(e => e.Id == cmd.EmployeeTypeId.Value && e.IsActive && !e.IsDeleted, ct);
            if (!typeExists)
                throw new NotFoundException(ExceptionMessages.NotFound("EmployeeType", cmd.EmployeeTypeId.Value));
        }

        user.EmployeeTypeId = cmd.EmployeeTypeId;

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
