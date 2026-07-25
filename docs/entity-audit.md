# Entity Audit — ERP Corporation API v2 Domain Layer

> Audited: 2026-07-22 | Scope: Task, Chat, Notification, User/Org, Roles

---

## Legend

| Symbol | Meaning |
|--------|---------|
| 🔴 | High — bug / data loss risk |
| 🟡 | Medium — inconsistency / maintainability |
| 🟢 | Low — design smell / future pain |
| ✅ | Fixed |

---

## Module: Users / Org Structure

| # | Severity | File | Issue | Fix |
|---|----------|------|-------|-----|
| U1 | ✅ | `User.cs` | `DepartmentId` FK cứng + `UserDepartments` collection — redundant double-tracking | Xóa `DepartmentId`/`Department` khỏi `User` và `Department.Users` collection; dùng `UserDepartment.IsPrimary` |
| U2 | ✅ | `User.cs` | `IsActive` + `UserStatus` enum — 2 field cùng nghĩa | `Status`/`IsActive` private setter, sync qua `ChangeStatus()`; `IsActive = Status is Active or Probation` |
| U3 | ✅ | `User.cs` | `CreatedBy`/`UpdatedBy` raw `Guid?` — không FK, không nav, không nhất quán với các entity khác | Tạo `IAuditable` + `AuditableEntityBase<T>`, các entity có audit extend base mới |
| U4 | ✅ | `EntityBase.cs` | `CreatedBy`/`UpdatedBy` không có trong base — scatter, mỗi entity tự khai báo | Tạo `AuditableEntityBase<T>` riêng, không pollute `EntityBase` |

---

## Module: Roles / Permissions

| # | Severity | File | Issue | Fix |
|---|----------|------|-------|-----|
| R1 | ✅ | `UserRole.cs` | Không có `ExpiresAt` — không support role có thời hạn | Thêm `DateTimeOffset? ExpiresAt` + domain method `IsValid()` check active + not revoked + not expired |
| R2 | ✅ | `Permission.cs` | `Resource` là free-form string — typo-prone khi query | Tạo `PermissionResources` const class theo module tại `Domain/Constants/` |
| R3 | 🟢 | `Permission.cs` | `PermissionCode` string redundant nếu `Module+Action+Resource` đã unique | Xem xét bỏ hoặc auto-generate từ 3 field kia |

---

## Module: Tasks

| # | Severity | File | Issue | Fix |
|---|----------|------|-------|-----|
| T1 | ✅ | `TaskItem.cs` | `UpdateStatus()` check `status.Code == "Done"` — magic string, vỡ nếu ai đổi seed data | Thêm `IsFinalState`/`IsInitialState` bool vào `TaskStatus`, bỏ string literal |
| T2 | ✅ | `TaskItem.cs` | `StartDate`/`DueDate`/`CompletedDate` dùng `DateTime?` nhưng `EntityBase.CreatedAt` dùng `DateTimeOffset` — timezone bug | Đổi toàn bộ `DateTime` → `DateTimeOffset` trên tất cả entities (TaskItem, UserAccount, TaskAssignee, TaskFollower, MessageTask, ConversationMember, MessageReadStatus, UserRole, RolePermission, Message) |
| T3 | ✅ | `TaskItem.cs` | `IsRecurring` bool + `RecurringPattern?` enum — redundant double-flag | Bỏ `IsRecurring`, dùng `RecurringPattern == null` là đủ |
| T4 | ✅ | `TaskItem.cs` | `TaskType.Recurring` + `IsRecurring` bool — redundant lại lần nữa | Bỏ `IsRecurring` (fix chung T3) |
| T5 | ✅ | `TaskKpi.cs` | `KPIID` Guid naked | Rename `KpiId`, comment rõ cross-module reference |
| T6 | ✅ | `TaskKpi.cs` | `Weight decimal?` — không có range validation | Private setter với `Math.Clamp(0-1)` |
| T7 | ✅ | `TaskLmsCourse.cs` | Thiếu `CompletedAt`, `CompletionStatus` | Thêm `CourseCompletionStatus` enum + `CompletedAt` + `MarkCompleted()` |
| T8 | ✅ | `TaskTemplate.cs` | `DefaultPriorityCode` string — magic string | Đổi sang `DefaultPriorityId` FK → `TaskPriority` |
| T9 | 🟢 | `TaskTemplate.cs` | Quá bare — không có subtask templates | Thêm `TaskTemplateItem` collection khi cần |
| T10 | ✅ | `TaskDependency.cs` | Thiếu `DependencyType` (FS/SS/FF/SF) | Thêm enum `DependencyType`, default `FinishToStart` |
| T11 | 🟢 | `TaskStatus.cs` / `TaskPriority.cs` | Full entity cho lookup data — heavy nếu chỉ read | OK nếu cần custom per-tenant |

---

## Module: Chat

| # | Severity | File | Issue | Fix |
|---|----------|------|-------|-----|
| C1 | ✅ | `Message.cs` | `IsDeleted` tự implement nhưng không có `DeletedBy`/`DeletedAt` — mất audit trail | Extend `SoftDeletableEntityBase<Guid>` — có sẵn `IsDeleted`, `DeletedAt`, `DeletedBy`; `Delete(Guid deletedBy)` |
| C2 | ✅ | `Message.cs` | `EditedAt` dùng `DateTime`, `EntityBase.CreatedAt` dùng `DateTimeOffset` — inconsistent timezone | Đổi `EditedAt` sang `DateTimeOffset?` (fix chung với T2) |
| C3 | ✅ | Chat module | Double read-tracking: `ConversationMember.LastReadMessageID` + `MessageReadStatus` per message | Design decision: giữ cả 2 — cursor (`LastReadMessageID`) cho unread count O(1), per-message (`MessageReadStatus`) cho "seen by" receipts. Khác nhau về use case. |
| C4 | ✅ | `Conversation.cs` | Thiếu `LastMessageAt` — phải query `MAX(Message.CreatedAt)` để sort inbox | Thêm `DateTimeOffset? LastMessageAt` |
| C5 | ✅ | `ConversationMember.cs` | Thiếu `LeftAt` — không biết khi nào user rời group | Thêm `DateTimeOffset? LeftAt` + `Leave()` domain method |
| C6 | 🟢 | `Conversation.cs` | `IsPrivate` + `ConversationType` — business rule overlap chưa rõ | Document rõ: DM luôn private? Channel có thể public? |
| C7 | ✅ | `ConversationMember.cs` | `RoleInConversation?` nullable — cần guard ở application layer | Default `RoleInConversation.Member` thay vì null |

---

## Module: Notifications

| # | Severity | File | Issue | Fix |
|---|----------|------|-------|-----|
| N1 | ✅ | `UserNotification.cs` | `TriggerKey` string không FK về `NotificationTriggerBinding` | Design decision: loose coupling — `TriggerKey` là string identifier cho trigger event, không cần FK. `EventTypeId` đã link typed. |
| N2 | ✅ | `UserNotification.cs` | Thiếu `Channel` — không distinguish in-app vs push vs email | Thêm `NotificationChannel Channel`, default `InApp` |
| N3 | ✅ | `UserNotification.cs` | Thiếu `ExpiresAt` — notification không bao giờ expire | Thêm `DateTimeOffset? ExpiresAt` |
| N4 | 🟢 | `NotificationTriggerBinding.cs` | `RecipientRulesJson = "{}"` — JSON blob cho business logic trong relational DB | Typed entity hoặc enum-based rules khi logic phức tạp |

---

## Missing Modules (Gap)

| # | Severity | Missing | Impact |
|---|----------|---------|--------|
| G1 | 🔴 | **Attendance / Chấm công** — không có `AttendanceRecord`, `LeaveRequest`, `LeaveType`, `WorkSchedule`, `Shift`, `Timesheet` | Nếu chấm công là core feature thì đây là gap lớn nhất |
| G2 | 🟡 | **LMS** — chỉ có `TaskLmsCourse` ref `CourseID`. Không có `Course`, `Enrollment`, `LessonProgress` | Nếu LMS internal: thiếu domain. Nếu external service: document + define integration contract |

---

## Summary

| Priority | Count | Action |
|----------|-------|--------|
| 🔴 High | 1 remaining (G1 — Attendance module) | Fix trước khi go-live |
| 🟡 Medium | ✅ Tất cả đã fix | — |
| 🟢 Low | C6, N4, R3, T9, T11 | Backlog, address khi touch module đó |
