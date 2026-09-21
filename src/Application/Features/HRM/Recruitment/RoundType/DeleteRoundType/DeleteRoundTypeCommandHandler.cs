namespace Application;

public sealed class DeleteRoundTypeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRoundTypeCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoundTypeCommand cmd, CancellationToken ct)
    {
        var roundType = await unitOfWork.Repository<Domain.RoundType>()
            .FindAsync(r => r.Id == cmd.Id, ct)
            ?? throw new NotFoundException("Loại vòng không tồn tại.");

        if (roundType.IsSystem)
            throw new BadRequestException("Không thể xoá loại vòng mặc định của hệ thống.");

        var inUse = await unitOfWork.Repository<InterviewRuleConfigStep>()
            .AnyAsync(s => s.RoundTypeId == cmd.Id, ct);
        if (inUse)
            throw new ConflictException("Loại vòng đang được sử dụng trong quy trình tuyển dụng, không thể xoá.");

        await unitOfWork.Repository<Domain.RoundType>().RemoveAsync(roundType);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
