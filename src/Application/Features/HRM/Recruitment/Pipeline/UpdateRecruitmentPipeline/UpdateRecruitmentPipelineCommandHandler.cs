namespace Application;

public sealed class UpdateRecruitmentPipelineCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRecruitmentPipelineCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRecruitmentPipelineCommand cmd, CancellationToken ct)
    {
        var pipeline = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .FindTrackedAsync(p => p.Id == cmd.Id, ct)
            ?? throw new NotFoundException("Quy trình không tồn tại.");

        var duplicate = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .FindAsync(p => p.Name == cmd.Name && p.Id != cmd.Id, ct);
        if (duplicate is not null)
            throw new ConflictException("Tên quy trình đã tồn tại.");

        if (cmd.IsDefault && !pipeline.IsDefault)
        {
            var current = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
                .GetAllTrackedAsync(p => p.IsDefault, ct);
            foreach (var p in current)
                p.IsDefault = false;
        }

        pipeline.Name = cmd.Name;
        pipeline.IsDefault = cmd.IsDefault;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
