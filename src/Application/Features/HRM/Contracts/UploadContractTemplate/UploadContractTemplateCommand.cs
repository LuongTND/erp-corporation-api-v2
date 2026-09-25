namespace Application;

public sealed record UploadContractTemplateCommand(
    string Name,
    string? Description,
    Stream FileStream,
    string OriginalFileName,
    string ContentType
) : IRequest<ContractTemplateResponse>;
