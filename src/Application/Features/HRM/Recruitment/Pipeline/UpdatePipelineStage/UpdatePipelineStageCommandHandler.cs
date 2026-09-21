namespace Application;

public sealed class UpdatePipelineStageCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePipelineStageCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePipelineStageCommand cmd, CancellationToken ct)
    {
        var stage = await unitOfWork.Repository<Domain.RecruitmentPipelineStage>()
            .FindTrackedAsync(s => s.Id == cmd.StageId, ct)
            ?? throw new NotFoundException("Vòng không tồn tại.");

        stage.Name = cmd.Name;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
