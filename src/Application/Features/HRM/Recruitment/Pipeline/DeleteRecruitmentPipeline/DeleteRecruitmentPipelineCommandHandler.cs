namespace Application;

public sealed class DeleteRecruitmentPipelineCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRecruitmentPipelineCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRecruitmentPipelineCommand cmd, CancellationToken ct)
    {
        var pipeline = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .FindTrackedAsync(p => p.Id == cmd.Id, ct)
            ?? throw new NotFoundException("Quy trình không tồn tại.");

        if (pipeline.IsDefault)
            throw new BadRequestException("Không thể xóa quy trình mặc định.");

        var stages = await unitOfWork.Repository<Domain.RecruitmentPipelineStage>()
            .GetAllTrackedAsync(s => s.PipelineId == cmd.Id, ct);

        await unitOfWork.Repository<Domain.RecruitmentPipelineStage>().RemoveRangeAsync(stages);
        await unitOfWork.Repository<Domain.RecruitmentPipeline>().RemoveAsync(pipeline);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
