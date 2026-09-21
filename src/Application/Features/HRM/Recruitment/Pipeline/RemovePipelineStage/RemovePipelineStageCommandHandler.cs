namespace Application;

public sealed class RemovePipelineStageCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemovePipelineStageCommand, Unit>
{
    public async Task<Unit> Handle(RemovePipelineStageCommand cmd, CancellationToken ct)
    {
        var stage = await unitOfWork.Repository<Domain.RecruitmentPipelineStage>()
            .FindTrackedAsync(s => s.Id == cmd.StageId, ct)
            ?? throw new NotFoundException("Vòng không tồn tại.");

        if (stage.IsFixed)
            throw new BadRequestException("Không thể xóa vòng cố định của hệ thống.");

        await unitOfWork.Repository<Domain.RecruitmentPipelineStage>().RemoveAsync(stage);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
