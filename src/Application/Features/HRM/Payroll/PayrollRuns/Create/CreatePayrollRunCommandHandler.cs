namespace Application;

public sealed class CreatePayrollRunCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePayrollRunCommand, Guid>
{
    public async Task<Guid> Handle(CreatePayrollRunCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<PayrollRun>()
            .AnyAsync(p => p.Month == cmd.Month && p.Year == cmd.Year, ct);
        if (exists)
            throw new ConflictException($"Bảng lương tháng {cmd.Month}/{cmd.Year} đã tồn tại.");

        var run = new PayrollRun
        {
            Id = Guid.NewGuid(),
            Month = cmd.Month,
            Year = cmd.Year,
            Note = cmd.Note,
            Status = PayrollRunStatus.Draft
        };
        await unitOfWork.Repository<PayrollRun>().AddAsync(run);
        await unitOfWork.EnsureSaveAsync(ct);
        return run.Id;
    }
}
