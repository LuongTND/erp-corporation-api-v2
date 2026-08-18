namespace API;

[Authorize]
[ApiController]
[Route("api/custom-field-definitions")]
public sealed class CustomFieldDefinitionsController(ISender sender) : ControllerBase
{
    [HasPermission(CustomFieldPermissions.ViewList)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomFieldDefinitionResponse>>>> GetAll(
        [FromQuery] string? module, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<CustomFieldDefinitionResponse>>.Ok(
            await sender.Send(new GetCustomFieldDefinitionsQuery(module), ct)));

    [HasPermission(CustomFieldPermissions.ViewDetail)]
    [HttpGet("{definitionId:guid}")]
    public async Task<ActionResult<ApiResponse<CustomFieldDefinitionResponse>>> GetById(
        Guid definitionId, CancellationToken ct)
        => Ok(ApiResponse<CustomFieldDefinitionResponse>.Ok(await sender.Send(new GetCustomFieldDetailQuery(definitionId), ct)));

    [HasPermission(CustomFieldPermissions.Create)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create(
        [FromBody] CreateCustomFieldDefinitionCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission(CustomFieldPermissions.Update)]
    [HttpPut("{definitionId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(
        Guid definitionId, [FromBody] UpdateCustomFieldDefinitionCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { DefinitionId = definitionId }, ct)));

    [HasPermission(CustomFieldPermissions.Delete)]
    [HttpDelete("{definitionId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(
        Guid definitionId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteCustomFieldDefinitionCommand(definitionId), ct)));
}
