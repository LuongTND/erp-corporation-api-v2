namespace Application;

public sealed class DeleteApplicationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteApplicationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteApplicationCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindTrackedAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        if (application.Stage == ApplicationStage.Hired)
            throw new BadRequestException("Không thể xoá hồ sơ đã được tuyển dụng.");

        await unitOfWork.Repository<Domain.Application>().RemoveAsync(application);
        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
