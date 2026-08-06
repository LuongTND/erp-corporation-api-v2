namespace Application;

public sealed class DeleteCustomFieldDefinitionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCustomFieldDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomFieldDefinitionCommand cmd, CancellationToken ct)
    {
        var definition = await unitOfWork.Repository<CustomFieldDefinition>()
            .FindTrackedAsync(d => d.Id == cmd.DefinitionId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("CustomFieldDefinition", cmd.DefinitionId));

        if (definition.IsSystem)
            throw new BadRequestException("Không thể xóa trường hệ thống.");

        var hasValues = await unitOfWork.Repository<UserCustomFieldValue>()
            .AnyAsync(v => v.DefinitionId == cmd.DefinitionId, ct);
        if (hasValues)
            throw new ConflictException("Không thể xóa trường đang có dữ liệu nhân viên. Hãy vô hiệu hóa thay vì xóa.");

        await unitOfWork.Repository<CustomFieldDefinition>().RemoveAsync(definition);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
