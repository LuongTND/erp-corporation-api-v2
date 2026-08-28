namespace Domain;

public enum RecruitmentRequestStatus
{
    Draft = 1,
    PendingLevel1Approval = 2,  // chờ Giám sát vùng / Trưởng BP duyệt
    PendingLevel2Approval = 3,  // chờ Trưởng phòng NS duyệt
    NeedMoreInfo = 4,
    Approved = 5,
    Rejected = 6,
    Cancelled = 7,
}
