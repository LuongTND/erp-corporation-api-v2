using Microsoft.AspNetCore.Authorization;

namespace Infrastructure;

public sealed class HasPermissionAttribute(string permission)
    : AuthorizeAttribute, IAuthorizationRequirementData
{
    public string Permission { get; } = permission;

    public IEnumerable<IAuthorizationRequirement> GetRequirements()
        => [new PermissionRequirement(Permission)];
}

public sealed record PermissionRequirement(string Permission) : IAuthorizationRequirement;
