namespace Application;

public sealed class AddPipelineStageCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddPipelineStageCommand, Guid>
{
    public async Task<Guid> Handle(AddPipelineStageCommand cmd, CancellationToken ct)
    {
        var pipeline = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .FindAsync(p => p.Id == cmd.PipelineId, ct)
            ?? throw new NotFoundException("Quy trình không tồn tại.");

        var roundType = await unitOfWork.Repository<Domain.RoundType>()
            .FindAsync(r => r.Id == cmd.RoundTypeId, ct)
            ?? throw new NotFoundException("Loại vòng không tồn tại.");

        var stage = new Domain.RecruitmentPipelineStage
        {
            Id = Guid.NewGuid(),
            PipelineId = cmd.PipelineId,
            RoundTypeId = cmd.RoundTypeId,
            Name = cmd.Name ?? roundType.Name,
            DisplayOrder = cmd.DisplayOrder,
            IsFixed = false,
        };

        await unitOfWork.Repository<Domain.RecruitmentPipelineStage>().AddAsync(stage);
        await unitOfWork.EnsureSaveAsync(ct);
        return stage.Id;
    }
}
