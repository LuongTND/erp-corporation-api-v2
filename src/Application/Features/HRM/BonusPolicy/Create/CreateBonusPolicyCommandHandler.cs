namespace Application;

public sealed class CreateBonusPolicyCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateBonusPolicyCommand, Guid>
{
    public async Task<Guid> Handle(CreateBonusPolicyCommand cmd, CancellationToken ct)
    {
        var nameExists = await unitOfWork.Repository<Domain.BonusPolicy>()
            .AnyAsync(b => b.Name == cmd.Name, ct);
        if (nameExists)
            throw new ConflictException(ExceptionMessages.AlreadyExists("BonusPolicy", cmd.Name));

        var policy = new Domain.BonusPolicy
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Description = cmd.Description
        };

        await unitOfWork.Repository<Domain.BonusPolicy>().AddAsync(policy);
        await unitOfWork.EnsureSaveAsync(ct);
        return policy.Id;
    }
}
