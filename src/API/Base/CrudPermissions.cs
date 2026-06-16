namespace API.Base;

public sealed class CrudPermissions
{
    public required string Read { get; init; }
    public required string Create { get; init; }
    public required string Update { get; init; }
    public required string Delete { get; init; }

    public string Get(CrudOperation operation) => operation switch
    {
        CrudOperation.Read => Read,
        CrudOperation.Create => Create,
        CrudOperation.Update => Update,
        CrudOperation.Delete => Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(operation))
    };
}

public enum CrudOperation
{
    Read,
    Create,
    Update,
    Delete
}
