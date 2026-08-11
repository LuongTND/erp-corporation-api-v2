namespace Application;

public sealed record CreatePayrollRunCommand(int Month, int Year, string? Note) : IRequest<Guid>;
