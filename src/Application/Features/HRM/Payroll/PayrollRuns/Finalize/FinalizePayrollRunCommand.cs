namespace Application;

public sealed record FinalizePayrollRunCommand(Guid RunId) : IRequest;
