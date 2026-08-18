namespace Application;

public sealed class GetContractTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetContractTemplateByIdQuery, ContractTemplateDetailResult>
{
    public async Task<ContractTemplateDetailResult> Handle(GetContractTemplateByIdQuery query, CancellationToken ct)
    {
        var template = await unitOfWork.Repository<ContractTemplate>()
            .FindAsync(t => t.Id == query.Id, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound(nameof(ContractTemplate), query.Id));

        return new ContractTemplateDetailResult(template.BlobName, template.OriginalFileName);
    }
}
