# ERP BaHungBakery — Development Checklist
> Dựa trên SRS_HRM v1.0 + SRS_LMS v1.0 · Cập nhật: 2026-07-24  
> Source: `Ver-2/erp-corporation-api-v2` · Stack: .NET 10, Clean Architecture, EF Core, Redis, Quartz

**Ký hiệu:** `[ ]` chưa làm · `[x]` xong · `[~]` có 1 phần · `[!]` blocker

---

## 0. Foundation (blocker cho toàn bộ modules)

### 0.1 Infrastructure Services
- [ ] `IFileStorageService` — interface + impl (S3 hoặc local) cho upload video/PDF/ảnh
- [ ] SignalR Hub — real-time cho Chat + Notification (entity đã có, hub chưa có)
- [ ] Quartz job scheduler — impl cho `IBackgroundJobService` (config Quartz đã có, swap impl)
- [ ] 2FA handler — `Setup2FAResponse` model đã có, handler chưa có
- [ ] Payment gateway abstraction — `IPaymentGatewayService` (VNPAY/MoMo/ZaloPay/Stripe)
- [ ] Email marketing integration — SendGrid/Mailchimp cho LMS campaign
- [ ] Zalo OA integration (optional) — thông báo qua Zalo Official Account

### 0.2 Domain Entities thiếu (bị reference nhưng chưa tồn tại)
- [!] `Course` entity — `TaskLmsCourse` đang trỏ vào, build fail
- [!] `KpiIndicator` / `KpiTarget` entity — `TaskKpi` đang trỏ vào, build fail
- [!] LMS entities: `CourseSection`, `Lesson`, `Enrollment`, `LessonProgress`
- [!] HRM entities: `LeaveRequest`, `LeaveBalance`, `LeaveType`
- [!] HRM entities: `AttendanceRecord`, `WorkShift`, `ShiftAssignment`
- [!] HRM entities: `EmploymentContract`, `ContractType`
- [!] HRM entities: `PayrollRun`, `PayrollItem`, `SalaryConfig`

### 0.3 EF Configuration + Migration
- [ ] Migration cho tất cả entities mới sau khi tạo xong
- [ ] `IEntityTypeConfiguration` cho mỗi entity mới (theo pattern `BaseEntityConfiguration`)

---

## HRM — Quản lý nhân sự

### HRM-01 · Dữ liệu nhân sự & Cơ cấu tổ chức · **Must · Sprint 1**

**Domain**
- [~] `User` entity — đã có nhưng thiếu: CCCD/CMND, quê quán, địa chỉ, người liên hệ khẩn cấp
- [~] `Department` entity — đã có, cần thêm: cây đa cấp (ParentId), cost center
- [~] `JobLevel` entity — đã có, cần thêm: ScopeType đã có, thêm cost center link
- [ ] `EmployeeProfile` entity — extended data: bằng cấp, kỹ năng, ngoại ngữ, ngày vào làm
- [ ] `Organization` / `Branch` entity — đa đơn vị/chi nhánh BaHungBakery
- [ ] `Position` entity — vị trí công việc, gắn với JobLevel + Department + Manager

**Application Features**
- [ ] `CreateEmployee` command + handler
- [ ] `UpdateEmployee` command + handler
- [ ] `DeactivateEmployee` command + handler (soft delete, không hard delete)
- [ ] `GetEmployee` query + handler
- [ ] `GetEmployees` query + handler (filter, paginate)
- [ ] `GetOrganizationTree` query — trả về cây phòng ban dạng nested
- [ ] `GetEmployee360Profile` query — tổng hợp: lịch sử lương, KPI, vi phạm, tài sản, đào tạo

**API**
- [ ] `EmployeesController`
- [ ] `DepartmentsController`
- [ ] `OrganizationsController`

---

### HRM-02 · Chấm công & Ca làm việc · **Must · Sprint 2**

**Domain**
- [ ] `WorkShift` entity — ca cố định/ca xoay/ca gãy (split shift)
- [ ] `ShiftAssignment` entity — phân ca theo nhân viên/cửa hàng/tuần
- [ ] `AttendanceRecord` entity — check-in/check-out, device_id, gps_lat, gps_lng, IP
- [ ] `AttendanceAdjustment` entity — bổ sung công khi thiết bị lỗi (cần phê duyệt)
- [ ] `MonthlyAttendanceSummary` entity — bảng công tổng hợp cuối kỳ (lockable)

**Application Features**
- [ ] `CheckIn` command — validate GPS/GEO-fence, ghi device
- [ ] `CheckOut` command
- [ ] `CreateShift` command
- [ ] `AssignShift` command — xếp ca cho nhân viên/nhóm
- [ ] `RequestShiftSwap` command + approval flow
- [ ] `SubmitAttendanceAdjustment` command (nhân viên)
- [ ] `ApproveAttendanceAdjustment` command (manager)
- [ ] `GetAttendanceSummary` query — bảng công theo nhân viên/kỳ
- [ ] `LockAttendancePeriod` command — chỉ HR Manager

**Infrastructure**
- [ ] Mobile GPS validation service
- [ ] Fingerprint/face device integration middleware (API hoặc middleware)

**API**
- [ ] `AttendanceController`
- [ ] `ShiftsController`

---

### HRM-03 · Nghỉ phép & Tăng ca (OT) · **Must · Sprint 3**

**Domain**
- [ ] `LeaveType` entity — phép năm, không lương, ốm, thai sản, kết hôn, tang chế
- [ ] `LeaveBalance` entity — số ngày phép còn lại theo nhân viên + loại phép
- [ ] `LeaveRequest` entity — đơn nghỉ, trạng thái, cấp duyệt
- [ ] `OvertimeRequest` entity — đăng ký OT, hệ số (150%/200%/300%)

**Application Features**
- [ ] `SubmitLeaveRequest` command
- [ ] `ApproveLeaveRequest` command (manager → HR nếu dài ngày)
- [ ] `RejectLeaveRequest` command
- [ ] `SubmitOvertimeRequest` command
- [ ] `ApproveOvertimeRequest` command
- [ ] `GetLeaveBalance` query — số phép còn lại real-time
- [ ] `GetLeaveHistory` query
- [ ] Auto-sync leave/OT vào AttendanceSummary (Quartz job)

**API**
- [ ] `LeaveController`
- [ ] `OvertimeController`

---

### HRM-04 · Tính lương (Payroll) · **Must · Sprint 4**

> **Dependency:** HRM-02 (bảng công) + HRM-03 (nghỉ phép/OT) phải xong trước

**Domain**
- [ ] `SalaryConfig` entity — công thức lương theo nhóm/vị trí (configurable, không hardcode)
- [ ] `PayrollRun` entity — kỳ lương, trạng thái (draft/reviewing/approved/locked)
- [ ] `PayrollItem` entity — chi tiết lương từng nhân viên: lương cơ bản, phụ cấp, KPI thưởng, OT, BHXH, TNCN, tạm ứng
- [ ] `SalaryAdvance` entity — tạm ứng lương

**Application Features**
- [ ] `RunPayroll` command — tự động tính lương theo công thức + bảng công + KPI
- [ ] `ReviewPayroll` command — HR Manager rà soát
- [ ] `ApprovePayroll` command — chốt kỳ lương
- [ ] `LockPayrollPeriod` command
- [ ] `GetPayslip` query — phiếu lương điện tử cho nhân viên (ESS)
- [ ] `GetPayrollSummary` query — bảng lương toàn bộ (chỉ Payroll Admin/HR Manager)
- [ ] `ExportPayrollToAccounting` command — đẩy chi phí lương sang Kế toán
- [ ] Quartz job — tính thuế TNCN lũy tiến theo biểu (configurable)
- [ ] Quartz job — tính BHXH/BHYT/BHTN (configurable theo tỷ lệ)

**API**
- [ ] `PayrollController`

---

### HRM-05 · KPI & Đánh giá hiệu suất · **Must · Sprint 5**

> **Dependency:** `KpiIndicator` domain entity blocker đã nêu ở 0.2

**Domain**
- [!] `KpiIndicator` entity — chỉ tiêu KPI (tên, đơn vị, trọng số, loại: manual/auto)
- [!] `KpiTarget` entity — giao KPI cho nhân viên theo kỳ
- [ ] `KpiActual` entity — kết quả thực tế (từ CRM/POS hoặc nhập tay)
- [ ] `PerformanceReview` entity — đánh giá định kỳ (nhân viên tự đánh giá + quản lý đánh giá)
- [ ] `KpiTemplate` entity — bộ chỉ tiêu mẫu theo vị trí

**Application Features**
- [ ] `CreateKpiTemplate` command — HR tạo bộ KPI theo chức danh
- [ ] `AssignKpi` command — giao KPI tự động theo chu kỳ (Quartz job)
- [ ] `UpdateKpiActual` command — cập nhật thực tế (manual hoặc từ integration)
- [ ] `SubmitSelfReview` command — nhân viên tự đánh giá
- [ ] `SubmitManagerReview` command — quản lý đánh giá
- [ ] `FinalizePerformanceReview` command — HR tổng hợp xếp loại
- [ ] `GetKpiDashboard` query — dashboard KPI theo nhân viên/phòng ban

**Integration hooks** (tích hợp kéo data tự động)
- [ ] Pull doanh số từ CRM/POS → cập nhật KPI Sales
- [ ] Pull tương tác từ Contact Center → cập nhật KPI CSKH

**API**
- [ ] `KpiController`
- [ ] `PerformanceReviewsController`

---

### HRM-06 · Tuyển dụng (Recruitment) · **Should · Sprint 6**

**Domain**
- [ ] `RecruitmentRequest` entity — đề nghị tuyển dụng của trưởng phòng
- [ ] `Candidate` entity — hồ sơ ứng viên
- [ ] `InterviewSchedule` entity — lịch phỏng vấn, người phỏng vấn
- [ ] `InterviewEvaluation` entity — đánh giá từng vòng
- [ ] `JobOffer` entity — đề nghị nhận việc

**Application Features**
- [ ] `SubmitRecruitmentRequest` command
- [ ] `ApproveRecruitmentRequest` command (HR Manager)
- [ ] `CreateCandidate` command (nhận CV)
- [ ] `UpdateCandidateStage` command — pipeline: CV → Sàng lọc → PV1 → PV2 → Offer → Trúng
- [ ] `ScheduleInterview` command
- [ ] `SubmitInterviewEvaluation` command (nhiều người chấm độc lập)
- [ ] `CreateOfferLetter` command
- [ ] `ConvertCandidateToEmployee` command — không nhập lại dữ liệu

**API**
- [ ] `RecruitmentController`
- [ ] `CandidatesController`

---

### HRM-07 · Onboarding / Offboarding · **Should · Sprint 6**

**Domain**
- [ ] `OnboardingChecklist` entity — template checklist theo vị trí
- [ ] `OnboardingTask` entity — từng bước: hồ sơ, tài khoản, tài sản, LMS training
- [ ] `OffboardingChecklist` entity — thu hồi tài sản, khóa tài khoản, bàn giao, exit interview

**Application Features**
- [ ] `StartOnboarding` command — kích hoạt checklist khi nhân viên vào
- [ ] `CompleteOnboardingTask` command — đánh dấu từng bước xong
- [ ] `StartOffboarding` command
- [ ] `CompleteOffboardingTask` command
- [ ] Auto-revoke all system access on offboarding completion date (Quartz job)
- [ ] `GetOnboardingProgress` query — HR theo dõi tiến độ tổng thể

**API**
- [ ] `OnboardingController`

---

### HRM-08 · Hợp đồng & Hồ sơ nhân sự · **Must · Sprint 6**

**Domain**
- [!] `EmploymentContract` entity — loại hợp đồng, ngày ký, ngày hết hạn, file đính kèm
- [ ] `ContractAmendment` entity — phụ lục hợp đồng (điều chỉnh lương/chức danh)
- [ ] `LegalDocument` entity — giấy KSK, NDA, quyết định bổ nhiệm/điều chuyển/kỷ luật

**Application Features**
- [ ] `CreateContract` command
- [ ] `RenewContract` command
- [ ] `AddContractAmendment` command
- [ ] `GetContractsByEmployee` query
- [ ] Quartz job — cảnh báo hợp đồng hết hạn trước 30/15 ngày (email + notification)

**API**
- [ ] `ContractsController`

---

### HRM-09 · Khen thưởng & Kỷ luật · **Could · Sprint 7**

**Domain**
- [ ] `RewardRecord` entity — khen thưởng (tiền thưởng, thư khen)
- [ ] `DisciplinaryRecord` entity — cảnh cáo, kỷ luật, hình thức

**Application Features**
- [ ] `ProposeReward` command (quản lý → HR Manager duyệt)
- [ ] `ProposeDisciline` command
- [ ] `ApproveRewardOrDiscipline` command
- [ ] Tự động gắn vào hồ sơ 360° nhân viên

---

### HRM-10 · Tài sản cấp phát · **Could · Sprint 7**

**Domain**
- [ ] `AssetAllocation` entity — tài sản cấp phát cho nhân viên (link sang Asset module)

**Application Features**
- [ ] `AllocateAsset` command
- [ ] `ReclaimAsset` command — auto-trigger khi offboarding

---

### HRM-11 · Cổng ESS/MSS · **Should · Sprint 7**

**Application Features**
- [ ] `GetMyProfile` query (ESS) — thông tin cá nhân
- [ ] `GetMyPayslip` query (ESS) — phiếu lương
- [ ] `GetMyAttendance` query (ESS) — bảng công
- [ ] `GetMyLeaveBalance` query (ESS)
- [ ] `GetMyKpiHistory` query (ESS)
- [ ] `GetTeamAttendance` query (MSS) — quản lý xem đội nhóm
- [ ] `GetTeamKpi` query (MSS)
- [ ] `GetTeamPerformance` query (MSS)
- [ ] Mobile-responsive API (không cần app riêng giai đoạn 1)

---

### HRM-12 · Engine phê duyệt (Approval Workflow) · **Must · Sprint 7**

**Domain**
- [ ] `ApprovalWorkflowConfig` entity — cấu hình số cấp, điều kiện rẽ nhánh theo loại đơn
- [ ] `ApprovalRequest` entity — đơn bất kỳ (nghỉ phép, OT, tuyển dụng, khen thưởng...)
- [ ] `ApprovalStep` entity — từng bước duyệt, người duyệt, trạng thái, ý kiến
- [ ] `ApprovalEscalation` entity — escalation khi quá hạn

**Application Features**
- [ ] `SubmitApprovalRequest` command — generic, nhận loại đơn + payload
- [ ] `ApproveStep` command
- [ ] `RejectStep` command
- [ ] `EscalateOverdue` command (Quartz job — cảnh báo khi đơn quá hạn)
- [ ] `GetPendingApprovals` query — inbox phê duyệt của quản lý

---

### HRM-13 · Báo cáo & Dashboard · **Must · Sprint 8**

**Application Features**
- [ ] `GetHeadcountDashboard` query — headcount theo phòng ban/chi nhánh, turnover rate
- [ ] `GetPayrollCostDashboard` query — quỹ lương, BHXH, OT vs ngân sách
- [ ] `GetKpiDistributionReport` query — top/bottom performer, phân bố xếp loại
- [ ] `GetAttendanceReport` query — tỷ lệ đi muộn, vắng mặt
- [ ] Export Excel/PDF cho tất cả báo cáo

---

### HRM-14 · Tích hợp hệ thống · **Must · Sprint 8**

- [ ] Push chi phí lương → Kế toán (API hoặc event)
- [ ] Pull doanh số từ CRM/POS → KPI (webhook hoặc polling Quartz)
- [ ] Pull tương tác Contact Center → KPI CSKH
- [ ] Sync lịch sử đào tạo từ LMS → hồ sơ 360° HRM
- [ ] Thiết bị chấm công vân tay/khuôn mặt (API middleware hoặc SDK)

---

## LMS — Học tập trực tuyến

### LMS-01 · Xác thực & Quản lý tài khoản · **Must · Sprint 1**

> Phần lớn đã có (Login/Register/JWT/Redis), chỉ cần bổ sung:

**Application Features**
- [~] `Register` command — đã có login, cần thêm: OAuth2 Google/Facebook
- [ ] `RegisterBulkByInviteLink` command — nhân viên nội bộ đăng ký qua link mời
- [ ] `ForgotPassword` command — OTP qua email/SMS (5 phút, 1 lần)
- [ ] `ResetPassword` command
- [ ] `GetLoginHistory` query — audit trail thiết bị/IP/browser

**Infrastructure**
- [ ] SMS OTP service (hoặc dùng email OTP trước)
- [ ] OAuth2 Google/Facebook provider

---

### LMS-02 · Mua & Kích hoạt khóa học · **Must · Sprint 2**

**Domain**
- [ ] `CourseEnrollment` entity — quyền truy cập khóa học, ngày kích hoạt, thời hạn
- [ ] `ActivationCode` entity — mã kích hoạt 1 lần, batch, thời hạn
- [ ] `Order` entity — đơn hàng mua khóa học
- [ ] `OrderItem` entity

**Application Features**
- [ ] `PurchaseCourse` command — học viên mua
- [ ] `ActivateWithCode` command — nhập mã kích hoạt
- [ ] `GrantCourseAccess` command — Admin cấp hàng loạt cho nhóm nhân viên (link HRM)
- [ ] `RevokeCourseAccess` command — thu hồi khi hoàn tiền/vi phạm
- [ ] `GenerateActivationCodes` command — tạo batch mã ngẫu nhiên, export Excel
- [ ] `GetMyEnrollments` query — danh sách khóa học đang sở hữu + thời hạn
- [ ] Quartz job — auto-revoke khi enrollment hết hạn

**API**
- [ ] `EnrollmentsController`
- [ ] `OrdersController`
- [ ] `ActivationCodesController`

---

### LMS-03 · Thanh toán trực tuyến · **Must · Sprint 2**

**Infrastructure**
- [ ] `IPaymentGatewayService` interface
- [ ] VNPAY adapter
- [ ] MoMo adapter
- [ ] ZaloPay adapter
- [ ] (Optional) Stripe adapter cho thẻ quốc tế

**Application Features**
- [ ] `CreatePaymentSession` command — tạo URL thanh toán
- [ ] `HandlePaymentWebhook` command — nhận kết quả từ cổng (HTTPS callback)
- [ ] `GetTransactionHistory` query
- [ ] `GetRevenueDashboard` query (Admin)
- [ ] Auto-send email hóa đơn sau thanh toán thành công (Quartz/background)

**API**
- [ ] `PaymentsController`
- [ ] `WebhooksController` — nhận callback từ payment gateway

---

### LMS-04 · Quản lý nội dung khóa học · **Must · Sprint 3**

**Domain**
- [!] `Course` entity — tên, mô tả, thumbnail, danh mục, cấp độ, giá, trạng thái (draft/pending/published)
- [!] `CourseSection` entity — chương (có thứ tự, có thể kéo-thả)
- [!] `Lesson` entity — bài học: loại (video/pdf/quiz/text), thứ tự, xem trước miễn phí?
- [ ] `CourseCategory` entity
- [ ] `CourseReviewRequest` entity — yêu cầu xuất bản, Admin duyệt

**Application Features**
- [ ] `CreateCourse` command (Giảng viên)
- [ ] `UpdateCourse` command
- [ ] `AddSection` command
- [ ] `AddLesson` command
- [ ] `ReorderSections` command — kéo-thả thứ tự
- [ ] `ReorderLessons` command
- [ ] `UploadLessonContent` command — video MP4/MOV, PDF/DOCX/PPTX (link IFileStorageService)
- [ ] `SubmitForReview` command (Giảng viên)
- [ ] `ApproveCourse` command (Admin)
- [ ] `RejectCourse` command (Admin + lý do)
- [ ] `GetCourseDetail` query
- [ ] `GetMyCourses` query (Giảng viên)

**API**
- [ ] `CoursesController`
- [ ] `SectionsController`
- [ ] `LessonsController`

---

### LMS-05 · Trải nghiệm học tập · **Must · Sprint 3**

**Domain**
- [ ] `LearningSession` entity — phiên học, timestamp, thiết bị, IP
- [ ] `VideoProgress` entity — vị trí dừng (timestamp) theo lesson + learner
- [ ] `LessonNote` entity — ghi chú tại timestamp cụ thể

**Infrastructure**
- [ ] Video streaming service — CDN signed URL (JWT token ngắn hạn 15-30 phút)
- [ ] PDF viewer (nhúng, không cho tải) — watermark tên/email học viên
- [ ] Watermark overlay service

**Application Features**
- [ ] `GetStreamingUrl` query — tạo signed URL ngắn hạn (không share được)
- [ ] `SaveVideoProgress` command — ghi nhớ vị trí dừng
- [ ] `GetLastPosition` query — "Tiếp tục từ phút X?"
- [ ] `AddLessonNote` command
- [ ] `GetMyNotes` query — tất cả ghi chú theo khóa học

---

### LMS-06 · Theo dõi tiến độ học tập · **Must · Sprint 4**

**Domain**
- [ ] `LessonCompletion` entity — bài học đã hoàn thành (80% video, cuộn qua PDF)
- [ ] `CourseProgress` entity — % hoàn thành tổng hợp theo enrollment

**Application Features**
- [ ] `MarkLessonCompleted` command — trigger khi đủ điều kiện (80% video)
- [ ] `GetCourseProgress` query (học viên)
- [ ] `GetAllLearnersProgress` query (Giảng viên/Admin — filter, paginate)
- [ ] `GetDropoffReport` query — bài học bị bỏ dở nhiều nhất
- [ ] Auto-trigger certificate generation khi đạt 100% + điểm thi đạt

---

### LMS-07 · Quiz & Bài thi · **Must · Sprint 4**

**Domain**
- [ ] `QuestionBank` entity — danh mục câu hỏi
- [ ] `Question` entity — loại: single choice, multiple choice, true/false
- [ ] `QuestionOption` entity — đáp án, đáp án đúng/sai, giải thích
- [ ] `QuizConfig` entity — cấu hình bài thi: số câu, thời gian, điểm đạt, số lần làm
- [ ] `ExamAttempt` entity — lần làm bài: timestamp, điểm, câu trả lời
- [ ] `ExamAnswer` entity — từng câu trả lời trong attempt

**Application Features**
- [ ] `CreateQuestion` command — Giảng viên tạo, gắn vào QuestionBank
- [ ] `ImportQuestionsFromExcel` command — import hàng loạt
- [ ] `ConfigureQuiz` command — gắn quiz vào lesson/cuối khóa
- [ ] `StartExamAttempt` command — lấy N câu ngẫu nhiên từ bank
- [ ] `SubmitExamAttempt` command — nộp bài, tính điểm
- [ ] `GetExamResult` query
- [ ] `GetExamStatistics` query (Giảng viên — phân phối điểm, câu sai nhiều)
- [ ] Tab-switch detection — ghi log sự kiện bất thường khi làm bài

---

### LMS-08 · Chứng chỉ có QR · **Must · Sprint 5**

**Domain**
- [ ] `Certificate` entity — certificate ID (unique), QR URL, ngày cấp, trạng thái
- [ ] `CertificateTemplate` entity — template PDF (logo, màu, chữ ký)

**Infrastructure**
- [ ] PDF certificate generator — tạo PDF với QR code nhúng
- [ ] QR code generator
- [ ] Public certificate verification endpoint (không cần login)

**Application Features**
- [ ] `IssueCertificate` command — auto-trigger sau khi hoàn thành khóa học
- [ ] `RevokeCertificate` command (Admin — khi hoàn tiền/gian lận)
- [ ] `GetMyCertificates` query
- [ ] `VerifyCertificate` query — public endpoint, trả về: tên học viên, khóa học, ngày cấp, trạng thái
- [ ] `DownloadCertificate` query — PDF
- [ ] `ManageCertificateTemplate` command (Admin)

**API**
- [ ] `CertificatesController`
- [ ] `CertificateVerificationController` (public, no auth)

---

### LMS-09 · Tương tác Học viên – Giảng viên · **Should · Sprint 5**

**Domain**
- [ ] `LessonComment` entity — bình luận/câu hỏi theo bài học, gắn timestamp video
- [ ] `CommentUpvote` entity
- [ ] `CourseRating` entity — 1-5 sao, nhận xét (mở sau khi học ≥30%)
- [ ] `InstructorReply` entity — phản hồi của giảng viên (1 lần/rating)

**Application Features**
- [ ] `PostComment` command — học viên đặt câu hỏi + timestamp video
- [ ] `ReplyComment` command — Giảng viên trả lời thread
- [ ] `UpvoteComment` command
- [ ] `SubmitCourseRating` command (chỉ khi ≥30% tiến độ)
- [ ] `ReplyToRating` command (Giảng viên — 1 lần)
- [ ] `ModerateComment` command (Admin — ẩn/xóa vi phạm)
- [ ] `GetLessonComments` query — sort by upvote
- [ ] `GetCourseRatings` query — phân bố sao, đánh giá nổi bật
- [ ] SignalR notification khi có câu hỏi mới (→ Giảng viên)

---

### LMS-10 · Nhắc học tiếp & Gamification · **Should · Sprint 5**

**Domain**
- [ ] `LearningStreak` entity — chuỗi ngày học liên tiếp, last_active_date
- [ ] `LearnerBadge` entity — huy hiệu đạt được (7/30/100 ngày streak)
- [ ] `XpTransaction` entity — điểm kinh nghiệm (hoàn thành bài, quiz, nhận chứng chỉ)
- [ ] `ReminderConfig` entity — cấu hình lịch nhắc cá nhân theo khóa học

**Application Features**
- [ ] `UpdateLearningStreak` command — cập nhật streak sau mỗi phiên học
- [ ] `AwardBadge` command — tự động khi đạt mốc streak
- [ ] `AddXp` command
- [ ] `SetLearningReminder` command
- [ ] Quartz job — gửi nhắc học khi không hoạt động X ngày (default 3)
- [ ] Quartz job — chiến dịch re-engagement email 3-7-14 ngày
- [ ] `GetLeaderboard` query (optional)

---

### LMS-11 · Bảo vệ nội dung số · **Must · Sprint 6**

**Domain**
- [ ] `TrustedDevice` entity — thiết bị đã đăng ký (max 2, configurable)
- [ ] `ActiveSession` entity — phiên học đang hoạt động (enforce 1 phiên cùng lúc)
- [ ] `SecurityLog` entity — IP, device, timezone, user agent mỗi phiên học

**Application Features**
- [ ] `RegisterDevice` command
- [ ] `RemoveDevice` command (học viên tự xóa từ xa)
- [ ] `EnforceSessionLimit` command — kick phiên cũ khi đăng nhập mới (SignalR)
- [ ] `DetectAnomalousLogin` command — 2 IP khác quốc gia cùng lúc → cảnh báo Admin
- [ ] `GetSecurityLogs` query (Admin)
- [ ] `GetMyDevices` query (học viên)
- [ ] Streaming token validation middleware — validate JWT token ngắn hạn trên mỗi request video

---

### LMS-12 · Voucher & Mã giảm giá · **Should · Sprint 6**

**Domain**
- [ ] `Voucher` entity — loại (%, cố định), giá trị, max discount, phạm vi áp dụng, thời hạn, số lần dùng
- [ ] `VoucherUsage` entity — lịch sử ai dùng, đơn nào

**Application Features**
- [ ] `CreateVoucher` command
- [ ] `CreateVoucherBatch` command — tạo hàng loạt mã ngẫu nhiên, export Excel
- [ ] `ValidateVoucher` command — kiểm tra hợp lệ tại bước thanh toán
- [ ] `ApplyVoucher` command
- [ ] `DeactivateVoucher` command
- [ ] `GetVoucherUsageReport` query

---

### LMS-13 · Hoàn tiền & Tranh chấp · **Must · Sprint 6**

**Domain**
- [ ] `RefundRequest` entity — yêu cầu hoàn tiền, lý do, trạng thái
- [ ] `RefundPolicy` entity — cấu hình: 7 ngày, <30% tiến độ (Admin config)

**Application Features**
- [ ] `SubmitRefundRequest` command (học viên)
- [ ] `ValidateRefundEligibility` command — tự động kiểm tra policy
- [ ] `ApproveRefund` command (Admin) — trigger: revoke access + revoke cert + refund gateway
- [ ] `RejectRefund` command (Admin + lý do)
- [ ] `GetRefundStatus` query (học viên)
- [ ] `GetRefundReport` query (Admin — tỷ lệ hoàn tiền theo khóa, lý do phổ biến)

---

### LMS-14 · Báo cáo & Dashboard · **Must · Sprint 7**

**Application Features**
- [ ] `GetRevenueDashboard` query — ngày/tháng/quý, theo khóa học, theo kênh thanh toán
- [ ] `GetLearnerDashboard` query — học viên mới, đang học, hoàn thành, bỏ dở
- [ ] `GetCompletionRateReport` query — theo khóa học, theo chương
- [ ] `GetDropoffReport` query — bài học bị bỏ dở, timestamp phổ biến nhất
- [ ] `GetExamScoreReport` query — phân phối điểm, tỷ lệ đạt/không đạt
- [ ] `GetInstructorReport` query — số học viên, lượt học, điểm đánh giá trung bình
- [ ] Export Excel/PDF tất cả báo cáo

---

### LMS-15 · Data Migration · **Must · Sprint 7**

**Infrastructure**
- [ ] Migration CLI tool — script chuyển dữ liệu từ hệ thống cũ
- [ ] Staging validation — chạy trên staging trước production
- [ ] Rollback mechanism
- [ ] `MigrationDashboard` — Admin theo dõi tiến độ real-time (bản ghi đã xử lý, lỗi, tỷ lệ thành công)

**Scope dữ liệu cần migrate:**
- [ ] Tài khoản học viên (email, họ tên, phone, ảnh — không migrate password plaintext)
- [ ] Lịch sử mua hàng + đơn hàng
- [ ] Quyền truy cập khóa học + thời hạn
- [ ] Tiến độ học tập (% hoàn thành, last timestamp)
- [ ] Chứng chỉ đã cấp → tạo mã QR mới trỏ verification endpoint mới
- [ ] Nội dung video/PDF → upload lên CDN mới, cập nhật URL
- [ ] Mã giảm giá còn hiệu lực

---

### LMS-16 · Tích hợp hệ thống · **Must · Sprint 8**

- [ ] HRM integration — đồng bộ lịch sử đào tạo vào hồ sơ 360° nhân sự
- [ ] HRM integration — nhận lệnh cấp quyền khóa học hàng loạt từ HRM
- [ ] Haravan integration — sync tài khoản + đơn hàng (webhook nhận sự kiện mua)
- [ ] Kế toán integration — đẩy doanh thu LMS (đơn hàng, hoàn tiền) vào hạch toán
- [ ] Email marketing integration — Mailchimp/SendGrid cho chiến dịch nhắc học, re-engagement
- [ ] Zalo OA integration (optional) — kích hoạt khóa học, nhắc học, nhận chứng chỉ
- [ ] VNPAY webhook handler
- [ ] MoMo webhook handler
- [ ] ZaloPay webhook handler

---

## Cross-cutting Concerns

### Notifications (đã có entities, cần thêm)
- [ ] `NotificationService` impl — gửi in-app notification
- [ ] Email notification cho: đơn duyệt/từ chối, hợp đồng sắp hết hạn, KPI giao, chứng chỉ cấp
- [ ] Push notification (FCM/APNs) — mobile ESS + LMS app
- [ ] SignalR real-time notification hub

### Audit Log (đã có entity)
- [ ] `AuditLog` interceptor cho tất cả sensitive operations: lương, chứng chỉ thu hồi, hoàn tiền, quyền truy cập

### Chat (đã có entities)
- [ ] SignalR Chat Hub — real-time messaging
- [ ] `SendMessage` command + handler
- [ ] `GetConversations` query
- [ ] `GetMessages` query
- [ ] File attachment upload (link IFileStorageService)

---

## Sprint Map (combined HRM + LMS)

| Sprint | HRM | LMS | Thời gian |
|--------|-----|-----|-----------|
| 0 | Setup DB schema, domain entities, EF migration | Setup domain entities LMS, migration | 1 tuần |
| 1 | HRM-01 Nhân sự & Org | LMS-01 Auth bổ sung | 2 tuần |
| 2 | HRM-02 Chấm công | LMS-02 Mua/Kích hoạt · LMS-03 Thanh toán | 2 tuần |
| 3 | HRM-03 Nghỉ phép & OT | LMS-04 Quản lý nội dung · LMS-05 Trải nghiệm | 2 tuần |
| 4 | HRM-04 Payroll | LMS-06 Tiến độ · LMS-07 Quiz | 2 tuần |
| 5 | HRM-05 KPI & Đánh giá | LMS-08 Chứng chỉ · LMS-09 Tương tác · LMS-10 Gamification | 2 tuần |
| 6 | HRM-06 Tuyển dụng · HRM-07 Onboarding · HRM-08 Hợp đồng | LMS-11 Bảo vệ nội dung · LMS-12 Voucher · LMS-13 Hoàn tiền | 2 tuần |
| 7 | HRM-09/10/11/12 ESS/Approval/Reward/Asset | LMS-14 Dashboard · LMS-15 Migration | 2 tuần |
| 8 | HRM-13 Dashboard · HRM-14 Tích hợp · UAT | LMS-16 Tích hợp · UAT · Go-live | 2 tuần |

**Tổng: ~17 tuần (4 tháng)**

---

## Ghi chú rủi ro

| Rủi ro | Module | Mitigation |
|--------|--------|------------|
| Payment gateway cần merchant account trước Sprint 2 | LMS-03 | BaHungBakery cung cấp API key trong Sprint 0 |
| KPI tự động cần CRM/POS có API sẵn | HRM-05 | Fallback: nhập tay, bật auto sau khi CRM xong |
| LMS-15 Migration: cần data mapping với hệ thống cũ | LMS-15 | Xác nhận cấu trúc cũ trong Sprint 0 |
| Thiết bị chấm công: cần SDK/API từ nhà cung cấp | HRM-02 | Xác nhận loại thiết bị + SDK trong Sprint 0 |
| Haravan API: cần sandbox + docs | LMS-16 | Yêu cầu Haravan cung cấp từ Sprint 0 |
| Video CDN: chi phí lưu trữ phụ thuộc vào tổng dung lượng cũ | LMS-04/15 | Estimate trong Sprint 0 sau khi có danh sách nội dung |
