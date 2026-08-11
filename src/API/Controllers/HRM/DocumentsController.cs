namespace API;

[Authorize]
[ApiController]
[Route("api/users/{userId:guid}/documents")]
public sealed class DocumentsController(ISender sender) : ControllerBase
{
    [HasPermission("users:view")]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDocumentResponse>>>> GetDocuments(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<EmployeeDocumentResponse>>.Ok(
            await sender.Send(new GetDocumentsQuery(userId), ct)));

    [HasPermission("users:edit")]
    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
    public async Task<ActionResult<ApiResponse<EmployeeDocumentResponse>>> UploadDocument(
        Guid userId,
        [FromForm] IFormFile file,
        [FromForm] DocumentCategory category,
        [FromForm] string? customName,
        [FromForm] DateTimeOffset? issuedDate,
        [FromForm] DateTimeOffset? expiryDate,
        [FromForm] string? notes,
        CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var cmd = new UploadDocumentCommand(
            userId, category, customName, stream,
            file.ContentType, file.FileName, file.Length,
            issuedDate, expiryDate, notes);
        return Ok(ApiResponse<EmployeeDocumentResponse>.Ok(await sender.Send(cmd, ct)));
    }

    [HasPermission("users:edit")]
    [HttpDelete("{documentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteDocument(
        Guid userId, Guid documentId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteDocumentCommand(userId, documentId), ct)));
}
