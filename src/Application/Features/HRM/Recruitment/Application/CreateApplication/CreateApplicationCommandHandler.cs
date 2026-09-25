namespace Application;

public sealed class CreateApplicationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateApplicationCommand, Guid>
{
    public async Task<Guid> Handle(CreateApplicationCommand cmd, CancellationToken ct)
    {
        var posting = await unitOfWork.Repository<JobPosting>()
            .FindAsync(p => p.Id == cmd.JobPostingId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("JobPosting", cmd.JobPostingId));

        if (posting.Status != JobPostingStatus.Published)
            throw new BadRequestException("Chỉ có thể thêm ứng viên vào tin đang đăng tuyển.");

        var primaryPhone = cmd.Phones.FirstOrDefault();
        var primaryEmail = cmd.Emails.FirstOrDefault();

        // Reuse applicant nếu trùng phone hoặc email chính
        var applicant = await MatchExistingApplicant(primaryPhone, primaryEmail, ct);

        if (applicant == null)
        {
            applicant = new Applicant
            {
                Id = Guid.NewGuid(),
                FullName = cmd.FullName,
                Gender = cmd.Gender,
                DateOfBirth = cmd.DateOfBirth,
                Province = cmd.Province,
                Ward = cmd.Ward,
                Address = cmd.Address,
                Notes = cmd.Notes,
                Phones = cmd.Phones.Select((p, i) => new ApplicantPhone { Id = Guid.NewGuid(), Phone = p, IsPrimary = i == 0 }).ToList(),
                Emails = cmd.Emails.Select((e, i) => new ApplicantEmail { Id = Guid.NewGuid(), Email = e, IsPrimary = i == 0 }).ToList(),
                Educations = cmd.Educations.Select(e => new ApplicantEducation { Id = Guid.NewGuid(), EducationLevel = e.EducationLevel, SchoolName = e.SchoolName, Major = e.Major, GraduationYear = e.GraduationYear }).ToList(),
                Experiences = cmd.Experiences.Select(e => new ApplicantExperience { Id = Guid.NewGuid(), CompanyName = e.CompanyName, RecentWorkplace = e.RecentWorkplace, StartDate = e.StartDate, EndDate = e.EndDate, Position = e.Position, Description = e.Description }).ToList(),
            };
            await unitOfWork.Repository<Applicant>().AddAsync(applicant);
        }

        var application = new Domain.Application
        {
            Id = Guid.NewGuid(),
            ApplicantId = applicant.Id,
            JobPostingId = cmd.JobPostingId,
            SourceChannel = cmd.SourceChannel,
            Stage = ApplicationStage.New,
        };

        await unitOfWork.Repository<Domain.Application>().AddAsync(application);
        await unitOfWork.EnsureSaveAsync(ct);
        return application.Id;
    }

    private async Task<Applicant?> MatchExistingApplicant(string? phone, string? email, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(phone))
        {
            var byPhone = await unitOfWork.Repository<Applicant>()
                .FindAsync(a => a.Phones.Any(p => p.Phone == phone), ct);
            if (byPhone != null) return byPhone;
        }

        if (!string.IsNullOrWhiteSpace(email))
            return await unitOfWork.Repository<Applicant>()
                .FindAsync(a => a.Emails.Any(e => e.Email == email), ct);

        return null;
    }
}
