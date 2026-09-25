namespace API;

[Authorize]
[ApiController]
[Route("api/users/{userId:guid}/documents")]
public sealed class DocumentsController(ISender sender) : ControllerBase
{
    [HasPermission(DocumentPermissions.View)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDocumentResponse>>>> GetDocuments(
        Guid userId, CancellationToken ct)
        => Ok(ApiResponse<IEnumerable<EmployeeDocumentResponse>>.Ok(
            await sender.Send(new GetDocumentsQuery(userId), ct)));

    [HasPermission(DocumentPermissions.Upload)]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<EmployeeDocumentResponse>>> UploadDocument(
        Guid userId,
        [FromForm] UploadDocumentRequest request,
        CancellationToken ct)
    {
        using var stream = request.File.OpenReadStream();
        var cmd = new UploadDocumentCommand(
            userId, request.Category, request.CustomName, stream,
            request.File.ContentType, request.File.FileName, request.File.Length,
            request.IssuedDate, request.ExpiryDate, request.Notes,
            IsVisibleToEmployee: request.IsVisibleToEmployee,
            IsHrUpload: true);
        return Ok(ApiResponse<EmployeeDocumentResponse>.Ok(await sender.Send(cmd, ct)));
    }

    [HasPermission(DocumentPermissions.Delete)]
    [HttpDelete("{documentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteDocument(
        Guid userId, Guid documentId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new DeleteDocumentCommand(userId, documentId, IsHrDelete: true), ct)));

    [HasPermission(DocumentPermissions.ToggleVisibility)]
    [HttpPatch("{documentId:guid}/visibility")]
    public async Task<ActionResult<ApiResponse<Unit>>> ToggleVisibility(
        Guid userId, Guid documentId, [FromBody] bool isVisibleToEmployee, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new ToggleDocumentVisibilityCommand(userId, documentId, isVisibleToEmployee), ct)));
}
