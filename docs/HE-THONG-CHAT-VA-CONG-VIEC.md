# Hệ thống Chat & Quản lý công việc — Đặc tả chi tiết

> Tài liệu mô tả **thiết kế cơ sở dữ liệu và đặc tả logic nghiệp vụ** cho hai module **Chat nội bộ** (Chat) và **Quản lý công việc** (Task) trong `erp-corporation-api-v2`.  
> Dùng làm nguồn tham chiếu thống nhất khi lập trình Entity, Migration, Service, API và tích hợp Realtime (SignalR).  
> Cập nhật cùng PR khi có thay đổi thiết kế cơ sở dữ liệu hoặc quy tắc nghiệp vụ.

**Trạng thái:** Đề xuất thiết kế cơ sở dữ liệu + Kiến trúc tích hợp.

---

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Nguyên tắc thiết kế đã chốt](#2-nguyên-tắc-thiết-kế-đã-chốt)
3. [Enum trong Domain (không lưu bảng DB)](#3-enum-trong-domain-không-lưu-bảng-db)
4. [Quy ước Audit & Entity Base](#4-quy-ước-audit--entity-base)
5. [Danh sách bảng](#5-danh-sách-bảng)
6. [Chi tiết từng bảng](#6-chi-tiết-từng-bảng)
    - [6.1. Nhóm bảng Quản lý công việc (Task)](#61-nhóm-bảng-quản-lý-công-việc-task)
    - [6.2. Nhóm bảng Chat nội bộ (Conversation)](#62-nhóm-bảng-chat-nội-bộ-conversation)
7. [Logic Phân quyền & Phạm vi Dữ liệu (Role & Scope)](#7-logic-phân-quyền--phạm-vi-dữ-liệu-role--scope)
8. [Logic Tích hợp Liên kết (Chat ↔ Task ↔ LMS ↔ HR)](#8-logic-tích-hợp-liên-kết-chat--task--lms--hr)
9. [SignalR & Realtime Flow](#9-signalr--realtime-flow)
10. [API Endpoints Đề xuất](#10-api-endpoints-đề-xuất)
11. [Kiến trúc code (Clean Architecture)](#11-kiến-trúc-code-clean-architecture)
12. [Sơ đồ quan hệ thực thể (ERD)](#12-sơ-đồ-quan-hệ-thực-thể-erd)

---

## 1. Tổng quan

Hệ thống cung cấp hai công cụ cộng tác cốt lõi cho doanh nghiệp:
- **Chat nội bộ**: Hỗ trợ trao đổi tin nhắn realtime 1-1, chat nhóm (phòng ban/dự án) và Channel thông báo công khai/riêng tư.
- **Quản lý công việc**: Cho phép tạo, giao việc, theo dõi tiến độ, bình luận thảo luận và liên kết nhiệm vụ với các module nhân sự (KPI, LMS, Onboarding).

Hai module này được thiết kế tích hợp sâu: người dùng có thể tạo nhanh Task trực tiếp từ một tin nhắn chat và gắn link Task đó vào cuộc trò chuyện.

---

## 2. Nguyên tắc thiết kế đã chốt

### 2.1. Tách biệt thực thể & Không lạm dụng bảng DB cho Enum
- Trạng thái công việc, độ ưu tiên, loại cuộc trò chuyện, loại tin nhắn được lưu dưới dạng **số nguyên (int)** trong DB và mapping với **C# Enum** trong code Domain. Không tạo bảng metadata thừa.

### 2.2. Kiểm soát Audit tự động
- Các bảng chính (`Tasks`, `Task_Templates`, `Conversations`, `Messages`) kế thừa `BaseEntity` và implement các marker interface (`IAuditable`, `ICreationTracked`, `IModificationTracked`).
- Thời gian và người thao tác được `AuditSaveChangesInterceptor` tự động điền thông qua DI `TimeProvider` và `ICurrentUserService`.

### 2.3. Ràng buộc toàn vẹn & Index hiệu năng
- Sử dụng Khóa chính hỗn hợp (Composite Primary Key) cho các bảng quan hệ nhiều-nhiều (`Task_Assignees`, `Task_Followers`, `Task_KPIs`, `Conversation_Members`, `Message_Tasks`).
- Các chỉ mục (Index) được đánh cấu trúc để tối ưu hóa truy vấn:
  - Lọc tin nhắn: `(ConversationId, SentAt DESC)`.
  - Hộp thư chat cá nhân: `(UserId, IsActive, JoinedAt DESC)`.
  - Task cá nhân: `(CreatedBy, DueDate DESC)` và các bảng liên kết theo `UserID`.

### 2.4. Xử lý xóa (Soft delete / Deactivation)
- Tin nhắn bị xóa (`Messages.IsDeleted = true`) không xóa vật lý để giữ tính toàn vẹn của thread thảo luận.
- Cuộc trò chuyện có thuộc tính `IsArchived = true` khi lưu trữ.
- Rời nhóm được đánh dấu bằng `Conversation_Members.IsActive = false` (không xóa cứng để lưu vết audit).

---

## 3. Enum trong Domain (không lưu bảng DB)

Các enum được khai báo tại `src/Domain/Enums/`. Trong database, các trường tương ứng sẽ lưu dưới dạng **số nguyên (int)** tương ứng với giá trị định nghĩa bên dưới.

### 3.1. Các Enum cho Module Task (Quản lý công việc)

#### 3.1.1. `TaskType` (Loại nhiệm vụ)
| Giá trị | Code | Name | Mô tả |
| :---: | :--- | :--- | :--- |
| 1 | `normal` | `Normal` | Nhiệm vụ thông thường |
| 2 | `recurring` | `Recurring` | Nhiệm vụ lặp lại |
| 3 | `project` | `Project` | Thuộc dự án |
| 4 | `onboarding` | `Onboarding` | Nhiệm vụ chào mừng NV mới |
| 5 | `offboarding` | `Offboarding` | Nhiệm vụ khi NV nghỉ việc |
| 6 | `training` | `Training` | Liên quan đào tạo |

```csharp
namespace Domain.Enums.Task;

public enum TaskType
{
    Normal = 1,
    Recurring = 2,
    Project = 3,
    Onboarding = 4,
    Offboarding = 5,
    Training = 6
}
```

#### 3.1.2. `TaskStatus` (Trạng thái nhiệm vụ)
| Giá trị | Code | Name | Màu gợi ý (UI) |
| :---: | :--- | :--- | :--- |
| 1 | `todo` | `ToDo` | Gray |
| 2 | `in_progress` | `InProgress` | Blue |
| 3 | `review` | `Review` | Orange |
| 4 | `done` | `Done` | Green |
| 5 | `cancelled` | `Cancelled` | Red |
| 6 | `overdue` | `Overdue` | Dark Red |

```csharp
namespace Domain.Enums.Task;

public enum TaskStatus
{
    ToDo = 1,
    InProgress = 2,
    Review = 3,
    Done = 4,
    Cancelled = 5,
    Overdue = 6
}
```

#### 3.1.3. `TaskPriority` (Độ ưu tiên nhiệm vụ)
| Giá trị | Code | Name | Màu gợi ý (UI) |
| :---: | :--- | :--- | :--- |
| 1 | `low` | `Low` | Green |
| 2 | `medium` | `Medium` | Yellow |
| 3 | `high` | `High` | Orange |
| 4 | `urgent` | `Urgent` | Red |

```csharp
namespace Domain.Enums.Task;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}
```

#### 3.1.4. `RecurringPattern` (Chu kỳ lặp nhiệm vụ)
| Giá trị | Code | Name | Giải nghĩa |
| :---: | :--- | :--- | :--- |
| 1 | `none` | `None` | Không lặp |
| 2 | `daily` | `Daily` | Hàng ngày |
| 3 | `weekly` | `Weekly` | Hàng tuần |
| 4 | `monthly` | `Monthly` | Hàng tháng |
| 5 | `quarterly` | `Quarterly` | Hàng quý |
| 6 | `yearly` | `Yearly` | Hàng năm |

```csharp
namespace Domain.Enums.Task;

public enum RecurringPattern
{
    None = 1,
    Daily = 2,
    Weekly = 3,
    Monthly = 4,
    Quarterly = 5,
    Yearly = 6
}
```

#### 3.1.5. `TaskActivityAction` (Hành động lịch sử Task)
| Giá trị | Code | Name | Ý nghĩa hiển thị |
| :---: | :--- | :--- | :--- |
| 1 | `created` | `Created` | Tạo nhiệm vụ |
| 2 | `status_changed` | `StatusChanged` | Thay đổi trạng thái |
| 3 | `progress_updated` | `ProgressUpdated` | Cập nhật tiến độ |
| 4 | `assigned` | `Assigned` | Gán người thực hiện |
| 5 | `unassigned` | `Unassigned` | Bỏ gán |
| 6 | `due_date_changed` | `DueDateChanged` | Thay đổi hạn chót |
| 7 | `comment_added` | `CommentAdded` | Thêm bình luận |
| 8 | `completed` | `Completed` | Hoàn thành |
| 9 | `cancelled` | `Cancelled` | Hủy |
| 10 | `subtask_added` | `SubtaskAdded` | Thêm subtask |

```csharp
namespace Domain.Enums.Task;

public enum TaskActivityAction
{
    Created = 1,
    StatusChanged = 2,
    ProgressUpdated = 3,
    Assigned = 4,
    Unassigned = 5,
    DueDateChanged = 6,
    CommentAdded = 7,
    Completed = 8,
    Cancelled = 9,
    SubtaskAdded = 10
}
```

---

### 3.2. Các Enum cho Module Chat (Hội thoại & Tin nhắn)

#### 3.2.1. `ConversationType` (Loại cuộc trò chuyện)
| Giá trị | Code | Name | Giải nghĩa |
| :---: | :--- | :--- | :--- |
| 1 | `direct` | `Direct` | Tin nhắn trực tiếp (1-1) |
| 2 | `group` | `Group` | Nhóm chat |
| 3 | `channel` | `Channel` | Channel công khai/riêng tư |

```csharp
namespace Domain.Enums.Chat;

public enum ConversationType
{
    Direct = 1,
    Group = 2,
    Channel = 3
}
```

#### 3.2.2. `RoleInConversation` (Vai trò trong cuộc trò chuyện)
| Giá trị | Code | Name | Quyền hạn hiển thị |
| :---: | :--- | :--- | :--- |
| 1 | `admin` | `Admin` | Quản trị viên phòng chat |
| 2 | `member` | `Member` | Thành viên thảo luận |
| 3 | `viewer` | `Viewer` | Thành viên chỉ đọc |

```csharp
namespace Domain.Enums.Chat;

public enum RoleInConversation
{
    Admin = 1,
    Member = 2,
    Viewer = 3
}
```

#### 3.2.3. `MessageType` (Loại tin nhắn)
| Giá trị | Code | Name | Định dạng |
| :---: | :--- | :--- | :--- |
| 1 | `text` | `Text` | Văn bản |
| 2 | `image` | `Image` | Ảnh |
| 3 | `video` | `Video` | Video |
| 4 | `file` | `File` | File đính kèm |
| 5 | `voice` | `Voice` | Tin nhắn thoại |
| 6 | `task_link` | `TaskLink` | Liên kết Task |
| 7 | `system` | `System` | Tin nhắn hệ thống |
| 8 | `poll` | `Poll` | Bình chọn |

```csharp
namespace Domain.Enums.Chat;

public enum MessageType
{
    Text = 1,
    Image = 2,
    Video = 3,
    File = 4,
    Voice = 5,
    TaskLink = 6,
    System = 7,
    Poll = 8
}
```

#### 3.2.4. `ConversationActivityAction` (Hoạt động lịch sử Chat)
| Giá trị | Code | Name | Giải nghĩa |
| :---: | :--- | :--- | :--- |
| 1 | `member_joined` | `MemberJoined` | Thành viên tham gia |
| 2 | `member_left` | `MemberLeft` | Thành viên rời nhóm |
| 3 | `name_changed` | `NameChanged` | Đổi tên nhóm/channel |
| 4 | `description_changed`| `DescriptionChanged`| Thay đổi mô tả |
| 5 | `message_deleted` | `MessageDeleted` | Xóa tin nhắn |
| 6 | `archived` | `Archived` | Lưu trữ |
| 7 | `unarchived` | `Unarchived` | Khôi phục |
| 8 | `pinned` | `Pinned` | Ghim tin nhắn |

```csharp
namespace Domain.Enums.Chat;

public enum ConversationActivityAction
{
    MemberJoined = 1,
    MemberLeft = 2,
    NameChanged = 3,
    DescriptionChanged = 4,
    MessageDeleted = 5,
    Archived = 6,
    Unarchived = 7,
    Pinned = 8
}
```

---

## 4. Quy ước Audit & Entity Base

- Thực thể chính kế thừa `BaseEntity` (chứa `Id` GUID).
- Thực thể nghiệp vụ quan trọng implement:
  - `ICreationTracked`: `CreatedAt` (datetime2), `CreatedBy` (Guid).
  - `IModificationTracked`: `UpdatedAt` (datetime2), `UpdatedBy` (Guid).
  - `IAuditable`: `IsActive` (bit).

---

## 5. Danh sách bảng

### 5.1. Phân hệ Quản lý công việc (Task)
| # | Tên bảng | Loại khóa chính | Vai trò / Mục đích |
|---|---|---|---|
| 1 | `Tasks` | Single (TaskID) | Bảng chính lưu trữ thông tin công việc |
| 2 | `Task_Assignees` | Composite (TaskID, UserID) | Danh sách thành viên được gán thực hiện |
| 3 | `Task_Followers` | Composite (TaskID, UserID) | Danh sách thành viên theo dõi công việc |
| 4 | `Task_Comments` | Single (CommentID) | Bình luận thảo luận trong công việc |
| 5 | `Task_Activity_Log` | Single (LogID) | Nhật ký ghi nhận thay đổi (Trạng thái, Tiến độ...) |
| 6 | `Task_KPIs` | Composite (TaskID, KPIID) | Liên kết công việc đóng góp vào KPI |
| 7 | `Task_LMS_Courses` | Composite (TaskID, CourseID) | Liên kết đào tạo (Task chỉ định học khóa học) |
| 8 | `Task_Templates` | Single (TemplateID) | Danh sách các mẫu công việc tái sử dụng |

### 5.2. Phân hệ Chat nội bộ (Chat)
| # | Tên bảng | Loại khóa chính | Vai trò / Mục đích |
|---|---|---|---|
| 1 | `Conversations` | Single (ConversationID) | Thông tin phòng chat / channel |
| 2 | `Conversation_Members` | Composite (ConversationID, UserID) | Danh sách và quyền thành viên trong phòng chat |
| 3 | `Messages` | Single (MessageID) | Lưu lịch sử tin nhắn |
| 4 | `Message_Attachments` | Single (AttachmentID) | File đính kèm (ảnh, video, văn bản) của tin nhắn |
| 5 | `Message_Reactions` | Single (ReactionID) | Thả cảm xúc lên tin nhắn |
| 6 | `Message_Read_Status` | Composite (MessageID, UserID) | Theo dõi trạng thái đã đọc của từng thành viên |
| 7 | `Message_Tasks` | Composite (MessageID, TaskID) | Bản đồ liên kết tin nhắn với công việc được tạo |
| 8 | `Conversation_Activity_Log` | Single (LogID) | Lịch sử hoạt động của nhóm chat (vào/ra nhóm, đổi tên...) |

---

## 6. Chi tiết từng bảng

### 6.1. Nhóm bảng Quản lý công việc (Task)

#### 6.1.1. Bảng `Tasks` (Nhiệm vụ)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TaskID` | `uniqueidentifier` | Có | PK (Khóa chính) |
| `TaskCode` | `nvarchar(50)` | Có | Mã định danh duy nhất (vd: TASK-202606-001) |
| `Title` | `nvarchar(255)` | Có | Tiêu đề công việc |
| `Description` | `nvarchar(max)` | Không | Nội dung chi tiết công việc |
| `TaskType` | `nvarchar(50)` | Không | Loại nhiệm vụ: Map với code của `TaskType` (normal, recurring, project...) |
| `Status` | `nvarchar(50)` | Có | Trạng thái: Map với code của `TaskStatus` (todo, in_progress, review...) |
| `Priority` | `nvarchar(50)` | Có | Độ ưu tiên: Map với code của `TaskPriority` (low, medium, high...) |
| `Progress` | `int` | Có | Tiến độ phần trăm hoàn thành (0 - 100) |
| `StartDate` | `datetime2` | Không | Ngày bắt đầu |
| `DueDate` | `datetime2` | Không | Hạn chót (Deadline) |
| `CompletedDate`| `datetime2` | Không | Ngày thực tế hoàn thành nhiệm vụ |
| `EstimatedHours`| `decimal(18,2)`| Không | Số giờ ước tính cần để hoàn thành |
| `ActualHours` | `decimal(18,2)`| Không | Số giờ thực tế thực hiện |
| `IsRecurring` | `bit` | Có | Công việc có lặp lại định kỳ hay không |
| `RecurringPattern`| `nvarchar(100)`| Không | Quy luật chu kỳ lặp: Map với code của `RecurringPattern` (none, daily...) |
| `ParentTaskID` | `uniqueidentifier`| Không | FK trỏ đến `Tasks.TaskID` (Phục vụ Subtask) |
| `CreatedBy` | `uniqueidentifier`| Có | FK trỏ đến `Users.Id` (Người tạo) |
| `CreatedAt` | `datetime2` | Có | Ngày tạo |
| `UpdatedAt` | `datetime2` | Có | Ngày cập nhật gần nhất |

#### 6.1.2. Bảng `Task_Assignees` (Người thực hiện)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TaskID` | `uniqueidentifier` | Có | PK, FK -> `Tasks.TaskID` |
| `UserID` | `uniqueidentifier` | Có | PK, FK -> `Users.Id` (Người được giao) |
| `AssignedAt` | `datetime2` | Có | Ngày gán nhiệm vụ |
| `AssignedBy` | `uniqueidentifier` | Có | FK trỏ đến `Users.Id` (Người gán - thường là Manager) |
| `IsPrimaryAssignee`| `bit` | Có | Người chịu trách nhiệm chính (có thể có nhiều người) |

#### 6.1.3. Bảng `Task_Followers` (Người theo dõi)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TaskID` | `uniqueidentifier` | Có | PK, FK -> `Tasks.TaskID` |
| `UserID` | `uniqueidentifier` | Có | PK, FK -> `Users.Id` (Người theo dõi) |
| `FollowedAt` | `datetime2` | Có | Ngày bắt đầu theo dõi |

#### 6.1.4. Bảng `Task_Comments` (Bình luận trong Task)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `CommentID` | `uniqueidentifier` | Có | PK (ID bình luận) |
| `TaskID` | `uniqueidentifier` | Có | FK -> `Tasks.TaskID` (ID nhiệm vụ) |
| `UserID` | `uniqueidentifier` | Có | FK -> `Users.Id` (ID người bình luận) |
| `Content` | `nvarchar(max)` | Có | Nội dung comment |
| `ParentCommentID`| `uniqueidentifier`| Không | FK trỏ đến `Task_Comments.CommentID` (Hỗ trợ trả lời comment thread) |
| `CreatedAt` | `datetime2` | Có | Thời gian bình luận |

#### 6.1.5. Bảng `Task_Activity_Log` (Lịch sử hoạt động Task)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `LogID` | `uniqueidentifier` | Có | PK (ID log) |
| `TaskID` | `uniqueidentifier` | Có | FK -> `Tasks.TaskID` (ID nhiệm vụ) |
| `UserID` | `uniqueidentifier` | Có | FK -> `Users.Id` (ID người thực hiện thay đổi) |
| `Action` | `nvarchar(100)` | Có | Hành động: Map với code của `TaskActivityAction` (created, status_changed...) |
| `OldValue` | `nvarchar(max)` | Không | Giá trị cũ |
| `NewValue` | `nvarchar(max)` | Không | Giá trị mới |
| `CreatedAt` | `datetime2` | Có | Thời gian thay đổi |

#### 6.1.6. Bảng `Task_KPIs` (Liên kết Task - KPI)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TaskID` | `uniqueidentifier` | Có | PK, FK -> `Tasks.TaskID` (ID nhiệm vụ) |
| `KPIID` | `uniqueidentifier` | Có | PK, FK trỏ đến bảng `KPIs` (ID KPI) |
| `Weight` | `decimal(5,2)` | Không | Trọng số đóng góp vào KPI |

#### 6.1.7. Bảng `Task_LMS_Courses` (Liên kết Task - Đào tạo)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TaskID` | `uniqueidentifier` | Có | PK, FK -> `Tasks.TaskID` (ID nhiệm vụ) |
| `CourseID` | `uniqueidentifier` | Có | PK, FK trỏ đến bảng `Courses` của LMS (ID khóa học LMS) |
| `RequiredForCompletion`| `bit` | Có | Hoàn thành task này có yêu cầu hoàn thành khóa học không |

#### 6.1.8. Bảng `Task_Templates` (Mẫu nhiệm vụ)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `TemplateID` | `uniqueidentifier` | Có | PK (ID template) |
| `TemplateName` | `nvarchar(255)` | Có | Tên mẫu nhiệm vụ |
| `Description` | `nvarchar(max)` | Không | Mô tả |
| `DefaultPriority`| `nvarchar(50)` | Không | Ưu tiên mặc định (Map với code của `TaskPriority`) |
| `DefaultDurationDays`| `int` | Không | Số ngày hoàn thành mặc định |
| `CreatedBy` | `uniqueidentifier` | Có | Người tạo template (FK -> `Users.Id`) |

---

### 6.2. Nhóm bảng Chat nội bộ (Conversation)

#### 6.2.1. Bảng `Conversations` (Cuộc trò chuyện)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `ConversationID` | `uniqueidentifier` | Có | PK (ID duy nhất của cuộc trò chuyện) |
| `ConversationType`| `nvarchar(50)` | Có | Loại cuộc trò chuyện: Map với code của `ConversationType` (direct, group, channel) |
| `Title` | `nvarchar(255)` | Không | Tên nhóm hoặc channel |
| `Description` | `nvarchar(500)` | Không | Mô tả nhóm |
| `IsPrivate` | `bit` | Có | Nhóm riêng tư hay công khai |
| `IsArchived` | `bit` | Có | Đã lưu trữ chưa |
| `CreatedBy` | `uniqueidentifier` | Có | ID người tạo (FK -> `Users.Id`) |
| `CreatedAt` | `datetime2` | Có | Thời gian tạo |
| `UpdatedAt` | `datetime2` | Có | Thời gian cập nhật |

#### 6.2.2. Bảng `Conversation_Members` (Thành viên)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `ConversationID` | `uniqueidentifier` | Có | PK, FK -> `Conversations.ConversationID` |
| `UserID` | `uniqueidentifier` | Có | PK, FK -> `Users.Id` (ID thành viên) |
| `RoleInConversation`| `nvarchar(50)` | Không | Vai trò thành viên: Map với code của `RoleInConversation` (admin, member, viewer) |
| `JoinedAt` | `datetime2` | Có | Ngày tham gia |
| `LastReadMessageID`| `uniqueidentifier` | Không | Tin nhắn cuối cùng người này đã đọc |
| `IsMuted` | `bit` | Có | Có tắt thông báo không |
| `IsActive` | `bit` | Có | Còn trong nhóm không |

#### 6.2.3. Bảng `Messages` (Tin nhắn)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `MessageID` | `uniqueidentifier` | Có | PK (ID tin nhắn) |
| `ConversationID` | `uniqueidentifier` | Có | FK -> `Conversations.ConversationID` (Thuộc cuộc trò chuyện nào) |
| `UserID` | `uniqueidentifier` | Có | FK -> `Users.Id` (Người gửi) |
| `Content` | `nvarchar(max)` | Không | Nội dung văn bản |
| `MessageType` | `nvarchar(50)` | Có | Loại tin nhắn: Map với code của `MessageType` (text, image, video, file, voice, task_link, system, poll) |
| `ParentMessageID`| `uniqueidentifier`| Không | ID tin nhắn cha (dùng để Reply / Thread) |
| `IsEdited` | `bit` | Có | Đã chỉnh sửa chưa |
| `IsDeleted` | `bit` | Có | Đã xóa chưa |
| `SentAt` | `datetime2` | Có | Thời gian gửi |
| `EditedAt` | `datetime2` | Không | Thời gian chỉnh sửa |

#### 6.2.4. Bảng `Message_Attachments` (Tệp đính kèm tin nhắn)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `AttachmentID` | `uniqueidentifier` | Có | PK (ID file) |
| `MessageID` | `uniqueidentifier` | Có | FK -> `Messages.MessageID` (Thuộc tin nhắn nào) |
| `FileName` | `nvarchar(255)` | Có | Tên file |
| `FileURL` | `nvarchar(1000)`| Có | Đường dẫn file (cloud) |
| `FileType` | `nvarchar(50)` | Có | Loại file: image / video / document / audio |
| `FileSize` | `int` | Có | Kích thước (bytes) |
| `ThumbnailURL` | `nvarchar(1000)`| Không | Đường dẫn ảnh thumbnail (dùng cho ảnh & video) |
| `UploadedAt` | `datetime2` | Có | Thời gian upload |

#### 6.2.5. Bảng `Message_Reactions` (Cảm xúc tin nhắn)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `ReactionID` | `uniqueidentifier` | Có | PK (ID reaction) |
| `MessageID` | `uniqueidentifier` | Có | FK -> `Messages.MessageID` (ID tin nhắn) |
| `UserID` | `uniqueidentifier` | Có | FK -> `Users.Id` (Người thả react) |
| `ReactionType` | `nvarchar(50)` | Có | Emoji cảm xúc (👍, ❤️, 😂, 🎉...) |
| `ReactedAt` | `datetime2` | Có | Thời điểm thả cảm xúc |

#### 6.2.6. Bảng `Message_Read_Status` (Trạng thái đọc)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `MessageID` | `uniqueidentifier` | Có | PK, FK -> `Messages.MessageID` (ID tin nhắn) |
| `UserID` | `uniqueidentifier` | Có | PK, FK -> `Users.Id` (Người đọc) |
| `IsRead` | `bit` | Có | Đã đọc hay chưa |
| `ReadAt` | `datetime2` | Không | Thời gian đọc |

#### 6.2.7. Bảng `Message_Tasks` (Liên kết Tin nhắn - Task)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `MessageID` | `uniqueidentifier` | Có | PK, FK -> `Messages.MessageID` (ID tin nhắn) |
| `TaskID` | `uniqueidentifier` | Có | PK, FK -> `Tasks.TaskID` (ID nhiệm vụ được gắn) |
| `LinkedAt` | `datetime2` | Có | Thời gian gắn |

#### 6.2.8. Bảng `Conversation_Activity_Log` (Hoạt động hội thoại)
| Cột | Kiểu dữ liệu | Bắt buộc | Mô tả |
| :--- | :--- | :---: | :--- |
| `LogID` | `uniqueidentifier` | Có | PK (ID log) |
| `ConversationID`| `uniqueidentifier` | Có | FK -> `Conversations.ConversationID` (ID cuộc trò chuyện) |
| `UserID` | `uniqueidentifier` | Không | Người thực hiện (FK -> `Users.Id`) |
| `Action` | `nvarchar(100)` | Có | Hoạt động: Map với code của `ConversationActivityAction` (member_joined, member_left...) |
| `Description` | `nvarchar(500)` | Không | Chi tiết |
| `CreatedAt` | `datetime2` | Có | Thời gian |

---

## 7. Logic Phân quyền & Phạm vi Dữ liệu (Role & Scope)

### 7.1. Phân quyền theo vai trò trong hệ thống (System Role)
Hệ thống áp dụng RBAC theo file cấu trúc phân quyền đã chốt:

- **Employee**:
  - Giao tiếp: Có quyền gửi/nhận tin nhắn chat 1-1, tham gia channel công khai, tạo nhóm chat nhỏ riêng tư.
  - Công việc: Tạo task cho bản thân, cập nhật tiến độ công việc được giao thực hiện, thêm bình luận/file vào task đó.
- **Manager**:
  - Giao tiếp: Tạo/quản lý channel cho team, ghim tin nhắn quan trọng trong cuộc trò chuyện nhóm của phòng ban.
  - Công việc: Tạo & giao task cho thành viên trong phòng ban quản lý, thay đổi thứ tự ưu tiên, phê duyệt hoàn thành task của cấp dưới.
- **HR Admin**:
  - Giao tiếp: Quản lý toàn bộ hệ thống channel công ty, export lịch sử chat phục vụ kiểm tra nội bộ.
  - Công việc: Xem báo cáo tổng hợp tiến độ công việc, tạo và cập nhật các mẫu danh sách Task (Template) dùng chung (như Onboarding nhân sự mới).
- **Leadership**:
  - Giao tiếp: Gửi thông báo toàn công ty (đặc quyền gửi tin nhắn lên channel Announcement).
  - Công việc: Xem dashboard tổng hợp tiến độ dự án, công việc và biểu đồ phân bổ tải công việc toàn doanh nghiệp.

### 7.2. Phạm vi Dữ liệu (Data Scope) trong Task
Phạm vi dữ liệu lấy từ `JobLevel.DefaultScopeType` của người dùng:
- **Scope Own**: Chỉ được thấy và cập nhật các task do bản thân tạo ra, được giao làm (Assignee) hoặc theo dõi (Follower).
- **Scope Team**: Thấy được toàn bộ task của các thành viên cấp dưới trực tiếp (trường `ManagerId` của thành viên trỏ tới ID của mình).
- **Scope Department**: Thấy được các task của tất cả nhân viên thuộc cùng phòng ban chính của Manager (bao gồm cả các phòng ban cấp con đệ quy).
- **Scope All**: Xem toàn bộ danh sách task trong doanh nghiệp (dành cho HR Admin và Leadership).

---

## 8. Logic Tích hợp Liên kết (Chat ↔ Task ↔ LMS ↔ HR)

### 8.1. Gắn Task vào Chat
1. Từ tin nhắn chat có nội dung công việc cụ thể, User click nút **"Tạo Task mới"**.
2. Hệ thống gọi API `POST /api/tasks` để khởi tạo Task.
3. Sau khi tạo Task thành công, hệ thống insert thông tin liên kết vào bảng `Message_Tasks`.
4. Đồng thời, một tin nhắn hệ thống (`MessageType = TaskLink`) được tự động gửi vào cuộc trò chuyện chứa link và thông tin tóm tắt của Task đó giúp các thành viên click xem nhanh.

### 8.2. LMS & Đào tạo tích hợp
- Khi giao công việc liên quan đến đào tạo chuyên môn, Manager có thể liên kết một khóa học LMS vào Task qua bảng `Task_LMS_Courses`.
- Nếu cột `RequiredForCompletion = true`, hệ thống sẽ chặn không cho nhân viên chuyển trạng thái Task sang `Review` hoặc `Done` cho đến khi hệ thống LMS gửi webhook xác nhận tài khoản nhân viên đã hoàn thành khóa học tương ứng.

### 8.3. Tự động hóa Onboarding nhân sự mới
- Khi HR Admin tạo hồ sơ nhân viên mới trên module Users → hệ thống tạo một Transaction nghiệp vụ:
  1. Tự động kích hoạt cơ chế tạo Group Chat chào mừng nhân viên mới bao gồm các đồng nghiệp cùng phòng ban.
  2. Tự động áp dụng `Task_Templates` có tên `"Onboarding Checklist"` để sinh danh sách các Task chuẩn bị và bàn giao công việc cho nhân viên mới.

---

## 9. SignalR & Realtime Flow

### 9.1. Khai báo SignalR Hub
- `ChatHub`: Lắng nghe các sự kiện và gửi tin nhắn trong thời gian thực.
  - Client Join Group: Khi user mở app, hệ thống gọi kết nối và đưa client vào các phòng chat tương ứng (`ConversationID` được map thành Group Name trong SignalR).
  - Gửi tin nhắn: Khi gửi thành công và DB lưu tin nhắn, Server broadcast qua Hub method:
    `Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", messageDto)`
  - Realtime Reaction/Read status: Client gửi sự kiện qua Hub và Server broadcast lại cho các client khác trong nhóm để cập nhật UI ngay lập tức.

### 9.2. Tích hợp Hệ thống Thông báo (Notification System)
- Khi một user được `@mention` trong tin nhắn chat nhóm, hệ thống song song ghi nhận tin nhắn và kích hoạt hệ thống thông báo (`PublishAsync`) với `TriggerKey = chat.message.mention`.
- Khi có thay đổi trạng thái Task được giao hoặc gần quá hạn, hệ thống thông báo phát đi `TriggerKey = task.status.changed` hoặc `task.deadline.warning` để đẩy tin nhắn in-app/email cho Assignees và Followers.

---

## 10. API Endpoints Đề xuất

### 10.1. API Chat & Cuộc trò chuyện (`/api/conversations`)
| Method | URL | Auth | Quyền (Permission) | Chức năng |
|---|---|---|---|---|
| `GET` | `/api/conversations` | Bearer | `chat.conversation.read` | Lấy danh sách cuộc trò chuyện của User hiện tại |
| `POST`| `/api/conversations` | Bearer | `chat.conversation.create` | Tạo phòng chat mới (Direct hoặc Group) |
| `POST`| `/api/conversations/channels` | Bearer | `chat.conversation.create` | Tạo Channel mới (Công khai/Riêng tư) |
| `PUT` | `/api/conversations/{id}` | Bearer | `chat.conversation.update` | Đổi tên nhóm/channel, cập nhật mô tả |
| `POST`| `/api/conversations/{id}/archive` | Bearer | `chat.conversation.update` | Lưu trữ cuộc trò chuyện |
| `POST`| `/api/conversations/{id}/members` | Bearer | `chat.member.manage` | Thêm thành viên vào phòng chat/nhóm |
| `DELETE`| `/api/conversations/{id}/members/{userId}` | Bearer | `chat.member.manage` | Xóa thành viên khỏi nhóm chat |

### 10.2. API Tin nhắn (`/api/messages`)
| Method | URL | Auth | Quyền (Permission) | Chức năng |
|---|---|---|---|---|
| `GET` | `/api/conversations/{conversationId}/messages` | Bearer | `chat.conversation.read` | Lấy danh sách tin nhắn trong cuộc trò chuyện (Phân trang) |
| `POST`| `/api/conversations/{conversationId}/messages` | Bearer | `chat.message.create` | Gửi tin nhắn mới (chấp nhận upload đính kèm file) |
| `PUT` | `/api/messages/{id}` | Bearer | `chat.message.update` | Chỉnh sửa tin nhắn (trong thời hạn 30 phút) |
| `DELETE`| `/api/messages/{id}` | Bearer | `chat.message.delete` | Thu hồi/xóa tin nhắn |
| `POST`| `/api/messages/{id}/reactions` | Bearer | `chat.message.create` | Thả cảm xúc lên tin nhắn |
| `POST`| `/api/conversations/{conversationId}/read` | Bearer | `chat.message.create` | Đánh dấu đã đọc tất cả tin nhắn trong phòng |

### 10.3. API Quản lý công việc (`/api/tasks`)
| Method | URL | Auth | Quyền (Permission) | Chức năng |
|---|---|---|---|---|
| `GET` | `/api/tasks` | Bearer | `task.item.read` | Lấy danh sách task (Áp dụng Data Scope và Bộ lọc) |
| `GET` | `/api/tasks/{id}` | Bearer | `task.item.read` | Chi tiết công việc và các subtask |
| `POST`| `/api/tasks` | Bearer | `task.item.create` | Tạo mới một công việc |
| `PUT` | `/api/tasks/{id}` | Bearer | `task.item.update` | Cập nhật thông tin công việc, trạng thái, tiến độ |
| `DELETE`| `/api/tasks/{id}` | Bearer | `task.item.delete` | Xóa công việc |
| `POST`| `/api/tasks/{id}/comments` | Bearer | `task.comment.create` | Gửi bình luận trong công việc |
| `GET` | `/api/tasks/reports` | Bearer | `task.report.read` | Lấy báo cáo hiệu suất, workload (dành cho HR/Leader) |

---

## 11. Kiến trúc code (Clean Architecture)

Cấu trúc các file trong project được phân bổ đối xứng theo đúng quy ước layer:

```text
Domain/
  Entities/
    Chat/
      Conversation.cs
      ConversationMember.cs
      Message.cs
      MessageAttachment.cs
      MessageReaction.cs
      MessageReadStatus.cs
      MessageTask.cs
      ConversationActivityLog.cs
    Tasks/
      TaskItem.cs
      TaskAssignee.cs
      TaskFollower.cs
      TaskComment.cs
      TaskActivityLog.cs
      TaskKpi.cs
      TaskLmsCourse.cs
      TaskTemplate.cs
  Enums/
    Chat/
      ConversationType.cs
      RoleInConversation.cs
      MessageType.cs
      ConversationActivityAction.cs
    Tasks/
      TaskType.cs
      TaskStatus.cs
      TaskPriority.cs
      RecurringPattern.cs
      TaskActivityAction.cs

Application/
  Interfaces/
    Repositories/
      Chat/
        IConversationRepository.cs
        IMessageRepository.cs
      Tasks/
        ITaskRepository.cs
    Services/
      Chat/
        IConversationService.cs
        IMessageService.cs
      Tasks/
        ITaskService.cs
  DTOs/
    Chat/
      ConversationDto.cs
      MessageDto.cs
      CreateMessageRequest.cs
    Tasks/
      TaskDto.cs
      CreateTaskRequest.cs
  Mappings/
    Chat/
      ChatMappingProfile.cs
    Tasks/
      TaskMappingProfile.cs

Infrastructure/
  Implementations/
    Repositories/
      Chat/
        ConversationRepository.cs
        MessageRepository.cs
      Tasks/
        TaskRepository.cs
    Services/
      Chat/
        ConversationService.cs
        MessageService.cs
      Tasks/
        TaskService.cs
  Persistence/
    Configurations/
      Chat/
        ConversationConfiguration.cs
        MessageConfiguration.cs
      Tasks/
        TaskConfiguration.cs

API/
  Controllers/
    Chat/
      ConversationsController.cs
      MessagesController.cs
    Tasks/
      TasksController.cs
  Hubs/
    ChatHub.cs
```

---

## 12. Sơ đồ quan hệ thực thể (ERD)

Mô tả quan hệ thực thể giữa 16 bảng của hai module:

```text
               +----------------------+
               |     Conversations    |
               +----------------------+
                 | 1                1 |
                 |                  |
                 | 1..N             | 1..N
        +--------v---------+      +-v---------------------+
        |  Conversation_   |      |        Messages       |
        |     Members      |      +-----------------------+
        +------------------+        | 1            | 1
                 | 1                |              |
                 |                  | 1..N         | 1..N
                 |                +-v----------+   |
                 |                |  Message_  |   |
                 |                |Attachments |   |
                 |                +------------+   |
                 |                                 |
                 |                               +-v-----------------+
                 |                               | Message_Reactions |
                 |                               +-------------------+
                 |
                 |   +----------------------+
                 +-->| Message_Read_Status  |
                     +----------------------+

                     
               +----------------------+
               |        Tasks         |<------------------+
               +----------------------+                   | (ParentTaskID)
                 | 1     | 1        | 1                   |
                 |       |          |                     |
                 | 1..N  | 1..N     | 1..N                |
        +--------v-----+ |        +-v------------+        |
        |     Task_    | |        |     Task_    |        |
        |  Assignees   | |        |   Followers  |        |
        |              | |        +--------------+        |
        +--------------+ |                                |
                         |                                |
                       +-v------------+                   |
                       |     Task_    |-------------------+
                       |   Comments   | 1..N
                       +--------------+
                         | 1
                         |
                         | 1..N (ParentCommentID)
                       +-v------------+
                       |     Task_    | (Thread reply)
                       |   Comments   |
                       +--------------+


        +-------------------------------------------------------------+
        |                 BẢNG LIÊN KẾT ĐA MODULE                     |
        +-------------------------------------------------------------+
        
        [Messages] (1) <─────── (1) [Message_Tasks] (1) ───────> (1) [Tasks]
        
        [Tasks] (1) <────────── (N) [Task_KPIs]
        
        [Tasks] (1) <────────── (N) [Task_LMS_Courses]
        
        [Task_Templates] (1) ── (Sinh các Tasks thực tế trong Onboarding)
```

---
*Tài liệu này định nghĩa chính xác cấu trúc dữ liệu và quy trình nghiệp vụ cho Chat & Task. Việc thay đổi cấu trúc bảng cần thực hiện đồng bộ qua Migration của EF Core.*
