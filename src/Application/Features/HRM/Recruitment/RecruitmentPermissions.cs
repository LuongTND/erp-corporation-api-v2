namespace Application;

public static class RecruitmentPermissions
{
    // Approver config (existing)
    public const string ViewApproverConfig   = "hrm:recruitment:approver-config:view";
    public const string ManageApproverConfig = "hrm:recruitment:approver-config:manage";

    // Recruitment request
    public const string ViewRequest      = "hrm:recruitment:request:view";
    public const string CreateRequest    = "hrm:recruitment:request:create";
    public const string UpdateRequest    = "hrm:recruitment:request:update";
    public const string SubmitRequest    = "hrm:recruitment:request:submit";
    public const string ApproveRequestLevel1 = "hrm:recruitment:request:approve-level1"; // Giám sát vùng / Trưởng BP
    public const string ApproveRequest      = "hrm:recruitment:request:approve";          // Trưởng phòng NS
    public const string RejectRequest       = "hrm:recruitment:request:reject";
    public const string RequestMoreInfo     = "hrm:recruitment:request:more-info";
    public const string ViewHistory      = "hrm:recruitment:request:history";

    // Candidate
    public const string ViewCandidate     = "hrm:recruitment:candidate:view";
    public const string CreateCandidate   = "hrm:recruitment:candidate:create";
    public const string UpdateCandidate   = "hrm:recruitment:candidate:update";
    public const string UploadCv          = "hrm:recruitment:candidate:upload-cv";
    public const string ScreenCandidate   = "hrm:recruitment:candidate:screen";
    public const string AssignCandidate   = "hrm:recruitment:candidate:assign";
    public const string EvaluateCandidate = "hrm:recruitment:candidate:evaluate";
    public const string RejectCandidate   = "hrm:recruitment:candidate:reject";
    public const string HireCandidate     = "hrm:recruitment:candidate:hire";

    // Job posting
    public const string ManageJobPosting   = "hrm:recruitment:posting:manage";
    public const string CreatePaidPosting  = "hrm:recruitment:posting:paid-create";
    public const string ApprovePaidPosting = "hrm:recruitment:posting:paid-approve";

    // Interview rule config (admin)
    public const string ManageInterviewRule = "hrm:recruitment:interview-rule:manage";

    // Interview schedule
    public const string ManageInterviewSchedule  = "hrm:recruitment:interview-schedule:manage";
    public const string CompleteInterviewSchedule = "hrm:recruitment:interview-schedule:complete";
}
