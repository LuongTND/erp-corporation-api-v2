namespace Application;

public sealed class CreateRecruitmentPipelineCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRecruitmentPipelineCommand, Guid>
{
    public async Task<Guid> Handle(CreateRecruitmentPipelineCommand cmd, CancellationToken ct)
    {
        var exists = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
            .FindAsync(p => p.Name == cmd.Name, ct);
        if (exists is not null)
            throw new ConflictException("Tên quy trình đã tồn tại.");

        // Nếu set default, clear default của pipeline cũ
        if (cmd.IsDefault)
        {
            var current = await unitOfWork.Repository<Domain.RecruitmentPipeline>()
                .GetAllTrackedAsync(p => p.IsDefault, ct);
            foreach (var p in current)
                p.IsDefault = false;
        }

        var pipeline = new Domain.RecruitmentPipeline
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            IsDefault = cmd.IsDefault,
        };

        // Seed 3 fixed stages
        var fixedStages = new[]
        {
            new Domain.RecruitmentPipelineStage { Id = Guid.NewGuid(), PipelineId = pipeline.Id, Name = "Ứng tuyển", DisplayOrder = 1, IsFixed = true },
            new Domain.RecruitmentPipelineStage { Id = Guid.NewGuid(), PipelineId = pipeline.Id, Name = "Offer",     DisplayOrder = 900, IsFixed = true },
            new Domain.RecruitmentPipelineStage { Id = Guid.NewGuid(), PipelineId = pipeline.Id, Name = "Đã tuyển",  DisplayOrder = 999, IsFixed = true },
        };

        await unitOfWork.Repository<Domain.RecruitmentPipeline>().AddAsync(pipeline);
        foreach (var stage in fixedStages)
            await unitOfWork.Repository<Domain.RecruitmentPipelineStage>().AddAsync(stage);

        await unitOfWork.EnsureSaveAsync(ct);
        return pipeline.Id;
    }
}
