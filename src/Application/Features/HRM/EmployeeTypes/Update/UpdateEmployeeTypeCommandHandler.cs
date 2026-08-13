namespace Application;

public sealed class UpdateEmployeeTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEmployeeTypeCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeTypeCommand cmd, CancellationToken ct)
    {
        var employeeType = await unitOfWork.Repository<EmployeeType>()
            .FindTrackedAsync(e => e.Id == cmd.EmployeeTypeId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("EmployeeType", cmd.EmployeeTypeId));

        var code = cmd.Code.ToUpperInvariant();

        if (await unitOfWork.Repository<EmployeeType>().AnyAsync(e => e.Code == code && e.Id != cmd.EmployeeTypeId, ct))
            throw new ConflictException(ExceptionMessages.AlreadyExists("Code", code));

        if (await unitOfWork.Repository<EmployeeType>().AnyAsync(e => e.Name == cmd.Name && e.Id != cmd.EmployeeTypeId, ct))
            throw new ConflictException(ExceptionMessages.AlreadyExists("Name", cmd.Name));

        employeeType.Name = cmd.Name;
        employeeType.Code = code;
        employeeType.Description = cmd.Description;
        employeeType.IsActive = cmd.IsActive;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
