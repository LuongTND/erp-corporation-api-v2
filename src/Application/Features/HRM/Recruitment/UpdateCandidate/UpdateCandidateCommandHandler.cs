namespace Application;

public sealed class UpdateCandidateCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCandidateCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCandidateCommand cmd, CancellationToken ct)
    {
        var application = await unitOfWork.Repository<Domain.Application>()
            .FindAsync(a => a.Id == cmd.ApplicationId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Application", cmd.ApplicationId));

        var applicant = await unitOfWork.Repository<Applicant>()
            .FindAsync(a => a.Id == application.ApplicantId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("Applicant", application.ApplicantId));

        if (!Enum.TryParse<RecruitmentChannel>(cmd.SourceChannel, ignoreCase: true, out var sourceChannel))
            throw new BadRequestException($"SourceChannel không hợp lệ: {cmd.SourceChannel}");

        applicant.FullName = cmd.FullName;
        applicant.Email = cmd.Email;
        applicant.Phone = cmd.Phone;
        applicant.Notes = cmd.Notes;
        application.SourceChannel = sourceChannel;

        await unitOfWork.EnsureSaveAsync(ct);
        return Unit.Value;
    }
}
