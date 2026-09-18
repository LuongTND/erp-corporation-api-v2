using Microsoft.EntityFrameworkCore;

namespace Application;

public sealed class UpdateMyProfileCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<UpdateMyProfileCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMyProfileCommand cmd, CancellationToken ct)
    {
        var userId = userContext.UserId;

        var user = await unitOfWork.Repository<User>()
            .Query(tracking: true)
            .Include(u => u.Profile)
            .Include(u => u.Identity)
            .Include(u => u.EmploymentInfo)
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException(ExceptionMessages.NotFound("User", userId));

        // Profile
        if (cmd.Gender != null || cmd.DateOfBirth != null || cmd.PhoneNumber != null ||
            cmd.PermanentAddress != null || cmd.CurrentAddress != null)
        {
            if (user.Profile is null)
            {
                await unitOfWork.Repository<EmployeeProfile>().AddAsync(new EmployeeProfile
                {
                    Id = Guid.NewGuid(), UserId = userId,
                    Gender = cmd.Gender, DateOfBirth = cmd.DateOfBirth,
                    PhoneNumber = cmd.PhoneNumber, PermanentAddress = cmd.PermanentAddress,
                    CurrentAddress = cmd.CurrentAddress,
                });
            }
            else
            {
                if (cmd.Gender != null)           user.Profile.Gender           = cmd.Gender;
                if (cmd.DateOfBirth != null)       user.Profile.DateOfBirth       = cmd.DateOfBirth;
                if (cmd.PhoneNumber != null)       user.Profile.PhoneNumber       = cmd.PhoneNumber;
                if (cmd.PermanentAddress != null)  user.Profile.PermanentAddress  = cmd.PermanentAddress;
                if (cmd.CurrentAddress != null)    user.Profile.CurrentAddress    = cmd.CurrentAddress;
            }
        }

        // Identity
        if (cmd.IdentityCardNumber != null || cmd.IdentityCardIssuedDate != null ||
            cmd.IdentityCardIssuedPlace != null || cmd.PassportNumber != null || cmd.PassportExpiryDate != null)
        {
            if (user.Identity is null)
            {
                await unitOfWork.Repository<EmployeeIdentity>().AddAsync(new EmployeeIdentity
                {
                    Id = Guid.NewGuid(), UserId = userId,
                    IdentityCardNumber = cmd.IdentityCardNumber, IdentityCardIssuedDate = cmd.IdentityCardIssuedDate,
                    IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace, PassportNumber = cmd.PassportNumber,
                    PassportExpiryDate = cmd.PassportExpiryDate,
                });
            }
            else
            {
                if (cmd.IdentityCardNumber != null)      user.Identity.IdentityCardNumber      = cmd.IdentityCardNumber;
                if (cmd.IdentityCardIssuedDate != null)  user.Identity.IdentityCardIssuedDate  = cmd.IdentityCardIssuedDate;
                if (cmd.IdentityCardIssuedPlace != null) user.Identity.IdentityCardIssuedPlace = cmd.IdentityCardIssuedPlace;
                if (cmd.PassportNumber != null)          user.Identity.PassportNumber          = cmd.PassportNumber;
                if (cmd.PassportExpiryDate != null)      user.Identity.PassportExpiryDate      = cmd.PassportExpiryDate;
            }
        }

        // Financial (TaxCode, BHXH, Bank) — nằm trong EmploymentInfo
        if (cmd.TaxCode != null || cmd.SocialInsuranceCode != null ||
            cmd.BankName != null || cmd.BankAccountNumber != null || cmd.BankBranch != null)
        {
            if (user.EmploymentInfo is null)
            {
                await unitOfWork.Repository<EmploymentInfo>().AddAsync(new EmploymentInfo
                {
                    Id = Guid.NewGuid(), UserId = userId,
                    DateOfJoin = DateOnly.FromDateTime(DateTime.UtcNow),
                    TaxCode = cmd.TaxCode, SocialInsuranceCode = cmd.SocialInsuranceCode,
                    BankName = cmd.BankName, BankAccountNumber = cmd.BankAccountNumber, BankBranch = cmd.BankBranch,
                });
            }
            else
            {
                if (cmd.TaxCode != null)             user.EmploymentInfo.TaxCode             = cmd.TaxCode;
                if (cmd.SocialInsuranceCode != null) user.EmploymentInfo.SocialInsuranceCode = cmd.SocialInsuranceCode;
                if (cmd.BankName != null)            user.EmploymentInfo.BankName            = cmd.BankName;
                if (cmd.BankAccountNumber != null)   user.EmploymentInfo.BankAccountNumber   = cmd.BankAccountNumber;
                if (cmd.BankBranch != null)          user.EmploymentInfo.BankBranch          = cmd.BankBranch;
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
