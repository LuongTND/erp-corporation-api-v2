namespace Contract;

public sealed class LabelResponse
{
    public Guid   Id       { get; init; }
    public string Name     { get; init; } = string.Empty;
    public string Color    { get; init; } = string.Empty;
    public bool   IsActive { get; init; }
}
