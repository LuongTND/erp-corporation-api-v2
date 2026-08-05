# Entity Audit — Open Items
> Tất cả 🔴🟡 đã fix. Chỉ còn backlog 🟢 dưới đây.

## Open (backlog — address khi touch module)

| # | Module | Issue |
|---|---|---|
| R3 | Permission | `PermissionCode` string redundant nếu `Module+Action+Resource` đã unique → xem xét bỏ hoặc auto-generate |
| T9 | Tasks | `TaskTemplate` bare — không có subtask templates → thêm `TaskTemplateItem` collection khi cần |
| T11 | Tasks | `TaskStatus`/`TaskPriority` full entity cho lookup data — heavy nếu chỉ read |
| C6 | Chat | `IsPrivate` + `ConversationType` — business rule overlap chưa rõ: DM luôn private? Channel public? |
| N4 | Notifications | `RecipientRulesJson="{}"` JSON blob → typed entity hoặc enum-based khi logic phức tạp |
| G1 | HRM | Attendance module — `AttendanceRecord`, `LeaveRequest`, `WorkShift` chưa có entity (Sprint 2-3) |
| G2 | LMS | `Course`, `Enrollment`, `LessonProgress` chưa có entity (Sprint LMS) |
