namespace Application;

public sealed class FinalizePayrollRunCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<FinalizePayrollRunCommand>
{
    public async Task Handle(FinalizePayrollRunCommand cmd, CancellationToken ct)
    {
        var run = await unitOfWork.Repository<PayrollRun>()
            .FindTrackedAsync(r => r.Id == cmd.RunId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("PayrollRun", cmd.RunId));

        if (run.Status == PayrollRunStatus.Finalized)
            throw new BadRequestException("Bảng lương đã được chốt rồi.");

        run.Status = PayrollRunStatus.Finalized;
        await unitOfWork.EnsureSaveAsync(ct);
    }
}
