# Entity Design — HRM & LMS
> Source: SRS_HRM_BaHungBakery.docx + SRS_LMS_BaHungBakery.docx v1.0 (23/07/2026)  
> Stack: .NET 10 · EF Core · Clean Architecture  
> Convention: tất cả entity kế thừa `BaseEntity` (Id, CreatedAt, UpdatedAt, DeletedAt?)

---

## Legend

```
[x] entity đã có trong codebase
[ ] chưa có — cần tạo
[~] có nhưng thiếu field
[!] blocker — entity khác đang FK vào đây
```

---

## 0. Base & Cross-cutting (đã có, liệt kê để tham chiếu)

```
[x] User            — auth, profile cơ bản, Role, Status
[x] Department      — [~] thiếu: ParentId (cây đa cấp), OrganizationId, CostCenter, ManagerId
[x] JobLevel        — cấp bậc, ScopeType
[x] Permission      — RBAC
[x] Notification*   — entity đã có, service chưa impl
[x] Chat*           — Conversation, Message entity đã có
[x] Task*           — TaskActivityLog entity đã có
```

---

## 1. HRM — Quản lý nhân sự

### 1.1 Cơ cấu tổ chức

#### `[ ]` Organization
```csharp
Guid        Id
Guid?       ParentId           // FK → Organization (cây: Công ty → Khối → Chi nhánh → Cửa hàng)
string      Name
string      Code               // unique
OrgType     Type               // Company | Branch | Store
string?     CostCenter
string?     Address
string?     TaxCode
bool        IsActive
```

#### `[~]` Department  *(bổ sung thêm field)*
```csharp
// existing fields giữ nguyên, thêm:
Guid?       OrganizationId     // FK → Organization
Guid?       ParentId           // FK → Department (cây đa cấp)
string?     CostCenter
Guid?       ManagerId          // FK → User
```

#### `[ ]` Position
```csharp
Guid        Id
Guid        DepartmentId       // FK → Department
Guid        JobLevelId         // FK → JobLevel
string      Title
string      Code               // unique
Guid?       DirectManagerPositionId  // FK → Position
string?     CostCenter
bool        IsActive
```

---

### 1.2 Hồ sơ nhân viên

#### `[ ]` EmployeeProfile  *(1-1 với User)*
```csharp
Guid        Id
Guid        UserId             // FK → User (unique)
string      EmployeeCode       // unique toàn hệ thống
string?     NationalId         // CCCD/CMND
DateOnly?   NationalIdIssuedDate
string?     NationalIdIssuedPlace
string?     HomeTown
string?     PermanentAddress
string?     TemporaryAddress
string?     EmergencyContactName
string?     EmergencyContactPhone
string?     EmergencyContactRelation
string?     TaxCode            // mã số thuế TNCN
string?     SocialInsuranceCode
string?     BankAccountNumber
string?     BankName
DateOnly    HireDate
Guid        PositionId         // FK → Position
ContractType ContractType
Guid?       ManagerId          // FK → User
EmployeeStatus Status          // Active | Inactive | Terminated
DateOnly?   TerminatedAt
```

#### `[ ]` EmployeeEducation
```csharp
Guid        Id
Guid        EmployeeId         // FK → EmployeeProfile
string      Degree             // Đại học, Thạc sĩ, Cao đẳng...
string      Institution
string?     Major
int?        GraduatedYear
string?     CertificateName
```

#### `[ ]` EmployeeSkill
```csharp
Guid        Id
Guid        EmployeeId
string      SkillName
int         ProficiencyLevel   // 1-5
SkillType   Type               // Technical | Language | Soft
```

---

### 1.3 Chấm công & Ca làm việc

#### `[ ]` WorkShift
```csharp
Guid        Id
Guid        OrganizationId
string      Name
ShiftType   Type               // Fixed | Rotating | Split
TimeOnly    StartTime
TimeOnly    EndTime
int         BreakMinutes
int         OvertimeThresholdMinutes
int         LateToleranceMinutes   // phút dung sai đi muộn
bool        IsActive
```

#### `[ ]` ShiftAssignment
```csharp
Guid        Id
Guid        EmployeeId
Guid        ShiftId
DateOnly    Date
Guid        OrganizationId
Guid        AssignedByUserId
```

#### `[ ]` AttendanceRecord
```csharp
Guid        Id
Guid        EmployeeId
Guid?       ShiftAssignmentId
DateOnly    Date
DateTime?   CheckInTime
DateTime?   CheckOutTime
AttendanceMethod CheckInMethod  // Fingerprint | Face | GPS | Web
decimal?    CheckInLatitude
decimal?    CheckInLongitude
string?     CheckInDeviceId
string?     CheckInIpAddress
AttendanceStatus Status         // Present | Late | EarlyLeave | Absent | Leave | Holiday
int?        WorkMinutes
int?        OvertimeMinutes
```

#### `[ ]` AttendanceAdjustment  *(bổ sung công khi thiết bị lỗi)*
```csharp
Guid        Id
Guid        EmployeeId
Guid?       AttendanceRecordId
DateOnly    Date
DateTime?   AdjustedCheckIn
DateTime?   AdjustedCheckOut
string      Reason
string?     EvidenceFileUrl
ApprovalStatus Status
Guid?       ApprovedByUserId
DateTime?   ApprovedAt
```

#### `[ ]` MonthlyAttendanceSummary  *(bảng công tổng hợp — có thể lock)*
```csharp
Guid        Id
Guid        EmployeeId
int         PeriodYear
int         PeriodMonth
decimal     StandardWorkDays
decimal     ActualWorkDays
decimal     PaidLeaveDays
decimal     UnpaidLeaveDays
decimal     AbsentDays
decimal     OvertimeHours
int         LateCount
int         EarlyLeaveCount
bool        IsLocked
Guid?       LockedByUserId
DateTime?   LockedAt
```

---

### 1.4 Nghỉ phép & OT

#### `[ ]` LeaveType
```csharp
Guid        Id
string      Name               // Phép năm, Nghỉ ốm, Thai sản, Kết hôn...
string      Code               // ANNUAL | SICK | MATERNITY | MARRIAGE | MOURNING | UNPAID
bool        IsPaid
bool        RequiresMedicalCertificate
decimal?    AnnualAllowanceDays
bool        IsSystemType       // loại hệ thống không xóa được
string?     Description
```

#### `[ ]` LeaveBalance
```csharp
Guid        Id
Guid        EmployeeId
Guid        LeaveTypeId
int         Year
decimal     AllocatedDays
decimal     UsedDays
decimal     CarryOverDays
// RemainingDays = AllocatedDays + CarryOverDays - UsedDays (computed)
```

#### `[!]` LeaveRequest  *(referenced by CHECKLIST as blocker)*
```csharp
Guid        Id
Guid        EmployeeId
Guid        LeaveTypeId
DateOnly    StartDate
DateOnly    EndDate
decimal     Days
string?     Reason
string?     EvidenceFileUrl
ApprovalStatus Status          // Pending | Approved | Rejected | Cancelled
Guid?       ApprovalRequestId  // FK → ApprovalRequest (workflow engine)
```

#### `[ ]` OvertimeRequest
```csharp
Guid        Id
Guid        EmployeeId
DateOnly    Date
TimeOnly    StartTime
TimeOnly    EndTime
decimal     PlannedHours
decimal     OvertimeRate       // 1.5 | 2.0 | 3.0
string      Reason
ApprovalStatus Status
Guid?       ApprovalRequestId
int?        ActualOvertimeMinutes  // ghi nhận sau khi thực hiện
```

---

### 1.5 Lương (Payroll)

#### `[ ]` SalaryConfig  *(công thức lương — cấu hình được)*
```csharp
Guid        Id
string      Name
Guid?       PositionId
Guid?       OrganizationId
string      FormulaJson        // JSON: các thành phần lương, hệ số
decimal     BaseAmount
string      AllowanceItemsJson // JSON: các loại phụ cấp
bool        IsActive
DateOnly    EffectiveFrom
DateOnly?   EffectiveTo
```

#### `[!]` PayrollRun  *(referenced by CHECKLIST as blocker)*
```csharp
Guid        Id
int         PeriodYear
int         PeriodMonth
Guid        OrganizationId
PayrollStatus Status           // Draft | Reviewing | Approved | Locked
Guid?       PreparedByUserId
Guid?       ApprovedByUserId
DateTime?   ApprovedAt
DateTime?   LockedAt
decimal     TotalGross
decimal     TotalNet
decimal     TotalEmployerSocialInsurance
decimal     TotalPersonalIncomeTax
```

#### `[!]` PayrollItem  *(referenced by CHECKLIST as blocker)*
```csharp
Guid        Id
Guid        PayrollRunId
Guid        EmployeeId
decimal     BasicSalary
decimal     AllowanceAmount
decimal     KpiBonus
decimal     OvertimePay
decimal     SalaryAdvanceDeduction
decimal     EmployeeSocialInsurance    // BHXH+BHYT+BHTN nhân viên đóng
decimal     EmployerSocialInsurance    // phần công ty đóng
decimal     PersonalIncomeTax          // TNCN tính theo biểu lũy tiến
decimal     OtherDeductions
decimal     NetSalary
string?     PayslipUrl
string?     PayslipPdfPath
```

#### `[ ]` SalaryAdvance  *(tạm ứng lương)*
```csharp
Guid        Id
Guid        EmployeeId
decimal     Amount
string      Reason
DateTime    RequestedAt
ApprovalStatus Status
Guid?       ApprovedByUserId
Guid?       DeductedInPayrollRunId     // FK → PayrollRun (kỳ đã trừ)
```

---

### 1.6 KPI & Đánh giá hiệu suất

#### `[!]` KpiIndicator  *(build blocker)*
```csharp
Guid        Id
string      Name
string      Code               // unique
string      Unit               // %, VND, số lượng, điểm, tỷ lệ
KpiType     Type               // Manual | AutoCRM | AutoPOS | AutoContactCenter
string?     DataSourceField    // field mapping khi auto-pull
bool        IsHigherBetter     // true: càng cao càng tốt (doanh số); false: càng thấp (tỷ lệ lỗi)
bool        IsActive
```

#### `[ ]` KpiTemplate  *(bộ chỉ tiêu mẫu theo vị trí)*
```csharp
Guid        Id
string      Name
Guid?       PositionId
Guid?       OrganizationId
bool        IsActive
```

#### `[ ]` KpiTemplateIndicator
```csharp
Guid        Id
Guid        TemplateId         // FK → KpiTemplate
Guid        IndicatorId        // FK → KpiIndicator
decimal     Weight             // 0-1, tổng toàn template = 1
decimal?    DefaultTargetValue
```

#### `[!]` KpiTarget  *(build blocker)*
```csharp
Guid        Id
Guid        EmployeeId
Guid?       TemplateId
Guid        IndicatorId
int         PeriodYear
int?        PeriodMonth
int?        PeriodQuarter
decimal     TargetValue
Guid        AssignedByUserId
DateTime    AssignedAt
```

#### `[ ]` KpiActual  *(kết quả thực tế)*
```csharp
Guid        Id
Guid        KpiTargetId
decimal     ActualValue
// AchievementRate = ActualValue / TargetValue * 100 (computed)
DateTime    UpdatedAt
Guid?       UpdatedByUserId    // null = auto-pulled
string?     DataSource         // CRM | POS | ContactCenter | Manual
```

#### `[ ]` PerformanceReview
```csharp
Guid        Id
Guid        EmployeeId
int         PeriodYear
int?        PeriodMonth
int?        PeriodQuarter
decimal?    SelfScore
string?     SelfComment
DateTime?   SelfReviewedAt
decimal?    ManagerScore
string?     ManagerComment
Guid?       ReviewerManagerId
DateTime?   ManagerReviewedAt
decimal?    HrFinalScore
PerformanceRating? HrRating   // Excellent | Good | Average | BelowAverage | Poor
ReviewStatus Status            // Draft | SelfReviewed | ManagerReviewed | Finalized
DateTime?   FinalizedAt
```

---

### 1.7 Tuyển dụng

#### `[ ]` RecruitmentRequest
```csharp
Guid        Id
Guid        DepartmentId
Guid        PositionId
Guid        RequestedByUserId
int         Headcount
decimal?    ExpectedSalaryFrom
decimal?    ExpectedSalaryTo
string      JobDescription
DateOnly?   RequiredByDate
RecruitmentChannel[] PreferredChannels  // LinkedIn | JobStreet | Referral | Internal
ApprovalStatus Status
Guid?       ApprovalRequestId
```

#### `[ ]` Candidate
```csharp
Guid        Id
Guid?       RecruitmentRequestId
string      FullName
string      Email
string?     Phone
string?     CvUrl
string      SourceChannel      // LinkedIn | JobStreet | Referral | Walk-in | Internal
CandidateStage Stage           // CV | Screening | Interview1 | Interview2 | Offer | Hired | Rejected
string?     RejectionReason
Guid?       ConvertedEmployeeId // FK → EmployeeProfile (sau khi trúng tuyển)
string?     Notes
```

#### `[ ]` InterviewSchedule
```csharp
Guid        Id
Guid        CandidateId
int         Round
DateTime    ScheduledAt
string      Location
string      InterviewerUserIdsJson  // JSON array
string?     Notes
InterviewFormat Format         // InPerson | Video | Phone
```

#### `[ ]` InterviewEvaluation
```csharp
Guid        Id
Guid        InterviewScheduleId
Guid        InterviewerUserId
decimal     Score              // 0-10
string?     StrengthNotes
string?     WeaknessNotes
InterviewRecommendation Recommendation  // Hire | Reject | NextRound
```

#### `[ ]` JobOffer
```csharp
Guid        Id
Guid        CandidateId
Guid        PositionId
decimal     OfferedSalary
DateOnly    ProposedStartDate
OfferStatus Status             // Draft | Sent | Accepted | Rejected | Expired
DateTime?   SentAt
DateTime?   ExpiresAt
string?     OfferLetterUrl
```

---

### 1.8 Onboarding / Offboarding

#### `[ ]` OnboardingChecklist  *(template)*
```csharp
Guid        Id
Guid?       PositionId         // null = áp dụng cho mọi vị trí
string      Name
bool        IsDefault
```

#### `[ ]` OnboardingTaskTemplate
```csharp
Guid        Id
Guid        ChecklistId
string      Title
string?     Description
string      AssigneeRole       // HR | IT | Manager | Employee
int         DueDaysFromHire
bool        IsLinkedToLMS
Guid?       LmsCourseId        // FK → Course (cross-module)
int         Order
```

#### `[ ]` EmployeeOnboarding  *(instance cho từng nhân viên)*
```csharp
Guid        Id
Guid        EmployeeId
Guid        ChecklistId
DateOnly    StartDate
OnboardingStatus Status
```

#### `[ ]` EmployeeOnboardingTask
```csharp
Guid        Id
Guid        EmployeeOnboardingId
Guid        OnboardingTaskTemplateId
Guid?       AssignedToUserId
TaskCompletionStatus Status    // Todo | InProgress | Done
DateTime?   CompletedAt
string?     Notes
```

#### `[ ]` EmployeeOffboarding
```csharp
Guid        Id
Guid        EmployeeId
DateOnly    ResignationDate
DateOnly?   LastWorkingDate
OffboardingReason Reason       // Resignation | Termination | Retirement | Transfer
string?     ExitInterviewNotes
OffboardingStatus Status
```

#### `[ ]` OffboardingTask
```csharp
Guid        Id
Guid        EmployeeOffboardingId
string      Title
string      AssigneeRole
TaskCompletionStatus Status
DateTime?   CompletedAt
string?     Notes
```

---

### 1.9 Hợp đồng & Hồ sơ

#### `[!]` EmploymentContract  *(build blocker)*
```csharp
Guid        Id
Guid        EmployeeId
string      ContractNumber
ContractType Type              // Probation | FixedTerm | Indefinite | PartTime
DateOnly    StartDate
DateOnly?   EndDate
decimal     Salary
string      PositionTitle
string?     FileUrl
ContractStatus Status          // Active | Expired | Terminated | Renewed
DateOnly?   SignedDate
Guid?       RenewedFromContractId  // FK → EmploymentContract
```

#### `[ ]` ContractAmendment  *(phụ lục hợp đồng)*
```csharp
Guid        Id
Guid        ContractId
string      AmendmentNumber
DateOnly    EffectiveDate
string      ChangesJson        // JSON: { field, oldValue, newValue }
string      Reason
string?     FileUrl
```

#### `[ ]` LegalDocument
```csharp
Guid        Id
Guid        EmployeeId
LegalDocType Type              // HealthCertificate | NDA | AppointmentDecision | TransferDecision | DisciplinaryDecision
string      Title
string      FileUrl
DateOnly?   IssuedDate
DateOnly?   ExpiresAt
```

---

### 1.10 Khen thưởng & Kỷ luật

#### `[ ]` RewardRecord
```csharp
Guid        Id
Guid        EmployeeId
RewardType  Type               // BonusMoney | Commendation | Certificate | Other
string      Title
decimal?    Amount
string      Reason
DateOnly    IssuedDate
Guid        IssuedByUserId
ApprovalStatus Status
Guid?       ApprovalRequestId
```

#### `[ ]` DisciplinaryRecord
```csharp
Guid        Id
Guid        EmployeeId
DisciplinaryType Type          // Warning | Reprimand | Suspension | Termination
string      Title
string      Reason
DateOnly    IssuedDate
DateOnly    EffectiveDate
DateOnly?   EndDate
Guid        IssuedByUserId
ApprovalStatus Status
Guid?       ApprovalRequestId
```

---

### 1.11 Tài sản cấp phát

#### `[ ]` AssetAllocation
```csharp
Guid        Id
Guid        EmployeeId
Guid        AssetId            // FK → Asset module (ERP lõi)
string      AssetName          // cache tên tài sản
string?     AssetCode
DateOnly    AllocatedAt
DateOnly?   RecalledAt
AssetCondition Condition       // Good | Damaged | Lost
string?     Notes
```

---

### 1.12 Engine phê duyệt (dùng chung toàn HRM)

#### `[ ]` ApprovalWorkflowConfig
```csharp
Guid        Id
string      Name
ApprovalRequestType RequestType  // LeaveRequest | OvertimeRequest | RecruitmentRequest | PayrollApproval | ...
int         StepCount
string      ConditionsJson     // JSON: điều kiện rẽ nhánh (vd: nghỉ > 3 ngày → thêm cấp HR)
bool        IsActive
Guid        OrganizationId
```

#### `[ ]` ApprovalRequest
```csharp
Guid        Id
Guid        WorkflowConfigId
ApprovalRequestType RequestType
Guid        RequesterId
Guid        ReferenceId        // ID của entity gốc (LeaveRequest.Id, OvertimeRequest.Id...)
string      Title
int         CurrentStep
ApprovalStatus Status          // Pending | Approved | Rejected | Cancelled
DateTime    CreatedAt
DateTime?   CompletedAt
```

#### `[ ]` ApprovalStep
```csharp
Guid        Id
Guid        ApprovalRequestId
int         StepNumber
Guid        ApproverId
StepStatus  Status             // Pending | Approved | Rejected | Skipped
string?     Comment
DateTime?   ProcessedAt
DateTime?   DueAt
```

#### `[ ]` ApprovalEscalation
```csharp
Guid        Id
Guid        ApprovalStepId
Guid        EscalatedToUserId
DateTime    EscalatedAt
string      Reason
```

---

## 2. LMS — Học tập trực tuyến

### 2.1 Khóa học & Nội dung

#### `[!]` Course  *(build blocker)*
```csharp
Guid        Id
Guid        InstructorId       // FK → User
Guid?       CategoryId         // FK → CourseCategory
string      Title
string      Description
string?     ThumbnailUrl
CourseLevel Level              // Beginner | Intermediate | Advanced
decimal     Price              // 0 = miễn phí
bool        IsFree
CourseStatus Status            // Draft | PendingReview | Published | Archived
bool        HasFinalExam
int?        PassingScore       // % điểm đạt bài thi cuối
bool        IsSequentialLearning  // true: học tuần tự; false: tự do
int?        TotalLessons       // cache, tính lại khi có thay đổi
decimal     AverageRating      // cache
int         EnrollmentCount    // cache
```

#### `[ ]` CourseCategory
```csharp
Guid        Id
string      Name
string      Slug               // unique
Guid?       ParentId           // FK → CourseCategory
int         Order
bool        IsActive
```

#### `[!]` CourseSection  *(build blocker)*
```csharp
Guid        Id
Guid        CourseId
string      Title
int         Order
bool        IsPublished
```

#### `[!]` Lesson  *(build blocker)*
```csharp
Guid        Id
Guid        SectionId
string      Title
LessonType  Type               // Video | PDF | Text | Quiz
int         Order
bool        IsFreePreview      // xem trước khi mua
bool        IsRequired         // tính vào % hoàn thành không
int?        VideoDurationSeconds
string?     VideoUrl           // URL CDN (protected)
string?     PdfUrl             // URL CDN (protected)
string?     TextContent        // cho loại Text
```

#### `[ ]` CourseReviewRequest  *(quy trình duyệt xuất bản)*
```csharp
Guid        Id
Guid        CourseId
Guid        RequestedByUserId
ReviewRequestStatus Status     // Pending | Approved | Rejected
string?     ReviewNote
Guid?       ReviewedByUserId
DateTime    RequestedAt
DateTime?   ReviewedAt
```

---

### 2.2 Mua & Kích hoạt

#### `[ ]` Order
```csharp
Guid        Id
Guid        UserId
decimal     TotalAmount
decimal     DiscountAmount
decimal     FinalAmount
OrderStatus Status             // Pending | Paid | Failed | Cancelled | Refunded
PaymentMethod PaymentMethod    // VNPAY | MoMo | ZaloPay | Visa | BankTransfer
string?     GatewayOrderId     // mã phía cổng thanh toán
Guid?       VoucherId
DateTime?   PaidAt
string?     InvoiceUrl
```

#### `[ ]` OrderItem
```csharp
Guid        Id
Guid        OrderId
Guid        CourseId
string      CourseTitle        // cache snapshot tại thời điểm mua
decimal     UnitPrice
decimal     DiscountAmount
decimal     FinalPrice
```

#### `[ ]` Transaction
```csharp
Guid        Id
Guid        OrderId
string      GatewayTransactionId
PaymentMethod PaymentMethod
decimal     Amount
TransactionStatus Status       // Pending | Success | Failed | Refunded
string?     GatewayResponseJson
DateTime    CreatedAt
```

#### `[ ]` ActivationCode
```csharp
Guid        Id
Guid        CourseId
string      Code               // unique, generated random
Guid?       BatchId            // nhóm code tạo cùng lúc
DateTime?   ExpiresAt
DateTime?   UsedAt
Guid?       UsedByUserId
Guid        CreatedByUserId
```

#### `[!]` CourseEnrollment  *(quyền truy cập khóa học)*
```csharp
Guid        Id
Guid        UserId
Guid        CourseId
EnrollmentStatus Status        // Active | Expired | Revoked
DateTime    EnrolledAt
DateTime?   ExpiresAt
DateTime?   RevokedAt
string?     RevokeReason
Guid?       ActivationCodeId
Guid?       OrderItemId
Guid?       GrantedByUserId    // Admin cấp trực tiếp cho nội bộ
```

---

### 2.3 Học tập & Tiến độ

#### `[ ]` LearningSession  *(mỗi lần ngồi học)*
```csharp
Guid        Id
Guid        UserId
Guid        LessonId
DateTime    StartedAt
DateTime?   EndedAt
string?     DeviceInfo
string      IpAddress
```

#### `[ ]` VideoProgress  *(ghi nhớ vị trí dừng)*
```csharp
Guid        Id
Guid        UserId
Guid        LessonId
int         LastPositionSeconds
int         WatchedPercent     // 0-100
DateTime    UpdatedAt
// Unique constraint: (UserId, LessonId)
```

#### `[!]` LessonCompletion  *(referenced in CHECKLIST)*
```csharp
Guid        Id
Guid        EnrollmentId
Guid        LessonId
DateTime    CompletedAt
int?        WatchedPercent     // 80%+ video → completed
```

#### `[ ]` CourseProgress  *(cache tổng hợp)*
```csharp
Guid        Id
Guid        EnrollmentId
int         CompletionPercent  // 0-100
Guid?       LastLessonId
DateTime    UpdatedAt
// Unique constraint: (EnrollmentId)
```

#### `[ ]` LessonNote  *(ghi chú tại timestamp video)*
```csharp
Guid        Id
Guid        UserId
Guid        LessonId
string      Content
int?        VideoTimestampSeconds
DateTime    CreatedAt
DateTime?   UpdatedAt
```

---

### 2.4 Quiz & Bài thi

#### `[ ]` QuestionBank
```csharp
Guid        Id
Guid        CourseId
Guid        InstructorId
string      Name
int         TotalQuestions     // cache
```

#### `[ ]` Question
```csharp
Guid        Id
Guid        BankId
string      Content
QuestionType Type              // SingleChoice | MultipleChoice | TrueFalse
string?     Explanation
bool        IsActive
```

#### `[ ]` QuestionOption
```csharp
Guid        Id
Guid        QuestionId
string      Content
bool        IsCorrect
int         Order
```

#### `[ ]` QuizConfig
```csharp
Guid        Id
Guid?       LessonId           // gắn vào bài học cụ thể (quiz giữa bài)
Guid?       CourseId           // gắn vào khóa học (bài thi cuối)
Guid        BankId
int         QuestionCount      // số câu lấy ngẫu nhiên từ bank
int?        TimeLimitMinutes   // null = không giới hạn thời gian
int         PassingScore       // % điểm đạt
int?        MaxAttempts        // null = không giới hạn
bool        ShowAnswerAfterSubmit
bool        RandomizeQuestions
QuizType    Type               // MidLesson | Final
```

#### `[ ]` ExamAttempt
```csharp
Guid        Id
Guid        QuizConfigId
Guid        UserId
DateTime    StartedAt
DateTime?   SubmittedAt
int?        Score              // %
bool?       IsPassed
string      QuestionsSnapshotJson  // snapshot đề bài tại thời điểm làm
bool        HasAbnormalEvent   // phát hiện chuyển tab
```

#### `[ ]` ExamAnswer
```csharp
Guid        Id
Guid        AttemptId
Guid        QuestionId
string      SelectedOptionIdsJson  // JSON array of Guid
bool        IsCorrect
```

---

### 2.5 Chứng chỉ

#### `[ ]` CertificateTemplate
```csharp
Guid        Id
Guid?       CourseId           // null = template mặc định
string      Name
string?     LogoUrl
string?     SignatureUrl
string?     SealUrl
string      PrimaryColor       // hex
bool        IsDefault
```

#### `[ ]` Certificate
```csharp
Guid        Id
Guid        UserId
Guid        CourseId
Guid        EnrollmentId
string      CertificateCode    // unique, for QR — format: CERT-{YYYYMM}-{RANDOM8}
Guid        TemplateId
DateTime    IssuedAt
CertificateStatus Status       // Valid | Revoked
DateTime?   RevokedAt
string?     RevokeReason
string?     PdfUrl
// QR trỏ tới: /certificates/verify/{CertificateCode}
```

---

### 2.6 Tương tác

#### `[ ]` LessonComment
```csharp
Guid        Id
Guid        LessonId
Guid        UserId
Guid?       ParentCommentId    // FK → LessonComment (thread)
string      Content
int?        VideoTimestampSeconds  // câu hỏi tại giây nào của video
int         UpvoteCount        // cache
bool        IsHidden           // Admin ẩn
DateTime    CreatedAt
DateTime?   UpdatedAt
```

#### `[ ]` CommentUpvote
```csharp
Guid        Id
Guid        CommentId
Guid        UserId
DateTime    CreatedAt
// Unique constraint: (CommentId, UserId)
```

#### `[ ]` CourseRating
```csharp
Guid        Id
Guid        CourseId
Guid        UserId
int         Rating             // 1-5
string?     Comment
bool        IsHidden
DateTime    CreatedAt
// Unique constraint: (CourseId, UserId)
// Chỉ cho phép sau khi CourseProgress.CompletionPercent >= 30
```

#### `[ ]` InstructorRatingReply
```csharp
Guid        Id
Guid        CourseRatingId
Guid        InstructorId
string      Content
DateTime    CreatedAt
// Max 1 reply per rating
```

---

### 2.7 Gamification & Nhắc nhở

#### `[ ]` LearningStreak
```csharp
Guid        Id
Guid        UserId
int         CurrentStreak      // số ngày liên tiếp hiện tại
int         LongestStreak
DateOnly    LastActiveDate
// Unique constraint: (UserId)
```

#### `[ ]` LearnerBadge
```csharp
Guid        Id
Guid        UserId
BadgeType   Type               // Streak7 | Streak30 | Streak100 | FirstCourse | ...
DateTime    EarnedAt
```

#### `[ ]` XpTransaction
```csharp
Guid        Id
Guid        UserId
int         Amount
XpReason    Reason             // LessonCompleted | QuizPassed | CertificateEarned | StreakMilestone
Guid?       ReferenceId        // LessonId hoặc CertificateId liên quan
DateTime    CreatedAt
```

#### `[ ]` ReminderConfig
```csharp
Guid        Id
Guid        UserId
Guid        CourseId
bool        IsEnabled
int?        ReminderHour       // 0-23
string?     ReminderDaysJson   // JSON array: [1,2,3,4,5] (1=Thứ 2)
int         InactiveDaysThreshold  // mặc định 3
```

---

### 2.8 Bảo vệ nội dung

#### `[ ]` TrustedDevice
```csharp
Guid        Id
Guid        UserId
string      DeviceFingerprint  // unique per user
string      DeviceName
string      Browser
DateTime    RegisteredAt
DateTime    LastUsedAt
bool        IsActive
```

#### `[ ]` ActiveSession
```csharp
Guid        Id
Guid        UserId
string      SessionToken       // JWT jti
string      DeviceFingerprint
string      IpAddress
DateTime    CreatedAt
DateTime    ExpiresAt
bool        IsRevoked
// chỉ 1 row active per user
```

#### `[ ]` SecurityLog
```csharp
Guid        Id
Guid        UserId
string      IpAddress
string      DeviceInfo
string?     Browser
string?     Timezone
string?     Country
SecurityEvent Event            // Login | Logout | NewDevice | SessionKicked | SuspiciousIp
DateTime    CreatedAt
```

#### `[ ]` LoginHistory
```csharp
Guid        Id
Guid        UserId
string      IpAddress
string?     DeviceInfo
string?     Browser
DateTime    LoginAt
bool        IsOAuth
string?     OAuthProvider      // Google | Facebook
bool        LoginSuccess
string?     FailReason
```

---

### 2.9 Voucher & Thanh toán

#### `[ ]` Voucher
```csharp
Guid        Id
string      Code               // unique, uppercase
VoucherType Type               // FixedAmount | Percentage
decimal     Value              // số tiền hoặc %
decimal?    MaxDiscountAmount  // cap cho loại %
VoucherScope Scope             // AllCourses | SpecificCourse | Category
Guid?       CourseId
Guid?       CategoryId
int?        MaxUsageCount      // null = không giới hạn
int         MaxUsagePerUser    // mặc định 1
decimal?    MinOrderAmount
int         UsedCount          // cache
DateTime    StartsAt
DateTime?   ExpiresAt
bool        IsActive
Guid?       BatchId
```

#### `[ ]` VoucherUsage
```csharp
Guid        Id
Guid        VoucherId
Guid        UserId
Guid        OrderId
decimal     DiscountAmount
DateTime    UsedAt
```

---

### 2.10 Hoàn tiền

#### `[ ]` RefundPolicy  *(cấu hình được — singleton hoặc per-course)*
```csharp
Guid        Id
Guid?       CourseId           // null = policy mặc định toàn hệ thống
int         MaxDaysAfterPurchase   // mặc định 7
int         MaxCompletionPercent   // mặc định 30
bool        IsActive
```

#### `[ ]` RefundRequest
```csharp
Guid        Id
Guid        UserId
Guid        OrderId
Guid        CourseId
string      Reason
int         CompletionPercentAtRequest  // snapshot % đã học khi gửi
RefundStatus Status            // Pending | Approved | Rejected
Guid?       ReviewedByUserId
string?     ReviewNote
DateTime    RequestedAt
DateTime?   ReviewedAt
decimal?    RefundedAmount
string?     GatewayRefundTransactionId
// Sau approve: auto revoke Enrollment + Certificate
```

---

## 3. Enums cần định nghĩa

### HRM
```csharp
enum OrgType               { Company, Branch, Store }
enum ContractType          { Probation, FixedTerm, Indefinite, PartTime }
enum ContractStatus        { Active, Expired, Terminated, Renewed }
enum EmployeeStatus        { Active, Inactive, Terminated }
enum ShiftType             { Fixed, Rotating, Split }
enum AttendanceMethod      { Fingerprint, Face, GPS, Web }
enum AttendanceStatus      { Present, Late, EarlyLeave, Absent, Leave, Holiday }
enum ApprovalStatus        { Pending, Approved, Rejected, Cancelled }
enum ApprovalRequestType   { LeaveRequest, OvertimeRequest, RecruitmentRequest, Reward, Disciplinary, SalaryAdvance, Payroll }
enum StepStatus            { Pending, Approved, Rejected, Skipped }
enum PayrollStatus         { Draft, Reviewing, Approved, Locked }
enum KpiType               { Manual, AutoCRM, AutoPOS, AutoContactCenter }
enum PerformanceRating     { Excellent, Good, Average, BelowAverage, Poor }
enum ReviewStatus          { Draft, SelfReviewed, ManagerReviewed, Finalized }
enum CandidateStage        { CV, Screening, Interview1, Interview2, Offer, Hired, Rejected }
enum InterviewRecommendation { Hire, Reject, NextRound }
enum OfferStatus           { Draft, Sent, Accepted, Rejected, Expired }
enum OnboardingStatus      { InProgress, Completed }
enum OffboardingReason     { Resignation, Termination, Retirement, Transfer, EndOfContract }
enum LegalDocType          { HealthCertificate, NDA, AppointmentDecision, TransferDecision, DisciplinaryDecision }
enum RewardType            { BonusMoney, Commendation, Certificate, Other }
enum DisciplinaryType      { Warning, Reprimand, Suspension, Termination }
enum AssetCondition        { Good, Damaged, Lost }
enum SkillType             { Technical, Language, Soft }
enum TaskCompletionStatus  { Todo, InProgress, Done }
enum InterviewFormat       { InPerson, Video, Phone }
```

### LMS
```csharp
enum CourseLevel           { Beginner, Intermediate, Advanced }
enum CourseStatus          { Draft, PendingReview, Published, Archived }
enum LessonType            { Video, PDF, Text, Quiz }
enum EnrollmentStatus      { Active, Expired, Revoked }
enum OrderStatus           { Pending, Paid, Failed, Cancelled, Refunded }
enum PaymentMethod         { VNPAY, MoMo, ZaloPay, Visa, Mastercard, BankTransfer }
enum TransactionStatus     { Pending, Success, Failed, Refunded }
enum QuestionType          { SingleChoice, MultipleChoice, TrueFalse }
enum QuizType              { MidLesson, Final }
enum CertificateStatus     { Valid, Revoked }
enum ReviewRequestStatus   { Pending, Approved, Rejected }
enum VoucherType           { FixedAmount, Percentage }
enum VoucherScope          { AllCourses, SpecificCourse, Category }
enum RefundStatus          { Pending, Approved, Rejected }
enum SecurityEvent         { Login, Logout, NewDevice, SessionKicked, SuspiciousIp, TabSwitch }
enum BadgeType             { Streak7, Streak30, Streak100, FirstCourse, FirstCertificate }
enum XpReason              { LessonCompleted, QuizPassed, CertificateEarned, StreakMilestone }
```

---

## 4. Quan hệ chính (diagram tóm tắt)

```
User ──────────────── EmployeeProfile (1-1)
                              │
                    ┌─────────┼──────────┐
                    │         │          │
              Position   LeaveRequest   PayrollItem
                    │
              Department ─── Organization

Course ──── CourseSection ──── Lesson
   │              │
   │         VideoProgress (User × Lesson)
   │
CourseEnrollment (User × Course)
   │
   ├── CourseProgress
   ├── LessonCompletion
   └── Certificate

Order ── OrderItem ── Course
   │
Transaction (gateway callback)

ApprovalRequest ──── ApprovalStep (multi-step)
   │
   └── (LeaveRequest | OvertimeRequest | RecruitmentRequest | ...)
```

---

## 5. Migration order (thứ tự tạo để tránh FK conflict)

```
Sprint 0:
1. Organization
2. Department (update: add OrganizationId, ParentId, CostCenter, ManagerId)
3. Position
4. EmployeeProfile + EmployeeEducation + EmployeeSkill

Sprint 1 (HRM core):
5. WorkShift → ShiftAssignment → AttendanceRecord → AttendanceAdjustment → MonthlyAttendanceSummary
6. LeaveType → LeaveBalance → LeaveRequest
7. OvertimeRequest
8. ApprovalWorkflowConfig → ApprovalRequest → ApprovalStep → ApprovalEscalation

Sprint 2:
9. SalaryConfig → PayrollRun → PayrollItem → SalaryAdvance

Sprint 3:
10. KpiIndicator → KpiTemplate → KpiTemplateIndicator → KpiTarget → KpiActual → PerformanceReview

Sprint 4:
11. RecruitmentRequest → Candidate → InterviewSchedule → InterviewEvaluation → JobOffer
12. OnboardingChecklist → OnboardingTaskTemplate → EmployeeOnboarding → EmployeeOnboardingTask
13. EmployeeOffboarding → OffboardingTask
14. EmploymentContract → ContractAmendment → LegalDocument
15. RewardRecord → DisciplinaryRecord
16. AssetAllocation

Sprint 0 (LMS):
17. CourseCategory → Course → CourseSection → Lesson → CourseReviewRequest

Sprint 1 (LMS):
18. Order → OrderItem → Transaction → ActivationCode → CourseEnrollment

Sprint 2 (LMS):
19. VideoProgress → LessonCompletion → CourseProgress → LearningSession → LessonNote
20. QuestionBank → Question → QuestionOption → QuizConfig → ExamAttempt → ExamAnswer

Sprint 3 (LMS):
21. CertificateTemplate → Certificate
22. LessonComment → CommentUpvote → CourseRating → InstructorRatingReply
23. LearningStreak → LearnerBadge → XpTransaction → ReminderConfig
24. TrustedDevice → ActiveSession → SecurityLog → LoginHistory
25. Voucher → VoucherUsage → RefundPolicy → RefundRequest
```

---

*Cập nhật lần cuối: 2026-07-27. Review lại khi SRS được khách hàng xác nhận phạm vi chính thức.*
