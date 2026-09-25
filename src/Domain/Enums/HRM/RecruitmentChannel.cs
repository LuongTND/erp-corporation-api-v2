namespace Domain;

public enum RecruitmentChannel
{
    Facebook = 1,
    LinkedIn = 2,           // Tách riêng mạng xã hội chuyên nghiệp
    Referral = 3,           // Giới thiệu
    PaidBoard = 4,          // Sàn tuyển dụng (TopCV, Vietnamworks...)
    CompanyWebsite = 5,     // Website tuyển dụng riêng của công ty
    Headhunter = 6,         // Đơn vị cung ứng nhân sự thuê ngoài
    WalkIn = 7,             // Nộp trực tiếp/Sự kiện/Tờ rơi
    Internal = 8,           // Tuyển dụng nội bộ
    Other = 10,
}