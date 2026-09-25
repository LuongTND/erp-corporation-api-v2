namespace Application;

public sealed class CreateEmployeeTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEmployeeTypeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeTypeCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.ToUpperInvariant();

        if (await unitOfWork.Repository<EmployeeType>().AnyAsync(e => e.Code == code, ct))
            throw new ConflictException(ExceptionMessages.AlreadyExists("Code", code));

        if (await unitOfWork.Repository<EmployeeType>().AnyAsync(e => e.Name == cmd.Name, ct))
            throw new ConflictException(ExceptionMessages.AlreadyExists("Name", cmd.Name));

        var employeeType = new EmployeeType
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Code = code,
            Description = cmd.Description,
            IsActive = true
        };

        await unitOfWork.Repository<EmployeeType>().AddAsync(employeeType);
        await unitOfWork.EnsureSaveAsync(ct);
        return employeeType.Id;
    }
}
