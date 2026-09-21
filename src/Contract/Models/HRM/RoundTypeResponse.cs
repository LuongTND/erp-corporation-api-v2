namespace Contract;

public class RoundTypeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsSystem { get; init; }
    public int DisplayOrder { get; init; }
}
