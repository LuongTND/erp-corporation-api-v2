namespace Application;

public sealed class StoreMemberResponse
{
    public Guid UserStoreId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string? JobLevelName { get; init; }
    public bool IsHomeStore { get; init; }
    public DateOnly StartDate { get; init; }
}
