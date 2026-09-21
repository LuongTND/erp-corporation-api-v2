namespace Application;

public sealed class ReorderPipelineStagesCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ReorderPipelineStagesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderPipelineStagesCommand cmd, CancellationToken ct)
    {
        var ids = cmd.Items.Select(i => i.StageId).ToList();
        var stages = await unitOfWork.Repository<Domain.RecruitmentPipelineStage>()
            .GetAllTrackedAsync(s => s.PipelineId == cmd.PipelineId && ids.Contains(s.Id), ct);

        foreach (var stage in stages)
        {
            var item = cmd.Items.First(i => i.StageId == stage.Id);
            stage.DisplayOrder = item.DisplayOrder;
        }

        // SaveChangesAsync: reorder có thể là no-op nếu thứ tự không đổi
        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
