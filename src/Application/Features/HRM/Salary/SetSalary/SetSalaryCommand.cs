namespace Application;

// Dùng cho cả create lần đầu lẫn cập nhật lương (tạo record mới, close record cũ)
public sealed record SetSalaryCommand(
    Guid UserId,
    decimal HourlyRate,
    DateOnly EffectiveFrom,
    string? Reason
) : IRequest<Guid>;
