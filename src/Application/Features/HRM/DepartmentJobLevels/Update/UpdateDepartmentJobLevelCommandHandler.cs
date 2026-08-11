namespace Application;

public sealed class UpdateDepartmentJobLevelCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateDepartmentJobLevelCommand, Unit>
{
    public async Task<Unit> Handle(UpdateDepartmentJobLevelCommand cmd, CancellationToken ct)
    {
        var djl = await unitOfWork.Repository<DepartmentJobLevel>()
            .FindTrackedAsync(d => d.Id == cmd.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("DepartmentJobLevel", cmd.Id));

        if (cmd.BonusPolicyId.HasValue)
        {
            var policyExists = await unitOfWork.Repository<Domain.BonusPolicy>()
                .AnyAsync(b => b.Id == cmd.BonusPolicyId.Value && !b.IsDeleted, ct);
            if (!policyExists)
                throw new NotFoundException(ExceptionMessages.NotFound("BonusPolicy", cmd.BonusPolicyId.Value));
        }

        djl.BonusPolicyId = cmd.BonusPolicyId;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
