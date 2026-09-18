namespace Application;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PermissionInfoAttribute(string name, string description) : Attribute
{
    public string Name        { get; } = name;
    public string Description { get; } = description;
}
