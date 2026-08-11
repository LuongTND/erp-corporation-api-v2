namespace Application;

public sealed class UpdatePayrollEntryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePayrollEntryCommand>
{
    public async Task Handle(UpdatePayrollEntryCommand cmd, CancellationToken ct)
    {
        var entry = await unitOfWork.Repository<PayrollEntry>()
            .FindTrackedAsync(e => e.Id == cmd.EntryId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("PayrollEntry", cmd.EntryId));

        var run = await unitOfWork.Repository<PayrollRun>()
            .FindAsync(r => r.Id == entry.PayrollRunId, ct);

        if (run?.Status == PayrollRunStatus.Finalized)
            throw new BadRequestException("Không thể sửa bảng lương đã được chốt.");

        entry.HoursWorked = cmd.HoursWorked;
        entry.BonusAmount = cmd.BonusAmount;
        entry.SocialInsurance = cmd.SocialInsurance;
        entry.HealthInsurance = cmd.HealthInsurance;
        entry.UnemploymentIns = cmd.UnemploymentIns;
        entry.PersonalIncomeTax = cmd.PersonalIncomeTax;
        entry.Note = cmd.Note;

        var deductions = (cmd.SocialInsurance ?? 0)
            + (cmd.HealthInsurance ?? 0)
            + (cmd.UnemploymentIns ?? 0)
            + (cmd.PersonalIncomeTax ?? 0);

        entry.GrossPay = entry.HourlyRateSnapshot * cmd.HoursWorked;
        entry.TotalDeductions = deductions;
        entry.NetPay = entry.GrossPay + cmd.BonusAmount - deductions;

        await unitOfWork.EnsureSaveAsync(ct);
    }
}
