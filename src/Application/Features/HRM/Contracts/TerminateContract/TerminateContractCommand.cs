namespace Application;

public sealed record TerminateContractCommand(
    Guid UserId,
    Guid ContractId,
    string? Reason
) : IRequest;
