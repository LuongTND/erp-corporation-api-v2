namespace Domain;

public enum DocumentCategory
{
    // Hồ sơ pháp lý
    IdentityCard,
    HouseholdBook,
    JudicialRecord,
    HealthCertificate,

    // Hồ sơ tuyển dụng
    RecruitmentDecision,
    ProbationContract,
    LaborContract,

    // Bằng cấp / Chứng chỉ
    Degree,
    Certificate,
    DriversLicense,
    FoodSafetyCertificate,

    // Nội bộ
    AppointmentDecision,
    TransferDecision,

    Other,
}
