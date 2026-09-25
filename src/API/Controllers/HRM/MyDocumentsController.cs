namespace API;

[Authorize]
[ApiController]
[Route("api/me/documents")]
public sealed class MyDocumentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeDocumentResponse>>>> GetMyDocuments(
        CancellationToken ct)
    {
        var callerId = User.GetUserId();
        return Ok(ApiResponse<IEnumerable<EmployeeDocumentResponse>>.Ok(
            await sender.Send(new GetDocumentsQuery(callerId, SelfView: true, CallerId: callerId), ct)));
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<ActionResult<ApiResponse<EmployeeDocumentResponse>>> UploadMyDocument(
        [FromForm] UploadDocumentRequest request,
        CancellationToken ct)
    {
        var callerId = User.GetUserId();
        using var stream = request.File.OpenReadStream();
        var cmd = new UploadDocumentCommand(
            callerId, request.Category, request.CustomName, stream,
            request.File.ContentType, request.File.FileName, request.File.Length,
            request.IssuedDate, request.ExpiryDate, request.Notes,
            IsVisibleToEmployee: false,
            IsHrUpload: false);
        return Ok(ApiResponse<EmployeeDocumentResponse>.Ok(await sender.Send(cmd, ct)));
    }

    [HttpDelete("{documentId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> DeleteMyDocument(
        Guid documentId, CancellationToken ct)
    {
        var callerId = User.GetUserId();
        return Ok(ApiResponse<Unit>.Ok(
            await sender.Send(new DeleteDocumentCommand(callerId, documentId, IsHrDelete: false), ct)));
    }
}
