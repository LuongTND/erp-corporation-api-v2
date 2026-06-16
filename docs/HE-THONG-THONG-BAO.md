# Hệ thống thông báo — Đặc tả thiết kế

> Tài liệu mô tả **thiết kế đã chốt** cho module thông báo trong `erp-corporation-api-v2`.  
> Dùng làm nguồn tham chiếu khi viết entity, migration, service, API và màn admin.  
> Cập nhật khi thay đổi quy tắc — **cùng PR** với thay đổi code (xem `CHECKLIST-PR.md`).

**Trạng thái:** Đã triển khai (Phase 1 + SignalR realtime).

---

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Nguyên tắc thiết kế đã chốt](#2-nguyên-tắc-thiết-kế-đã-chốt)
3. [Hai khái niệm: Trigger vs EventType](#3-hai-khái-niệm-trigger-vs-eventtype)
4. [Danh sách bảng](#4-danh-sách-bảng)
5. [Chi tiết từng bảng](#5-chi-tiết-từng-bảng)
6. [Luồng phát thông báo](#6-luồng-phát-thông-báo)
7. [Thêm chức năng mới — hai cách](#7-thêm-chức-năng-mới--hai-cách)
8. [Admin gán thông báo cho chức năng](#8-admin-gán-thông-báo-cho-chức-năng)
9. [Quy ước mã & placeholder](#9-quy-ước-mã--placeholder)
10. [API (đề xuất)](#10-api-đề-xuất)
11. [Kiến trúc code (khớp repo)](#11-kiến-trúc-code-khớp-repo)
12. [Phân quyền admin](#12-phân-quyền-admin)
13. [Lộ trình triển khai (Phase)](#13-lộ-trình-triển-khai-phase)
14. [Sơ đồ quan hệ (ERD)](#14-sơ-đồ-quan-hệ-erd)
15. [Ví dụ end-to-end](#15-ví-dụ-end-to-end)

---

## 1. Tổng quan

Hệ thống thông báo cho phép:

- Mỗi **chức năng nghiệp vụ** phát thông báo qua một **điểm kích hoạt cố định** (`TriggerKey`).
- **Admin** gán **loại thông báo** (`EventType`) cho từng trigger — dùng loại đã có hoặc tạo loại mới.
- **User** nhận thông báo trong **hộp thư in-app** (`UserNotifications`); email/push mở rộng ở phase sau.
- **Dev** không hard-code nội dung thông báo trong service — chỉ gọi `PublishAsync(triggerKey, ...)`.

**Luồng tóm tắt:**

```
Chức năng nghiệp vụ
    → PublishAsync(triggerKey, recipients, data)
        → Đọc NotificationTriggerBinding (trigger → eventType)
        → Load NotificationTemplate (title/body theo kênh)
        → Render placeholder {{...}}
        → Ghi UserNotifications (+ Outbox email ở Phase 2)
```

**Tách ba trụ:**

| Trụ | Trả lời câu hỏi | Lưu ở đâu |
|-----|-----------------|-----------|
| **Trigger** | Chức năng nào / hành động nào kích hoạt? | `NotificationTriggerBindings` |
| **EventType + Template** | Nội dung hiển thị thế nào? | `NotificationEventTypes`, `NotificationTemplates` |
| **Inbox** | User nhận gì, đã đọc chưa? | `UserNotifications` |

---

## 2. Nguyên tắc thiết kế đã chốt

### 2.1. Tách Trigger và EventType

- **Trigger** = điểm trong code (do dev đăng ký qua seed).
- **EventType** = catalog loại thông báo + template (admin quản lý).
- **Gán** trigger → eventType nằm trong DB, admin đổi được **không cần deploy lại**.

### 2.2. Một trigger — một eventType (Phase 1)

- Mỗi `TriggerKey` gán đúng **một** `EventTypeId` tại một thời điểm.
- **Nhiều trigger** có thể dùng **chung một** EventType (tái sử dụng template generic).

### 2.3. Dev không gọi trực tiếp EventCode

- Service nghiệp vụ **chỉ** gọi `PublishAsync(triggerKey, ...)`.
- Không gọi `PublishByEventCode("hrm.user.created")` trong feature code.

### 2.4. Trigger do dev seed, admin không tạo trigger mới (Phase 1)

- Trigger mới xuất hiện khi có chức năng mới (migration/seed hoặc `IDataSeeder`).
- Admin UI: **xem, gán, bật/tắt** — không tạo `TriggerKey` tùy ý (tránh orphan / typo).

### 2.5. Kênh thông báo

| Kênh | Enum | Phase |
|------|------|-------|
| In-app (hộp thư) | `InApp` | 1 |
| Email | `Email` | 2 |
| Push | `Push` | 3 |

Phase 1 chỉ bắt buộc template `InApp`; các kênh khác có thể null/disabled.

### 2.6. Tích hợp Outbox (Phase 2+)

- Email gửi qua **Outbox** (pattern đã có trong repo: `OutboxMessage`, `OutboxProcessorHostedService`).
- Không gửi SMTP đồng bộ trong request HTTP.

---

## 3. Hai khái niệm: Trigger vs EventType

| Khái niệm | Ai quản lý | Ví dụ | Mô tả |
|-----------|------------|-------|--------|
| **TriggerKey** | Dev (seed) | `hrm.user.create`, `hrm.leave.approve` | Điểm kích hoạt gắn với chức năng |
| **EventCode** | Admin | `hrm.user.created`, `system.generic.success` | Loại thông báo + bộ template |

**Ví dụ gán:**

| TriggerKey | EventType (EventCode) | Ý nghĩa |
|------------|----------------------|---------|
| `hrm.user.create` | `hrm.user.created` | Tạo user → template riêng |
| `hrm.leave.approve` | `system.generic.success` | Duyệt nghỉ → dùng template chung |
| `payroll.export` | `system.generic.success` | Xuất lương → cùng template với leave |

---

## 4. Danh sách bảng

| # | Bảng | Mục đích | Phase |
|---|------|----------|-------|
| 1 | `NotificationEventTypes` | Catalog loại thông báo | 1 |
| 2 | `NotificationTemplates` | Template theo event + kênh | 1 |
| 3 | `NotificationTriggerBindings` | Gán trigger → eventType | 1 |
| 4 | `UserNotifications` | Hộp thư in-app của user | 1 |
| 5 | `UserNotificationSettings` | User bật/tắt theo event/kênh | 3 |

---

## 5. Chi tiết từng bảng

### 5.1. `NotificationEventTypes`

Catalog **loại thông báo** — admin CRUD.

| Cột | Kiểu | Bắt buộc | Mô tả |
|-----|------|----------|--------|
| `Id` | `uniqueidentifier` | PK | |
| `EventCode` | `nvarchar(100)` | UK | Quy ước `{module}.{resource}.{action}` |
| `Name` | `nvarchar(200)` | | Tên hiển thị admin UI |
| `Module` | `nvarchar(50)` | | `HRM`, `Payroll`, `System`, … |
| `Description` | `nvarchar(500)` | | Mô tả + gợi ý placeholder |
| `DefaultTitleTemplate` | `nvarchar(500)` | | Fallback nếu chưa có row `NotificationTemplates` |
| `DefaultBodyTemplate` | `nvarchar(max)` | | |
| `IsActive` | `bit` | | `false` → không cho gán mới |
| `CreatedAt`, `UpdatedAt` | `datetime2` | | Audit |

**Seed sẵn (generic — tái sử dụng):**

| EventCode | Name |
|-----------|------|
| `system.generic.info` | Thông tin chung |
| `system.generic.success` | Thành công |
| `system.generic.warning` | Cảnh báo |
| `system.generic.error` | Lỗi / từ chối |

Generic dùng placeholder: `{{title}}`, `{{message}}`, `{{actorName}}` (tùy data dev truyền).

---

### 5.2. `NotificationTemplates`

Override template the **event + kênh** (admin tùy chỉnh nội dung).

| Cột | Kiểu | Bắt buộc | Mô tả |
|-----|------|----------|--------|
| `Id` | `uniqueidentifier` | PK | |
| `EventTypeId` | `uniqueidentifier` | FK | → `NotificationEventTypes` |
| `Channel` | `int` / enum | | `InApp`, `Email`, `Push` |
| `TitleTemplate` | `nvarchar(500)` | | Hỗ trợ `{{placeholder}}` |
| `BodyTemplate` | `nvarchar(max)` | | |
| `IsActive` | `bit` | | |
| `CreatedAt`, `UpdatedAt` | `datetime2` | | |

**Unique:** `(EventTypeId, Channel)`.

**Thứ tự resolve template:**

1. Row active trong `NotificationTemplates` (đúng channel).
2. Fallback `DefaultTitleTemplate` / `DefaultBodyTemplate` trên `NotificationEventTypes`.

---

### 5.3. `NotificationTriggerBindings`

**Bảng gán** — admin đổi loại thông báo cho chức năng tại đây.

| Cột | Kiểu | Bắt buộc | Mô tả |
|-----|------|----------|--------|
| `Id` | `uniqueidentifier` | PK | |
| `TriggerKey` | `nvarchar(100)` | UK | Mã điểm kích hoạt, dev seed |
| `Name` | `nvarchar(200)` | | Tên hiển thị: "Tạo nhân viên" |
| `Module` | `nvarchar(50)` | | Lọc UI |
| `Description` | `nvarchar(500)` | | Mô tả trigger + placeholder gợi ý |
| `EventTypeId` | `uniqueidentifier` | FK, nullable | **Null = chưa gán → không gửi** |
| `LinkUrlTemplate` | `nvarchar(500)` | | VD: `/users/{{userId}}`, `/leave-requests/{{id}}` |
| `IsActive` | `bit` | | `false` = tắt gửi (giữ gán) |
| `CreatedAt`, `UpdatedAt` | `datetime2` | | Audit |

**Quy tắc:**

- `TriggerKey` unique — một trigger một bản ghi.
- `EventTypeId` null hoặc event inactive → `PublishAsync` **no-op** (log debug, không throw).
- `IsActive = false` → no-op.

---

### 5.4. `UserNotifications`

Hộp thư in-app — bản ghi **đã render** (không lưu template).

| Cột | Kiểu | Bắt buộc | Mô tả |
|-----|------|----------|--------|
| `Id` | `uniqueidentifier` | PK | |
| `UserId` | `uniqueidentifier` | FK | Người nhận |
| `TriggerKey` | `nvarchar(100)` | | Audit: trigger đã kích hoạt |
| `EventTypeId` | `uniqueidentifier` | FK | Loại thông báo đã dùng |
| `Title` | `nvarchar(500)` | | Đã render |
| `Body` | `nvarchar(max)` | | Đã render |
| `LinkUrl` | `nvarchar(500)` | | Deep link (nullable) |
| `IsRead` | `bit` | | Mặc định `false` |
| `ReadAt` | `datetime2` | | |
| `CreatedAt` | `datetime2` | | Thời điểm gửi |

**Index gợi ý:** `(UserId, IsRead, CreatedAt DESC)`.

---

### 5.5. `UserNotificationSettings` (Phase 3)

User opt-in/out theo event hoặc module + kênh.

| Cột | Kiểu | Mô tả |
|-----|------|--------|
| `Id` | PK | |
| `UserId` | FK | |
| `EventTypeId` | FK, nullable | Null = setting theo module |
| `Module` | nvarchar | |
| `Channel` | enum | |
| `IsEnabled` | bit | |

Publisher kiểm tra setting **sau** khi resolve template, **trước** khi insert inbox.

---

## 6. Luồng phát thông báo

### 6.1. Sequence

```
┌─────────────┐     PublishAsync      ┌───────────────────────┐
│ *Service    │ ────────────────────► │ INotificationPublisher│
└─────────────┘                       └──────────┬────────────┘
                                                 │
                    ┌────────────────────────────┼────────────────────────────┐
                    ▼                            ▼                            ▼
         NotificationTriggerBindings   NotificationTemplates      (Phase 3) Settings
                    │                            │
                    └──────────────┬─────────────┘
                                   ▼
                          RenderTemplate(data)
                                   │
                                   ▼
                          UserNotifications (InApp)
                                   │
                          (Phase 2) Outbox → Email
```

### 6.2. Interface publisher (Application)

```csharp
Task PublishAsync(
    string triggerKey,
    IReadOnlyList<Guid> recipientUserIds,
    object data,
    CancellationToken cancellationToken = default);
```

**Optional overload** (link từ caller nếu không dùng `LinkUrlTemplate`):

```csharp
Task PublishAsync(
    string triggerKey,
    IReadOnlyList<Guid> recipientUserIds,
    object data,
    string? linkUrlOverride,
    CancellationToken cancellationToken = default);
```

### 6.3. Logic `NotificationPublisher` (Infrastructure)

1. Load binding theo `triggerKey` (cache per-request nếu cần).
2. Nếu binding null / inactive / `EventTypeId` null → return.
3. Load EventType + Template (`InApp`).
4. Render title/body: thay `{{key}}` bằng property từ `data` (case-insensitive).
5. Render `LinkUrl` từ `LinkUrlTemplate` + `data`, hoặc dùng override.
6. Insert `UserNotifications` cho từng `recipientUserId`.
7. (Phase 2) Enqueue Outbox nếu có template `Email` active.

### 6.4. Render placeholder

- Format: `{{propertyName}}` — map property public trên object `data` (anonymous type / DTO).
- Property thiếu → thay bằng chuỗi rỗng (Phase 1); log warning optional.
- Không execute script / HTML raw từ user input — escape khi hiển thị FE.

---

## 7. Thêm chức năng mới — hai cách

Dev **luôn** làm hai việc khi thêm chức năng có thông báo:

1. **Đăng ký trigger** (seed) — kèm gán EventType ban đầu (hoặc để null cho admin gán sau).
2. **Một lời gọi** `PublishAsync` sau khi nghiệp vụ thành công.

Admin/dev **không** sửa core publisher khi thêm feature.

### 7.1. Cách 1 — Gán loại thông báo đã có (tái sử dụng)

**Khi nào:** Nội dung linh hoạt, truyền từ code qua `data` (generic).

**Bước seed:**

```text
TriggerKey:     payroll.export
EventTypeId:    → system.generic.success (đã có)
LinkUrlTemplate: /payroll/exports/{{exportId}}
```

**Bước code:**

```csharp
await _notificationPublisher.PublishAsync(
    triggerKey: "payroll.export",
    recipientUserIds: [managerId],
    data: new
    {
        title = "Xuất bảng lương thành công",
        message = $"Kỳ {period} đã xuất bởi {actorName}",
        actorName = currentUser.FullName,
        exportId = export.Id
    },
    cancellationToken);
```

Admin sau này có thể đổi gán sang eventType khác trên UI.

---

### 7.2. Cách 2 — Tạo loại thông báo mới rồi gán

**Khi nào:** Copy/marketing cố định, placeholder nghiệp vụ rõ ràng.

**Bước 1 — Seed EventType:**

| EventCode | DefaultTitleTemplate | DefaultBodyTemplate |
|-----------|---------------------|---------------------|
| `hrm.leave.approved` | Đơn nghỉ đã duyệt | `{{employeeName}} nghỉ {{fromDate}}–{{toDate}}, duyệt bởi {{approverName}}` |

**Bước 2 — Seed binding:**

```text
TriggerKey:      hrm.leave.approve
EventTypeId:     → hrm.leave.approved
LinkUrlTemplate: /leave-requests/{{leaveRequestId}}
```

**Bước 3 — Code (giống cách 1):**

```csharp
await _notificationPublisher.PublishAsync(
    "hrm.leave.approve",
    recipientUserIds: [employeeUserId],
    data: new
    {
        employeeName = leave.Employee.FullName,
        fromDate = leave.FromDate.ToString("dd/MM/yyyy"),
        toDate = leave.ToDate.ToString("dd/MM/yyyy"),
        approverName = currentUser.FullName,
        leaveRequestId = leave.Id
    },
    cancellationToken);
```

Admin sửa template trên UI — không cần deploy lại.

---

### 7.3. Checklist dev khi thêm feature

| # | Việc | File / vị trí |
|---|------|----------------|
| 1 | Thêm constant `TriggerKey` | `Application/Constants/NotificationTriggers.cs` |
| 2 | Seed row `NotificationTriggerBindings` | `Infrastructure/Persistence/Seed/` |
| 3 | (Tuỳ chọn) Seed `NotificationEventTypes` mới | Cùng seeder |
| 4 | Gọi `PublishAsync` trong service | Sau `SaveChanges` thành công |
| 5 | Document placeholder trong `Description` của trigger | Admin UI đọc được |

---

## 8. Admin gán thông báo cho chức năng

Admin **không** sửa code — chỉ cập nhật **`EventTypeId`** trên bản ghi trigger.

### 8.1. Hai màn hình admin

**Màn A — Loại thông báo (`NotificationEventTypes`)**

- CRUD event type.
- Sửa template theo kênh (`NotificationTemplates`).
- Tạo loại mới trước khi gán (Cách 2).

**Màn B — Gán chức năng (`NotificationTriggerBindings`)**

- Danh sách trigger theo module (do dev seed).
- Cột **Loại thông báo**: dropdown các EventType `IsActive = true`.
- Nút lưu → cập nhật `EventTypeId`.
- Switch **Bật/Tắt** → `IsActive` (giữ gán, không gửi).

### 8.2. Thao tác gán loại đã tạo

1. Vào **Cài đặt → Thông báo → Gán chức năng**.
2. Tìm trigger (VD: `hrm.leave.approve`).
3. Dropdown **Loại thông báo** → chọn event đã có (VD: `hrm.leave.approved` hoặc `system.generic.success`).
4. (Tuỳ chọn) Sửa `LinkUrlTemplate`.
5. **Lưu**.

Từ thời điểm đó, mọi lần code gọi trigger đó dùng template của event vừa gán.

### 8.3. Tạo loại mới và gán (cùng luồng admin)

1. **Loại thông báo → Tạo mới** (EventCode, template, module).
2. **Gán chức năng →** chọn trigger → dropdown → chọn loại vừa tạo.
3. Lưu.

Hoặc nút **「Tạo loại mới & gán」** trên màn B (Phase 2 UI): mở modal tạo event, sau save tự gán vào trigger đang chọn.

### 8.4. Tắt thông báo tạm thời

- Set `IsActive = false` trên binding — chức năng vẫn chạy, publisher no-op.
- Không xóa binding / event type.

---

## 9. Quy ước mã & placeholder

### 9.1. TriggerKey

```
{module}.{resource}.{action}
```

| Ví dụ | Chức năng |
|-------|-----------|
| `hrm.user.create` | Tạo nhân viên |
| `hrm.user.update` | Cập nhật nhân viên |
| `hrm.leave.approve` | Duyệt đơn nghỉ |
| `hrm.leave.reject` | Từ chối đơn nghỉ |
| `payroll.payslip.generated` | Tạo phiếu lương |

- Chữ thường, phân cách `.`.
- Constant tập trung — không magic string rải rác.

### 9.2. EventCode

Cùng quy ước `{module}.{resource}.{action}` nhưng mô tả **sự kiện** (quá khứ / trạng thái):

| TriggerKey | EventCode (có thể khác) |
|------------|-------------------------|
| `hrm.user.create` | `hrm.user.created` |
| `hrm.leave.approve` | `hrm.leave.approved` |

Trigger và Event **không bắt buộc** trùng tên — gán linh hoạt qua binding.

### 9.3. Placeholder trong template

- Cú pháp: `{{camelCaseProperty}}`.
- Document trong `NotificationTriggerBindings.Description` và `NotificationEventTypes.Description`.
- Generic events: `{{title}}`, `{{message}}`, `{{actorName}}`.

---

## 10. API (đề xuất)

Base path: `/api/notifications` (inbox user) và `/api/notification-admin` (admin) — hoặc gom dưới `/api/notifications` với permission.

### 10.1. User inbox (Phase 1)

| Method | Path | Mô tả |
|--------|------|--------|
| `GET` | `/api/notifications` | Danh sách của user hiện tại (paginated, filter `isRead`) |
| `GET` | `/api/notifications/unread-count` | Số chưa đọc |
| `PATCH` | `/api/notifications/{id}/read` | Đánh dấu đã đọc |
| `PATCH` | `/api/notifications/read-all` | Đọc tất cả |

### 10.2. Admin — Loại thông báo

| Method | Path | Permission gợi ý |
|--------|------|------------------|
| `GET` | `/api/notification-event-types` | `system.notification.event.read` |
| `GET` | `/api/notification-event-types/{id}` | read |
| `POST` | `/api/notification-event-types` | `system.notification.event.create` |
| `PUT` | `/api/notification-event-types/{id}` | `system.notification.event.update` |
| `DELETE` | `/api/notification-event-types/{id}` | `system.notification.event.delete` |

Query: `page`, `pageSize`, `module`, `isActive`.

### 10.3. Admin — Template theo kênh

| Method | Path | Mô tả |
|--------|------|--------|
| `GET` | `/api/notification-event-types/{eventTypeId}/templates` | List template |
| `PUT` | `/api/notification-event-types/{eventTypeId}/templates/{channel}` | Upsert template |

### 10.4. Admin — Gán trigger (màn admin gán chức năng)

| Method | Path | Mô tả |
|--------|------|--------|
| `GET` | `/api/notification-triggers` | List binding (paginated, `module`) |
| `GET` | `/api/notification-triggers/{triggerKey}` | Chi tiết một trigger |
| `PUT` | `/api/notification-triggers/{triggerKey}` | **Gán / đổi EventTypeId**, `linkUrlTemplate`, `isActive` |

**Body `PUT`:**

```json
{
  "eventTypeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "linkUrlTemplate": "/leave-requests/{{leaveRequestId}}",
  "isActive": true
}
```

**Validation:**

- `eventTypeId` phải tồn tại và `IsActive = true` (hoặc `null` để bỏ gán).
- `triggerKey` phải tồn tại (404 nếu dev chưa seed).
- Không cho phép `POST` tạo trigger mới từ admin (Phase 1).

### 10.5. Response list (paginated)

Giống pattern hiện có: `PaginatedResult<T>` với `items`, `page`, `pageSize`, `totalCount`, …

---

## 11. Kiến trúc code (khớp repo)

Theo Clean Architecture hiện tại (`CAU-TRUC-THU-MUC.md`):

```
Domain/
  Entities/Notifications/
    NotificationEventType.cs
    NotificationTemplate.cs
    NotificationTriggerBinding.cs
    UserNotification.cs
  Enums/Notifications/
    NotificationChannel.cs

Application/
  Constants/
    NotificationTriggers.cs          // TriggerKey constants
  Contracts/
    INotificationPublisher.cs
    INotificationEventTypeService.cs
    INotificationTriggerService.cs
    IUserNotificationService.cs
  Dtos/Notifications/
    ...

Infrastructure/
  Implementations/
    Services/Notifications/
      NotificationPublisher.cs
      NotificationEventTypeService.cs
      NotificationTriggerService.cs
      UserNotificationService.cs
      TemplateRenderer.cs
  Persistence/
    Configurations/Notifications/
    Seed/NotificationSeed.cs

API/
  Controllers/
    NotificationsController.cs              // inbox user
    NotificationEventTypesController.cs     // admin
    NotificationTriggersController.cs       // admin gán
```

**Luồng gọi:**

```
Controller → I*Service → I*Repository → AppDbContext
Feature *Service → INotificationPublisher (không qua controller)
```

---

## 12. Phân quyền admin

Seed permissions (module `system`):

| Permission | Mô tả |
|------------|--------|
| `system.notification.event.read` | Xem loại thông báo |
| `system.notification.event.create` | Tạo loại mới |
| `system.notification.event.update` | Sửa template / event |
| `system.notification.event.delete` | Xóa (soft hoặc hard — chốt khi implement) |
| `system.notification.trigger.read` | Xem danh sách gán |
| `system.notification.trigger.update` | Gán / đổi loại, bật/tắt trigger |

User inbox: mọi user đã đăng nhập chỉ đọc/sửa **notification của chính mình** — không cần permission riêng.

Super Admin / `BypassDataScope`: không áp dụng cho notification admin — vẫn cần permission trên.

---

## 13. Lộ trình triển khai (Phase)

### Phase 1 — In-app core (MVP)

- [ ] Entity + migration 4 bảng (không có `UserNotificationSettings`).
- [ ] Seed: generic events + trigger mẫu (`hrm.user.create` → `hrm.user.created`).
- [ ] `INotificationPublisher` + `TemplateRenderer`.
- [ ] Tích hợp publish trong `UserService` (create user).
- [ ] API inbox user + admin CRUD event types + **PUT trigger binding**.
- [ ] FE tester: icon chuông + list + admin 2 màn (event types, trigger bindings).

### Phase 2 — Email + admin UX

- [ ] Template kênh `Email`.
- [ ] Outbox handler gửi email.
- [ ] Modal 「Tạo loại mới & gán」 trên màn trigger.
- [ ] Preview template với sample data.

### Phase 3 — User preferences & nâng cao

- [ ] `UserNotificationSettings`.
- [ ] Push notification.
- [ ] Rule người nhận phức tạp (role/department) — nếu cần, tách bảng `NotificationRecipientRules`.

---

## 14. Sơ đồ quan hệ (ERD)

```
NotificationEventTypes (1) ──────< NotificationTemplates (N)
         │
         │ EventTypeId
         ▼
NotificationTriggerBindings (N)     TriggerKey unique
         │
         │ publish → render
         ▼
UserNotifications (N) >────────── User (1)
         │
    EventTypeId (audit)
```

**Cardinality:**

- Một `EventType` — nhiều `Template` (mỗi channel một row).
- Một `EventType` — có thể được **nhiều Trigger** trỏ tới (tái sử dụng).
- Một `Trigger` — **một** `EventTypeId` (Phase 1).

---

## 15. Ví dụ end-to-end

### 15.1. Tạo user — event riêng

| Bước | Actor | Hành động |
|------|-------|-----------|
| 1 | Dev | Seed `hrm.user.create` → `hrm.user.created` |
| 2 | Dev | `UserService.CreateAsync` → `PublishAsync("hrm.user.create", [newUserId], data)` |
| 3 | Admin | (Tuỳ chọn) Sửa template "Chào mừng {{fullName}}..." |
| 4 | User | Mở inbox → thấy thông báo → click link `/users/{id}` |

### 15.2. Admin đổi sang generic success

| Bước | Actor | Hành động |
|------|-------|-----------|
| 1 | Admin | PUT `hrm.user.create` → `eventTypeId` = `system.generic.success` |
| 2 | Dev | Đảm bảo `data` có `title`, `message` khi publish |
| 3 | — | Lần tạo user tiếp theo dùng template generic — **không deploy** |

### 15.3. Trigger chưa gán

| Trạng thái | Hành vi |
|------------|---------|
| `EventTypeId = null` | Publish no-op |
| Admin gán event | Lần publish sau có thông báo |

---

## Phụ lục — Liên kết tài liệu

| Tài liệu | Nội dung |
|----------|----------|
| `HE-THONG-PHAN-QUYEN-VA-TO-CHUC.md` | RBAC, scope — pattern permission seed |
| `CAU-TRUC-THU-MUC.md` | Cấu trúc thư mục solution |
| `API-DOCUMENTATION.md` | Quy ước API chung (pagination, auth) |
| `CHECKLIST-PR.md` | Checklist trước khi merge |

---

*Tài liệu này mô tả thiết kế đã thống nhất: tách Trigger / EventType, admin gán qua `NotificationTriggerBindings`, dev chỉ publish theo `TriggerKey`.*
