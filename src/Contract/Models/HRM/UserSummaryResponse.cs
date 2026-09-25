namespace Contract;

public sealed class UserSummaryResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset JoinDate { get; init; }
    public IEnumerable<LabelResponse> Labels { get; init; } = [];
}
