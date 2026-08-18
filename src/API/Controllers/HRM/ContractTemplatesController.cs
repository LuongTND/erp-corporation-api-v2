namespace API;

[Authorize]
[ApiController]
[Route("api/hrm/contract-templates")]
public sealed class ContractTemplatesController(ISender sender, IBlobStorageService blobStorage) : ControllerBase
{
    private const string Container = "contract-templates";

    [HasPermission(ContractTemplatePermissions.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContractTemplateResponse>>>> GetAll(CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<ContractTemplateResponse>>.Ok(
            await sender.Send(new GetContractTemplatesQuery(), ct)));

    [HasPermission(ContractTemplatePermissions.Upload)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContractTemplateResponse>>> Upload(
        [FromForm] string name,
        [FromForm] string? description,
        IFormFile file,
        CancellationToken ct)
        => Ok(ApiResponse<ContractTemplateResponse>.Ok(
            await sender.Send(new UploadContractTemplateCommand(
                name, description, file.OpenReadStream(), file.FileName, file.ContentType), ct)));

    [HasPermission(ContractTemplatePermissions.Download)]
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var template = await sender.Send(new GetContractTemplateByIdQuery(id), ct);
        var stream = await blobStorage.DownloadAsync(Container, template.BlobName, ct);
        var contentType = template.OriginalFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        return File(stream, contentType, template.OriginalFileName);
    }


    [HasPermission(ContractTemplatePermissions.Delete)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object?>>> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteContractTemplateCommand(id), ct);
        return Ok(ApiResponse<object?>.Ok(null));
    }
}
