# API Documentation — ERP Corporation API v2

> Tài liệu liệt kê tất cả API endpoints trong hệ thống.  
> **Cập nhật liên tục** mỗi khi thêm/sửa API mới.  
> Cập nhật lần cuối: **2026-06-18**

---

## Tổng quan API

### 🔐 Auth — Xác thực (`/api/auth`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 1   | Đăng nhập               | `POST`    | `/api/auth/login`                 | Đăng nhập, trả về access token và refresh token |
| 2   | Làm mới token           | `POST`    | `/api/auth/refresh`               | Lấy access token mới khi token cũ hết hạn       |
| 3   | Thu hồi token           | `POST`    | `/api/auth/revoke`                | Thu hồi refresh token (logout)                  |
| 4   | Thông tin hiện tại      | `GET`     | `/api/auth/me`                    | Lấy thông tin người dùng đang đăng nhập         |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 👤 Users — Quản lý người dùng (`/api/users`)

|-----|---------------------------|-----------|-----------------------------------------------|---------------------------------------|
| #   | Tên API                   | Method    | URL                                           | Chức năng                             |
|-----|---------------------------|-----------|-----------------------------------------------|---------------------------------------|
| 5   | DS người dùng             | `GET`     | `/api/users`                                  | Lấy danh sách tất cả người dùng       |
| 6   | Chi tiết người dùng       | `GET`     | `/api/users/{id}`                             | Lấy thông tin người dùng theo ID      |
| 7   | Tạo người dùng            | `POST`    | `/api/users`                                  | Tạo người dùng mới                    |
| 8   | Cập nhật người dùng       | `PUT`     | `/api/users/{id}`                             | Cập nhật thông tin người dùng         |
| 9   | Xóa người dùng            | `DELETE`  | `/api/users/{id}`                             | Xóa (vô hiệu hóa) người dùng          |
| 10  | Gán vai trò               | `POST`    | `/api/users/{id}/roles`                       | Gán danh sách vai trò cho người dùng  |
| 11  | Đặt lại mật khẩu          | `POST`    | `/api/users/{id}/reset-password`              | Đặt lại mật khẩu cho người dùng       |
| 12  | DS phòng ban kiêm nhiệm   | `GET`     | `/api/users/{id}/departments`                 | Lấy danh sách phòng ban kiêm nhiệm    |
| 13  | Gán phòng ban kiêm nhiệm  | `POST`    | `/api/users/{id}/departments`                 | Gán phòng ban kiêm nhiệm mới          |
| 14  | Gỡ phòng ban kiêm nhiệm   | `DELETE`  | `/api/users/{id}/departments/{departmentId}`  | Gỡ bỏ phòng ban kiêm nhiệm            |
|-----|---------------------------|-----------|-----------------------------------------------|---------------------------------------|

### 🏢 Departments — Quản lý phòng ban (`/api/departments`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 15  | DS phòng ban            | `GET`     | `/api/departments`                | Lấy tất cả phòng ban                            |
| 16  | Chi tiết phòng ban      | `GET`     | `/api/departments/{id}`           | Lấy thông tin phòng ban theo ID                 |
| 17  | Tạo phòng ban           | `POST`    | `/api/departments`                | Tạo phòng ban mới                               |
| 18  | Cập nhật phòng ban      | `PUT`     | `/api/departments/{id}`           | Cập nhật thông tin phòng ban                    |
| 19  | Xóa phòng ban           | `DELETE`  | `/api/departments/{id}`           | Xóa (vô hiệu hóa) phòng ban                     |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 🛡️ Roles — Quản lý vai trò (`/api/roles`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 20  | DS vai trò              | `GET`     | `/api/roles`                      | Lấy tất cả vai trò                              |
| 21  | Chi tiết vai trò        | `GET`     | `/api/roles/{id}`                 | Lấy thông tin vai trò theo ID                   |
| 22  | DS quyền                | `GET`     | `/api/roles/permissions`          | Lấy danh sách tất cả quyền                      |
| 23  | Tạo vai trò             | `POST`    | `/api/roles`                      | Tạo vai trò mới                                 |
| 24  | Cập nhật vai trò        | `PUT`     | `/api/roles/{id}`                 | Cập nhật thông tin cơ bản của vai trò           |
| 25  | Cập nhật quyền vai trò  | `PUT`     | `/api/roles/{id}/permissions`     | Cập nhật danh sách quyền cho vai trò            |
| 26  | Xóa vai trò             | `DELETE`  | `/api/roles/{id}`                 | Xóa (vô hiệu hóa) vai trò                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 🏷️ JobLevels — Quản lý cấp bậc chức danh (`/api/job-levels`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 27  | DS cấp bậc              | `GET`     | `/api/job-levels`                 | Lấy danh sách tất cả cấp bậc                    |
| 28  | Chi tiết cấp bậc        | `GET`     | `/api/job-levels/{id}`            | Lấy thông tin cấp bậc theo ID                   |
| 29  | Tạo cấp bậc             | `POST`    | `/api/job-levels`                 | Tạo cấp bậc mới                                 |
| 30  | Cập nhật cấp bậc        | `PUT`     | `/api/job-levels/{id}`            | Cập nhật cấp bậc chức danh                      |
| 31  | Xóa cấp bậc             | `DELETE`  | `/api/job-levels/{id}`            | Xóa (vô hiệu hóa) cấp bậc                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 💬 Conversations — Quản lý cuộc trò chuyện (`/api/conversations`)

|-----|---------------------------|-----------|-------------------------------------------|-------------------------------------------------|
| #   | Tên API                   | Method    | URL                                       | Chức năng                                       |
|-----|---------------------------|-----------|-------------------------------------------|-------------------------------------------------|
| 32  | DS cuộc trò chuyện        | `GET`     | `/api/conversations`                      | Lấy danh sách cuộc trò chuyện của tôi           |
| 33  | Chi tiết cuộc trò chuyện  | `GET`     | `/api/conversations/{id}`                 | Lấy thông tin một cuộc trò chuyện               |
| 34  | Tạo nhóm chat             | `POST`    | `/api/conversations`                      | Tạo cuộc trò chuyện nhóm mới                    |
| 35  | Chat 1-1                  | `POST`    | `/api/conversations/direct/{otherUserId}` | Lấy hoặc tạo hội thoại 1-1 với người dùng       |
| 36  | Tắt/bật thông báo         | `POST`    | `/api/conversations/{id}/mute`            | Tắt hoặc bật thông báo cuộc trò chuyện          |
| 37  | Lưu trữ/khôi phục         | `POST`    | `/api/conversations/{id}/archive`         | Lưu trữ hoặc khôi phục cuộc trò chuyện          |
| 38  | Thêm thành viên           | `POST`    | `/api/conversations/{id}/members`         | Thêm danh sách thành viên vào nhóm              |
| 39  | Xóa thành viên            | `DELETE`  | `/api/conversations/{id}/members/{userId}`| Xóa một thành viên khỏi nhóm                    |
| 40  | DS tin nhắn               | `GET`     | `/api/conversations/{id}/messages`        | Lấy tin nhắn phân trang trong cuộc trò chuyện   |
| 41  | Gửi tin nhắn              | `POST`    | `/api/conversations/{id}/messages`        | Gửi tin nhắn vào cuộc trò chuyện                |
| 42  | Đánh dấu đã đọc           | `POST`    | `/api/conversations/{id}/read`            | Đánh dấu đã đọc toàn bộ tin nhắn                |
|-----|---------------------------|-----------|-------------------------------------------|-------------------------------------------------|

### ✉️ Messages — Quản lý tin nhắn (`/api/messages`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 43  | Sửa tin nhắn            | `PUT`     | `/api/messages/{id}`              | Chỉnh sửa nội dung tin nhắn                     |
| 44  | Xóa tin nhắn            | `DELETE`  | `/api/messages/{id}`              | Xóa tin nhắn (soft delete)                      |
| 45  | Thêm/bỏ reaction        | `POST`    | `/api/messages/{id}/reactions`    | Toggle emoji reaction cho tin nhắn              |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### ✅ Tasks — Quản lý công việc (`/api/tasks`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 46  | DS công việc            | `GET`     | `/api/tasks`                      | Lấy danh sách công việc có phân trang & lọc     |
| 47  | Chi tiết công việc      | `GET`     | `/api/tasks/{id}`                 | Lấy thông tin một công việc                     |
| 48  | Tạo công việc           | `POST`    | `/api/tasks`                      | Tạo công việc mới                               |
| 49  | Cập nhật công việc      | `PUT`     | `/api/tasks/{id}`                 | Cập nhật thông tin công việc                    |
| 50  | Xóa công việc           | `DELETE`  | `/api/tasks/{id}`                 | Xóa (vô hiệu hóa) công việc                     |
| 51  | DS bình luận            | `GET`     | `/api/tasks/{id}/comments`        | Lấy danh sách bình luận của công việc           |
| 52  | Thêm bình luận          | `POST`    | `/api/tasks/{id}/comments`        | Thêm bình luận vào công việc                    |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 🔔 Notifications — Thông báo người dùng (`/api/notifications`)

|-----|-----------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                     | Method    | URL                               | Chức năng                                       |
|-----|-----------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 53  | DS thông báo của tôi        | `GET`     | `/api/notifications`              | Lấy danh sách thông báo (phân trang, lọc đọc)   |
| 54  | Số thông báo chưa đọc       | `GET`     | `/api/notifications/unread-count` | Đếm số thông báo chưa đọc                       |
| 55  | Đánh dấu đã đọc 1 thông báo | `PATCH`   | `/api/notifications/{id}/read`    | Đánh dấu một thông báo là đã đọc                |
| 56  | Đánh dấu tất cả đã đọc      | `PATCH`   | `/api/notifications/read-all`     | Đánh dấu toàn bộ thông báo là đã đọc            |
|-----|-----------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 📋 NotificationEventTypes — Loại sự kiện thông báo (`/api/notification-event-types`)

|-----|-------------------------|-----------|----------------------------------------------------------------|------------------------------------------|
| #   | Tên API                 | Method    | URL                                                            | Chức năng                                |
|-----|-------------------------|-----------|----------------------------------------------------------------|------------------------------------------|
| 57  | DS loại sự kiện         | `GET`     | `/api/notification-event-types`                                | Lấy danh sách loại sự kiện (phân trang)  |
| 58  | Chi tiết loại sự kiện   | `GET`     | `/api/notification-event-types/{id}`                           | Lấy thông tin một loại sự kiện           |
| 59  | Tạo loại sự kiện        | `POST`    | `/api/notification-event-types`                                | Tạo loại sự kiện mới                     |
| 60  | Cập nhật loại sự kiện   | `PUT`     | `/api/notification-event-types/{id}`                           | Cập nhật thông tin loại sự kiện          |
| 61  | Xóa loại sự kiện        | `DELETE`  | `/api/notification-event-types/{id}`                           | Xóa (vô hiệu hóa) loại sự kiện           |
| 62  | DS template theo sự kiện| `GET`     | `/api/notification-event-types/{eventTypeId}/templates`        | Lấy danh sách template theo loại sự kiện |
| 63  | Tạo/cập nhật template   | `PUT`     | `/api/notification-event-types/{eventTypeId}/templates/{channel}` | Upsert template thông báo theo kênh   |
|-----|-------------------------|-----------|----------------------------------------------------------------|------------------------------------------|

### ⚙️ NotificationTriggers — Cấu hình trigger thông báo (`/api/notification-triggers`)

|-----|-------------------------------|-----------|-------------------------------------------|-------------------------------------------------|
| #   | Tên API                       | Method    | URL                                       | Chức năng                                       |
|-----|-------------------------------|-----------|-------------------------------------------|-------------------------------------------------|
| 64  | DS trigger                    | `GET`     | `/api/notification-triggers`              | Lấy danh sách trigger (phân trang, lọc module)  |
| 65  | Chi tiết trigger              | `GET`     | `/api/notification-triggers/{triggerKey}` | Lấy thông tin trigger theo key                  |
| 66  | Cập nhật binding trigger      | `PUT`     | `/api/notification-triggers/{triggerKey}` | Cập nhật EventType và người nhận của trigger    |
|-----|-------------------------------|-----------|-------------------------------------------|-------------------------------------------------|

### 🔑 Permissions — Quản lý quyền hệ thống (`/api/permissions`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 67  | DS quyền                | `GET`     | `/api/permissions`                | Lấy danh sách tất cả quyền (phân trang)         |
| 68  | Chi tiết quyền          | `GET`     | `/api/permissions/{id}`           | Lấy thông tin một quyền theo ID                 |
| 69  | Tạo quyền               | `POST`    | `/api/permissions`                | Tạo quyền mới                                   |
| 70  | Cập nhật quyền          | `PUT`     | `/api/permissions/{id}`           | Cập nhật thông tin quyền                        |
| 71  | Xóa quyền               | `DELETE`  | `/api/permissions/{id}`           | Xóa (vô hiệu hóa) quyền                         |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

### 📍 Attendances — Chấm công (`/api/attendances`)

|-----|-------------------------------|-----------|------------------------------------------------------|---------------------------------------------------|
| #   | Tên API                       | Method    | URL                                                  | Chức năng                                         |
|-----|-------------------------------|-----------|------------------------------------------------------|---------------------------------------------------|
| 72  | Tạo vị trí chấm công          | `POST`    | `/api/attendances/locations`                         | Tạo vị trí chấm công mới                          |
| 73  | Cập nhật vị trí chấm công     | `PUT`     | `/api/attendances/locations/{id}`                    | Cập nhật thông tin vị trí                         |
| 74  | Chi tiết vị trí chấm công     | `GET`     | `/api/attendances/locations/{id}`                    | Lấy thông tin một vị trí chấm công               |
| 75  | DS vị trí chấm công           | `GET`     | `/api/attendances/locations`                         | Lấy danh sách vị trí chấm công (phân trang)      |
| 76  | Chấm công (vào/ra)            | `POST`    | `/api/attendances/check-in?latitude=&longitude=`     | Chấm công vào hoặc ra, tọa độ qua query string   |
| 77  | Trạng thái chấm công hôm nay  | `GET`     | `/api/attendances/today`                             | Lấy trạng thái chấm công hôm nay của bản thân   |
| 78  | Lịch sử chấm công             | `GET`     | `/api/attendances/logs`                              | Lấy log chấm công (phân trang, lọc theo ngày)    |
|-----|-------------------------------|-----------|------------------------------------------------------|---------------------------------------------------|

### 🏥 System — Health Check (`/api/health`)

|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                 | Method    | URL                               | Chức năng                                       |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 79  | Health check            | `GET`     | `/api/health`                     | Kiểm tra trạng thái hoạt động của API           |
|-----|-------------------------|-----------|-----------------------------------|-------------------------------------------------|

---

## Chi tiết từng API

### Mục lục chi tiết

1. [Auth — Xác thực](#1-auth--xác-thực)
2. [Users — Quản lý người dùng](#2-users--quản-lý-người-dùng)
3. [Departments — Quản lý phòng ban](#3-departments--quản-lý-phòng-ban)
4. [Roles — Quản lý vai trò](#4-roles--quản-lý-vai-trò)
5. [JobLevels — Quản lý cấp bậc chức danh](#5-joblevels--quản-lý-cấp-bậc-chức-danh)
6. [Conversations — Quản lý cuộc trò chuyện](#6-conversations--quản-lý-cuộc-trò-chuyện)
7. [Messages — Quản lý tin nhắn](#7-messages--quản-lý-tin-nhắn)
8. [Tasks — Quản lý công việc](#8-tasks--quản-lý-công-việc)
9. [Notifications — Thông báo người dùng](#9-notifications--thông-báo-người-dùng)
10. [NotificationEventTypes — Loại sự kiện thông báo](#10-notificationeventtypes--loại-sự-kiện-thông-báo)
11. [NotificationTriggers — Cấu hình trigger](#11-notificationtriggers--cấu-hình-trigger-thông-báo)
12. [Permissions — Quản lý quyền hệ thống](#12-permissions--quản-lý-quyền-hệ-thống)
13. [Attendances — Chấm công](#13-attendances--chấm-công)
14. [System — Health Check](#14-system--health-check)

---

## 1. Auth — Xác thực

Base URL: `/api/auth`

---

### 1.1. Đăng nhập

|-------------|---------------------------------------------------------------|
| **Method**  | `POST`                                                        |
| **URL**     | `/api/auth/login`                                             |
| **Auth**    | Không yêu cầu                                                 |
| **Mô tả**   | Đăng nhập vào hệ thống, trả về access token và refresh token  |
|-------------|---------------------------------------------------------------|

**Request:**

```json
{
  "email": "admin1@digifnb.com",
  "password": "123456"
}
```

**Response (200):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiry": "2026-06-03T08:00:00Z",
  "employeeCode": "NV001",
  "fullName": "Nguyễn Văn A",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4=...",
  "refreshTokenExpiry": "2026-07-03T07:00:00Z"
}
```

---

### 1.2. Làm mới token

|-------------|---------------------------------------------------------------|
| **Method**  | `POST`                                                        |
| **URL**     | `/api/auth/refresh`                                           |
| **Auth**    | Không yêu cầu                                                 |
| **Mô tả**   | Lấy access token mới khi token cũ hết hạn                     |
|-------------|---------------------------------------------------------------|

**Request:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4=..."
}
```

**Response (200):** Giống response của API đăng nhập.

---

### 1.3. Thu hồi token

|-------------|---------------------------------------------------------------|
| **Method**  | `POST`                                                        |
| **URL**     | `/api/auth/revoke`                                            |
| **Auth**    | `Bearer Token`                                                |
| **Mô tả**   | Thu hồi refresh token (dùng khi logout)                       |
|-------------|---------------------------------------------------------------|

**Request:**

```json
{
  "refreshToken": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4=..."
}
```

**Response:** `204 No Content`

---

### 1.4. Lấy thông tin người dùng hiện tại

|-------------|---------------------------------------------------------------|
| **Method**  | `GET`                                                         |
| **URL**     | `/api/auth/me`                                                |
| **Auth**    | `Bearer Token`                                                |
| **Mô tả**   | Lấy thông tin người dùng đang đăng nhập                       |
|-------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "employeeCode": "NV001",
  "fullName": "Nguyễn Văn A",
  "email": "admin1@digifnb.com",
  "avatarUrl": "https://blob.example.com/avatar.jpg",
  "departmentId": "...",
  "departmentName": "Phòng Nhân sự",
  "jobLevelId": "...",
  "jobLevelName": "Manager",
  "managerId": null,
  "managerName": null,
  "dateOfJoin": "2024-01-15",
  "status": 1,
  "isActive": true,
  "roles": ["ROLE_HR_ADMIN", "ROLE_EMPLOYEE"]
}
```

---

## 2. Users — Quản lý người dùng

Base URL: `/api/users`

---

### 2.1. Lấy danh sách người dùng

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/users`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.read`                                           |
| **Mô tả**       | Lấy danh sách tất cả người dùng                               |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "employeeCode": "NV001",
    "fullName": "Nguyễn Văn A",
    "email": "a@company.com",
    "avatarUrl": null,
    "departmentId": "...",
    "departmentName": "Phòng IT",
    "jobLevelId": "...",
    "jobLevelName": "Staff",
    "managerId": "...",
    "managerName": "Trần Văn B",
    "dateOfJoin": "2024-01-15",
    "status": 1,
    "isActive": true,
    "roles": ["ROLE_EMPLOYEE"]
  }
]
```

---

### 2.2. Lấy thông tin người dùng theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/users/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.read`                                           |
| **Mô tả**       | Lấy chi tiết thông tin một người dùng                         |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body. `id` truyền trên URL (GUID).

**Response (200):** Giống một phần tử trong response của API 2.1.

---

### 2.3. Tạo người dùng mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/users`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.create`                                         |
| **Mô tả**       | Tạo người dùng mới, trả về thông tin vừa tạo                  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "employeeCode": "NV002",
  "fullName": "Lê Thị C",
  "email": "c@company.com",
  "departmentId": "3fa85f64-...",
  "jobLevelId": "3fa85f64-...",
  "dateOfJoin": "2026-06-01",
  "status": 1,
  "managerId": "3fa85f64-...",
  "avatarUrl": null,
  "password": "matkhau123"
}
```

> `managerId`, `avatarUrl`, `password` là tùy chọn (nullable).  
> `status`: `1` = Active, `2` = Probation, `3` = Resigned, `4` = Terminated, `5` = Suspended, `6` = MaternityLeave

**Response (201):** Giống response của API 2.2.

---

### 2.4. Cập nhật thông tin người dùng

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/users/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.update`                                         |
| **Mô tả**       | Cập nhật thông tin hồ sơ người dùng                           |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "fullName": "Lê Thị C (updated)",
  "email": "c.new@company.com",
  "departmentId": "3fa85f64-...",
  "jobLevelId": "3fa85f64-...",
  "dateOfJoin": "2026-06-01",
  "status": 1,
  "managerId": null,
  "avatarUrl": null
}
```

**Response (200):** Giống response của API 2.2.

---

### 2.5. Xóa người dùng

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/users/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.delete`                                         |
| **Mô tả**       | Xóa (vô hiệu hóa) người dùng                                  |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response:** `204 No Content`

---

### 2.6. Gán vai trò cho người dùng

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/users/{id}/roles`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.assign`                                          |
| **Mô tả**       | Gán danh sách vai trò cho người dùng                          |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
[
  "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "9a1b2c3d-4e5f-6789-abcd-ef0123456789"
]
```

> Mảng GUID của các Role cần gán.

**Response (200):**

```json
{
  "message": "Gán vai trò thành công."
}
```

---

### 2.7. Đặt lại mật khẩu

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/users/{id}/reset-password`                              |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.user.resetpassword`                                   |
| **Mô tả**       | Đặt lại mật khẩu cho người dùng (tối thiểu 6 ký tự)           |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
"matkhaumoi123"
```

> Body là chuỗi JSON string trực tiếp.

**Response (200):**

```json
{
  "message": "Đặt lại mật khẩu thành công."
}
```

---

### 2.8. Lấy danh sách phòng ban kiêm nhiệm

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/users/{id}/departments`                                 |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.read`                                           |
| **Mô tả**       | Lấy danh sách các phòng ban kiêm nhiệm của nhân sự            |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "userId": "3fa85f64-...",
    "departmentId": "3fa85f64-...",
    "departmentName": "Phòng Kế toán",
    "departmentCode": "ACC",
    "isPrimary": false,
    "startDate": "2026-06-01",
    "endDate": null,
    "isActive": true
  }
]
```

---

### 2.9. Gán phòng ban kiêm nhiệm mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/users/{id}/departments`                                 |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.update`                                         |
| **Mô tả**       | Gán phòng ban kiêm nhiệm mới cho nhân sự                      |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "startDate": "2026-06-01",
  "endDate": null
}
```

**Response (200):** Giống một phần tử trong danh sách phòng ban kiêm nhiệm.

---

### 2.10. Gỡ bỏ phòng ban kiêm nhiệm

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/users/{id}/departments/{departmentId}`                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.employee.update`                                         |
| **Mô tả**       | Kết thúc/Gỡ bỏ phòng ban kiêm nhiệm của nhân sự              |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

## 3. Departments — Quản lý phòng ban

Base URL: `/api/departments`

---

### 3.1. Lấy danh sách phòng ban

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/departments`                                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.department.read`                                         |
| **Mô tả**       | Lấy tất cả phòng ban                                          |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "departmentName": "Phòng Nhân sự",
    "departmentCode": "HR",
    "parentDepartmentId": null,
    "parentDepartmentName": null,
    "managerId": "3fa85f64-...",
    "managerName": "Nguyễn Văn A",
    "description": "Phòng quản lý nhân sự",
    "isActive": true
  }
]
```

---

### 3.2. Lấy thông tin phòng ban theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/departments/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.department.read`                                         |
| **Mô tả**       | Lấy chi tiết thông tin một phòng ban                          |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):** Giống một phần tử trong response của API 3.1.

---

### 3.3. Tạo phòng ban mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/departments`                                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.department.create`                                       |
| **Mô tả**       | Tạo phòng ban mới, trả về thông tin vừa tạo                   |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "departmentName": "Phòng Kỹ thuật",
  "departmentCode": "TECH",
  "parentDepartmentId": "3fa85f64-...",
  "managerId": "3fa85f64-...",
  "description": "Phòng kỹ thuật phần mềm"
}
```

> `parentDepartmentId`, `managerId`, `description` là tùy chọn.

**Response (201):** Giống response của API 3.2.

---

### 3.4. Cập nhật thông tin phòng ban

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/departments/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.department.update`                                       |
| **Mô tả**       | Cập nhật thông tin phòng ban                                  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "departmentName": "Phòng Kỹ thuật (updated)",
  "departmentCode": "TECH",
  "parentDepartmentId": null,
  "managerId": "3fa85f64-...",
  "description": "Mô tả mới",
  "isActive": true
}
```

**Response (200):** Giống response của API 3.2.

---

### 3.5. Xóa phòng ban

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/departments/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.department.delete`                                       |
| **Mô tả**       | Xóa (vô hiệu hóa - IsActive = false) phòng ban                |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

## 4. Roles — Quản lý vai trò

Base URL: `/api/roles`

---

### 4.1. Lấy danh sách vai trò

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/roles`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.read`                                            |
| **Mô tả**       | Lấy tất cả vai trò trong hệ thống                             |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "roleName": "ROLE_HR_ADMIN",
    "displayName": "Quản trị Nhân sự",
    "description": "Quản lý toàn bộ module HR",
    "isSystemRole": true,
    "bypassDataScope": false,
    "isActive": true,
    "permissions": [
      {
        "id": "...",
        "permissionCode": "hrm.employee.read",
        "permissionName": "Xem nhân viên",
        "module": 1,
        "action": 2,
        "resource": "employee",
        "description": null,
        "isActive": true
      }
    ]
  }
]
```

---

### 4.2. Lấy thông tin vai trò theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/roles/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.read`                                            |
| **Mô tả**       | Lấy chi tiết thông tin một vai trò                            |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):** Giống một phần tử trong response của API 4.1.

---

### 4.3. Lấy danh sách tất cả quyền

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/roles/permissions`                                      |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.read`                                            |
| **Mô tả**       | Lấy danh sách tất cả quyền để gán cho vai trò                 |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "permissionCode": "hrm.employee.read",
    "permissionName": "Xem nhân viên",
    "module": 1,
    "action": 2,
    "resource": "employee",
    "description": null,
    "isActive": true
  }
]
```

---

### 4.4. Tạo vai trò mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/roles`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.create`                                          |
| **Mô tả**       | Tạo vai trò mới, trả về thông tin vừa tạo                     |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "roleName": "ROLE_ACCOUNTANT",
  "displayName": "Kế toán",
  "description": "Vai trò dành cho bộ phận kế toán",
  "bypassDataScope": false
}
```

> `description` là tùy chọn. `bypassDataScope` mặc định là `false`.

**Response (201):** Giống response của API 4.2.

---

### 4.5. Cập nhật quyền cho vai trò

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/roles/{id}/permissions`                                 |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.update`                                          |
| **Mô tả**       | Cập nhật danh sách quyền cho vai trò (thay thế toàn bộ)       |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "permissionIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "9a1b2c3d-4e5f-6789-abcd-ef0123456789"
  ]
}
```

**Response (200):**

```json
{
  "message": "Cập nhật danh sách quyền cho vai trò thành công."
}
```

---

### 4.6. Cập nhật thông tin vai trò

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/roles/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.update`                                          |
| **Mô tả**       | Cập nhật thông tin hiển thị cơ bản của vai trò                |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "displayName": "Quản trị Nhân sự (updated)",
  "description": "Quản lý toàn bộ nhân viên và lương",
  "bypassDataScope": false,
  "isActive": true
}
```

**Response (200):** Giống response của API 4.2.

---

### 4.7. Xóa vai trò

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/roles/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.role.update`                                          |
| **Mô tả**       | Xóa (vô hiệu hóa - IsActive = false) vai trò                  |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

## 5. JobLevels — Quản lý cấp bậc chức danh

Base URL: `/api/job-levels`

---

### 5.1. Lấy danh sách cấp bậc chức danh

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/job-levels`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.joblevel.read`                                           |
| **Mô tả**       | Lấy danh sách tất cả các cấp bậc chức danh                    |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "levelName": "Manager",
    "levelOrder": 2,
    "defaultScopeType": 3,
    "description": "Manager level with department scope.",
    "baseSalaryMin": null,
    "baseSalaryMax": null,
    "isActive": true
  }
]
```

---

### 5.2. Lấy thông tin cấp bậc theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/job-levels/{id}`                                        |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.joblevel.read`                                           |
| **Mô tả**       | Lấy chi tiết thông tin một cấp bậc chức danh                  |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong response của API 5.1.

---

### 5.3. Tạo cấp bậc chức danh mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/job-levels`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.joblevel.create`                                         |
| **Mô tả**       | Tạo cấp bậc chức danh mới                                     |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "levelName": "Senior Manager",
  "levelOrder": 2,
  "defaultScopeType": 3,
  "description": "Senior Manager level",
  "baseSalaryMin": 20000000,
  "baseSalaryMax": 40000000
}
```

**Response (201):** Giống response của API 5.2.

---

### 5.4. Cập nhật cấp bậc chức danh

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/job-levels/{id}`                                        |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.joblevel.update`                                         |
| **Mô tả**       | Cập nhật thông tin cấp bậc chức danh                          |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "levelName": "Senior Manager (updated)",
  "levelOrder": 2,
  "defaultScopeType": 3,
  "description": "Updated Senior Manager level",
  "baseSalaryMin": 25000000,
  "baseSalaryMax": 50000000,
  "isActive": true
}
```

**Response (200):** Giống response của API 5.2.

---

### 5.5. Xóa cấp bậc chức danh

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/job-levels/{id}`                                        |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `hrm.joblevel.delete`                                         |
| **Mô tả**       | Xóa (vô hiệu hóa - IsActive = false) cấp bậc chức danh        |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

## 6. Conversations — Quản lý cuộc trò chuyện

Base URL: `/api/conversations`

> **SignalR Hub:** `/hubs/chat` — Client kết nối để nhận sự kiện realtime.

---

### 6.1. Lấy danh sách cuộc trò chuyện

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/conversations`                                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.read`                                      |
| **Mô tả**       | Lấy danh sách cuộc trò chuyện mà người dùng hiện tại tham gia |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "conversationType": 1,
    "title": "Nhóm IT",
    "description": null,
    "isPrivate": true,
    "isArchived": false,
    "createdBy": "3fa85f64-...",
    "createdAt": "2026-06-10T08:00:00Z",
    "updatedAt": null,
    "members": [
      {
        "userID": "3fa85f64-...",
        "fullName": "Nguyễn Văn A",
        "employeeCode": "NV001",
        "avatarUrl": null,
        "roleInConversation": 1,
        "joinedAt": "2026-06-10T08:00:00Z",
        "isMuted": false,
        "isActive": true
      }
    ],
    "lastMessage": null,
    "unreadCount": 0
  }
]
```

> `conversationType`: `1` = Direct, `2` = Group  
> `roleInConversation`: `1` = Member, `2` = Admin

---

### 6.2. Lấy thông tin cuộc trò chuyện theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/conversations/{id}`                                     |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.read`                                      |
| **Mô tả**       | Lấy chi tiết một cuộc trò chuyện                              |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong response của API 6.1.

---

### 6.3. Tạo cuộc trò chuyện nhóm

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations`                                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.create`                                    |
| **Mô tả**       | Tạo cuộc trò chuyện nhóm mới                                  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "conversationType": 2,
  "title": "Nhóm dự án ABC",
  "description": "Kênh trao đổi dự án ABC",
  "isPrivate": true,
  "memberIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "9a1b2c3d-4e5f-6789-abcd-ef0123456789"
  ]
}
```

**Response (201):** Giống response của API 6.2.

---

### 6.4. Lấy hoặc tạo hội thoại 1-1

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/direct/{otherUserId}`                     |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.create`                                    |
| **Mô tả**       | Lấy hội thoại 1-1 đã có, hoặc tạo mới nếu chưa tồn tại       |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body. `otherUserId` truyền trên URL (GUID).

**Response (200):** Giống response của API 6.2.

---

### 6.5. Tắt/bật thông báo cuộc trò chuyện

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/{id}/mute`                                |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.update`                                    |
| **Mô tả**       | Tắt hoặc bật thông báo cho cuộc trò chuyện                    |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
true
```

> Body là JSON boolean (`true` = tắt thông báo, `false` = bật thông báo).

**Response (200):**

```json
{ "message": "Cập nhật tắt/bật thông báo thành công." }
```

---

### 6.6. Lưu trữ/khôi phục cuộc trò chuyện

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/{id}/archive`                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.update`                                    |
| **Mô tả**       | Lưu trữ hoặc khôi phục cuộc trò chuyện                        |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
true
```

> Body là JSON boolean (`true` = lưu trữ, `false` = khôi phục).

**Response (200):**

```json
{ "message": "Đã lưu trữ cuộc trò chuyện." }
```

---

### 6.7. Thêm thành viên vào nhóm

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/{id}/members`                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.member.manage`                                          |
| **Mô tả**       | Thêm một hoặc nhiều thành viên vào nhóm chat                  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
[
  "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "9a1b2c3d-4e5f-6789-abcd-ef0123456789"
]
```

> Mảng GUID của các user cần thêm.

**Response (200):**

```json
{ "message": "Thêm thành viên thành công." }
```

---

### 6.8. Xóa thành viên khỏi nhóm

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/conversations/{id}/members/{userId}`                    |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.member.manage`                                          |
| **Mô tả**       | Xóa một thành viên khỏi nhóm chat                             |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

### 6.9. Lấy tin nhắn phân trang

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/conversations/{id}/messages`                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.conversation.read`                                      |
| **Mô tả**       | Lấy danh sách tin nhắn trong cuộc trò chuyện (phân trang)     |
|-----------------|---------------------------------------------------------------|

**Query Parameters:**

| Tham số    | Kiểu  | Mặc định | Mô tả                  |
|------------|-------|----------|------------------------|
| `page`     | `int` | `1`      | Số trang               |
| `pageSize` | `int` | `50`     | Số tin nhắn mỗi trang  |

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "conversationID": "3fa85f64-...",
    "userID": "3fa85f64-...",
    "fullName": "Nguyễn Văn A",
    "avatarUrl": null,
    "content": "Xin chào!",
    "messageType": 1,
    "parentMessageID": null,
    "isEdited": false,
    "isDeleted": false,
    "sentAt": "2026-06-16T08:00:00Z",
    "editedAt": null,
    "attachments": [],
    "reactions": [],
    "linkedTaskId": null
  }
]
```

> `messageType`: `1` = Text, `2` = Image, `3` = File, `4` = System

---

### 6.10. Gửi tin nhắn

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/{id}/messages`                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.message.create`                                         |
| **Mô tả**       | Gửi tin nhắn vào cuộc trò chuyện. Broadcast qua SignalR event `ReceiveMessage` |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "content": "Xin chào mọi người!",
  "messageType": 1,
  "parentMessageID": null,
  "attachments": [
    {
      "fileName": "report.pdf",
      "fileURL": "https://blob.example.com/report.pdf",
      "fileType": "application/pdf",
      "fileSize": 204800,
      "thumbnailURL": null
    }
  ]
}
```

> `attachments` là tùy chọn. `parentMessageID` dùng để trả lời tin nhắn.

**Response (200):** Giống một phần tử trong response của API 6.9.

---

### 6.11. Đánh dấu đã đọc

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/conversations/{id}/read`                                |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.message.create`                                         |
| **Mô tả**       | Đánh dấu toàn bộ tin nhắn trong cuộc trò chuyện là đã đọc    |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):**

```json
{ "message": "Đã đánh dấu đã đọc cuộc hội thoại." }
```

---

## 7. Messages — Quản lý tin nhắn

Base URL: `/api/messages`

---

### 7.1. Sửa tin nhắn

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/messages/{id}`                                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.message.update`                                         |
| **Mô tả**       | Chỉnh sửa nội dung tin nhắn. Broadcast qua SignalR event `MessageEdited` |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
"Nội dung tin nhắn đã chỉnh sửa"
```

> Body là chuỗi JSON string trực tiếp.

**Response (200):** Giống response của API 6.9 (một tin nhắn).

---

### 7.2. Xóa tin nhắn

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/messages/{id}`                                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.message.delete`                                         |
| **Mô tả**       | Xóa mềm tin nhắn (IsDeleted = true). Broadcast qua SignalR event `MessageDeleted` |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response:** `204 No Content`

---

### 7.3. Toggle emoji reaction

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/messages/{id}/reactions`                                |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `chat.message.create`                                         |
| **Mô tả**       | Thêm hoặc bỏ emoji reaction cho tin nhắn. Broadcast qua SignalR event `ReactionToggled` |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
"👍"
```

> Body là chuỗi emoji hoặc tên reaction (JSON string).

**Response (200):**

```json
{
  "id": "3fa85f64-...",
  "messageID": "3fa85f64-...",
  "userID": "3fa85f64-...",
  "fullName": "Nguyễn Văn A",
  "reactionType": "👍"
}
```

> Nếu user đã react cùng loại → bỏ reaction (toggle). Nếu chưa → thêm mới.

---

## 8. Tasks — Quản lý công việc

Base URL: `/api/tasks`

---

### 8.1. Lấy danh sách công việc

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/tasks`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.read`                                              |
| **Mô tả**       | Lấy danh sách công việc có phân trang và bộ lọc               |
|-----------------|---------------------------------------------------------------|

**Query Parameters (`TaskQuery`):**

| Tham số    | Kiểu     | Mô tả                                    |
|------------|----------|------------------------------------------|
| `page`     | `int`    | Số trang (mặc định: 1)                   |
| `pageSize` | `int`    | Số bản ghi mỗi trang (mặc định: 20)      |
| `status`   | `int?`   | Lọc theo trạng thái (xem Enum bên dưới)  |
| `priority` | `int?`   | Lọc theo độ ưu tiên                      |
| `assignee` | `Guid?`  | Lọc theo người được giao                 |

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-...",
      "taskCode": "TASK-001",
      "title": "Phân tích yêu cầu module Chat",
      "description": "Phân tích và thiết kế API Chat",
      "taskType": 1,
      "status": 1,
      "priority": 2,
      "progress": 0,
      "startDate": "2026-06-10T00:00:00Z",
      "dueDate": "2026-06-20T00:00:00Z",
      "completedDate": null,
      "estimatedHours": 8.0,
      "actualHours": null,
      "isRecurring": false,
      "recurringPattern": null,
      "parentTaskID": null,
      "createdBy": "3fa85f64-...",
      "createdAt": "2026-06-10T08:00:00Z",
      "updatedAt": null,
      "assignees": [],
      "followers": [],
      "comments": [],
      "activityLogs": [],
      "kpiIds": [],
      "lmsCourseIds": [],
      "subtasks": []
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### 8.2. Lấy thông tin công việc theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/tasks/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.read`                                              |
| **Mô tả**       | Lấy chi tiết một công việc                                    |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong `items` của response API 8.1.

---

### 8.3. Tạo công việc mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/tasks`                                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.create`                                            |
| **Mô tả**       | Tạo công việc mới                                             |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "title": "Phân tích yêu cầu module Chat",
  "description": "Phân tích và thiết kế API Chat",
  "taskType": 1,
  "priority": 2,
  "startDate": "2026-06-10T00:00:00Z",
  "dueDate": "2026-06-20T00:00:00Z",
  "estimatedHours": 8.0,
  "isRecurring": false,
  "recurringPattern": null,
  "parentTaskID": null,
  "assigneeIds": ["3fa85f64-..."],
  "primaryAssigneeId": "3fa85f64-...",
  "followerIds": [],
  "kpiIds": [],
  "lmsCourseIds": []
}
```

> `description`, `startDate`, `dueDate`, `estimatedHours`, `parentTaskID`, `primaryAssigneeId` là tùy chọn.

**Response (201):** Giống response của API 8.2.

---

### 8.4. Cập nhật công việc

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/tasks/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.update`                                            |
| **Mô tả**       | Cập nhật thông tin, trạng thái, tiến độ công việc             |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "title": "Phân tích yêu cầu module Chat (updated)",
  "description": "Đã hoàn thành phân tích",
  "taskType": 1,
  "status": 3,
  "priority": 2,
  "progress": 100,
  "startDate": "2026-06-10T00:00:00Z",
  "dueDate": "2026-06-20T00:00:00Z",
  "estimatedHours": 8.0,
  "actualHours": 7.5,
  "isRecurring": false,
  "recurringPattern": null,
  "parentTaskID": null
}
```

**Response (200):** Giống response của API 8.2.

---

### 8.5. Xóa công việc

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/tasks/{id}`                                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.delete`                                            |
| **Mô tả**       | Xóa (vô hiệu hóa) công việc                                   |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response:** `204 No Content`

---

### 8.6. Lấy danh sách bình luận

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/tasks/{id}/comments`                                    |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.item.read`                                              |
| **Mô tả**       | Lấy danh sách bình luận của một công việc                     |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "taskID": "3fa85f64-...",
    "userID": "3fa85f64-...",
    "fullName": "Nguyễn Văn A",
    "avatarUrl": null,
    "content": "Đã hoàn thành phần thiết kế.",
    "parentCommentID": null,
    "createdAt": "2026-06-16T08:00:00Z",
    "replies": []
  }
]
```

---

### 8.7. Thêm bình luận

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/tasks/{id}/comments`                                    |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `task.comment.create`                                         |
| **Mô tả**       | Thêm bình luận vào công việc (hỗ trợ reply thread)            |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "content": "Đã hoàn thành phần thiết kế.",
  "parentCommentID": null
}
```

> `parentCommentID` là tùy chọn — dùng để reply bình luận khác.

**Response (200):** Giống một phần tử trong response của API 8.6.

---

## Quy ước chung

### Authentication

Tất cả API (trừ `login` và `refresh`) yêu cầu gửi header:

```
Authorization: Bearer <access_token>
```

### Error Response

Khi có lỗi, API trả về format:

```json
{
  "statusCode": 404,
  "message": "Không tìm thấy tài nguyên."
}
```

|-------------|---------------------------------------------------------------|
| HTTP Status | Ý nghĩa                                                       |
|-------------|---------------------------------------------------------------|
| `400`       | Bad Request — dữ liệu đầu vào không hợp lệ                    |
| `401`       | Unauthorized — chưa đăng nhập hoặc token hết hạn              |
| `403`       | Forbidden — không có quyền truy cập                           |
| `404`       | Not Found — không tìm thấy tài nguyên                         |
| `409`       | Conflict — lỗi nghiệp vụ (sai mật khẩu, tài khoản bị khóa...) |
|-------------|---------------------------------------------------------------|

### Enum Values

**UserStatus:**

|---------|----------------|----------------|
| Giá trị | Tên            | Mô tả          |
|---------|----------------|----------------|
| `1`     | Active         | Đang làm việc  |
| `2`     | Probation      | Thử việc       |
| `3`     | Resigned       | Đã nghỉ việc   |
| `4`     | Terminated     | Bị chấm dứt HĐ |
| `5`     | Suspended      | Tạm đình chỉ   |
| `6`     | MaternityLeave | Nghỉ thai sản  |
|---------|----------------|----------------|

**PermissionModule:**

|---------|-------------|
| Giá trị | Tên         |
|---------|-------------|
| `1`     | Hrm         |
| `2`     | Lms         |
| `3`     | Payroll     |
| `4`     | Task        |
| `5`     | Chat        |
| `6`     | Report      |
| `7`     | System      |
| `8`     | Kpi         |
| `9`     | Onboarding  |
|---------|-------------|

**PermissionAction:**

|---------|----------------|
| Giá trị | Tên            |
|---------|----------------|
| `1`     | Create         |
| `2`     | Read           |
| `3`     | Update         |
| `4`     | Delete         |
| `5`     | Approve        |
| `6`     | Assign         |
| `7`     | Export         |
| `8`     | ManageTemplate |
|---------|----------------|

**TaskStatus:**

|---------|------------|
| Giá trị | Tên        |
|---------|------------|
| `1`     | Todo       |
| `2`     | InProgress |
| `3`     | Done       |
| `4`     | Cancelled  |
| `5`     | OnHold     |
|---------|------------|

**TaskPriority:**

|---------|----------|
| Giá trị | Tên      |
|---------|----------|
| `1`     | Low      |
| `2`     | Medium   |
| `3`     | High     |
| `4`     | Critical |
|---------|----------|

**TaskType:**

|---------|----------|
| Giá trị | Tên      |
|---------|----------|
| `1`     | Normal   |
| `2`     | Bug      |
| `3`     | Feature  |
| `4`     | Research |
|---------|----------|

**RecurringPattern:**

|---------|----------|
| Giá trị | Tên      |
|---------|----------|
| `1`     | Daily    |
| `2`     | Weekly   |
| `3`     | Monthly  |
|---------|----------|

**MessageType:**

|---------|--------|
| Giá trị | Tên    |
|---------|--------|
| `1`     | Text   |
| `2`     | Image  |
| `3`     | File   |
| `4`     | System |
|---------|--------|

**ConversationType:**

|---------|--------|
| Giá trị | Tên    |
|---------|--------|
| `1`     | Direct |
| `2`     | Group  |
|---------|--------|

**RoleInConversation:**

|---------|--------|
| Giá trị | Tên    |
|---------|--------|
| `1`     | Member |
| `2`     | Admin  |
|---------|--------|

### SignalR Hub — Chat Realtime

Client kết nối tới: `wss://<host>/hubs/chat`

| Sự kiện (Server → Client) | Dữ liệu               | Mô tả                                   |
|---------------------------|-----------------------|-----------------------------------------|
| `ReceiveMessage`          | `MessageDto`          | Nhận tin nhắn mới trong nhóm            |
| `MessageEdited`           | `MessageDto`          | Tin nhắn được chỉnh sửa                 |
| `MessageDeleted`          | `Guid` (messageId)    | Tin nhắn bị xóa                         |
| `ReactionToggled`         | `MessageReactionDto`  | Reaction được thêm hoặc bỏ              |

---

## 9. Notifications — Thông báo người dùng

Base URL: `/api/notifications`

---

### 9.1. Lấy danh sách thông báo

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notifications`                                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | Không yêu cầu quyền riêng (chỉ cần đăng nhập)               |
| **Mô tả**       | Lấy danh sách thông báo của người dùng hiện tại               |
|-----------------|---------------------------------------------------------------|

**Query Parameters:**

| Tham số    | Kiểu      | Mô tả                                         |
|------------|-----------|-----------------------------------------------|
| `page`     | `int`     | Số trang (mặc định: 1)                        |
| `pageSize` | `int`     | Số bản ghi mỗi trang (mặc định: 20)           |
| `isRead`   | `bool?`   | Lọc: `true` = đã đọc, `false` = chưa đọc, bỏ trống = tất cả |

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-...",
      "triggerKey": "hrm.employee.created",
      "eventTypeId": "3fa85f64-...",
      "title": "Nhân viên mới được tạo",
      "body": "Nguyễn Văn A vừa được thêm vào hệ thống.",
      "linkUrl": "/users/3fa85f64-...",
      "isRead": false,
      "readAt": null,
      "createdAt": "2026-06-16T08:00:00Z"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### 9.2. Lấy số thông báo chưa đọc

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notifications/unread-count`                             |
| **Auth**        | `Bearer Token`                                                |
| **Mô tả**       | Trả về số lượng thông báo chưa đọc của người dùng hiện tại   |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
{ "count": 3 }
```

---

### 9.3. Đánh dấu đã đọc một thông báo

|-----------------|---------------------------------------------------------------|
| **Method**      | `PATCH`                                                       |
| **URL**         | `/api/notifications/{id}/read`                                |
| **Auth**        | `Bearer Token`                                                |
| **Mô tả**       | Đánh dấu một thông báo cụ thể là đã đọc                      |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200):** Giống một phần tử trong `items` của API 9.1.

---

### 9.4. Đánh dấu tất cả đã đọc

|-----------------|---------------------------------------------------------------|
| **Method**      | `PATCH`                                                       |
| **URL**         | `/api/notifications/read-all`                                 |
| **Auth**        | `Bearer Token`                                                |
| **Mô tả**       | Đánh dấu toàn bộ thông báo của người dùng là đã đọc          |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response:** `204 No Content`

---

## 10. NotificationEventTypes — Loại sự kiện thông báo

Base URL: `/api/notification-event-types`

---

### 10.1. Lấy danh sách loại sự kiện

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notification-event-types`                               |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.read`                              |
| **Mô tả**       | Lấy danh sách loại sự kiện thông báo (phân trang)             |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-...",
      "eventCode": "hrm.employee.created",
      "name": "Nhân viên mới được tạo",
      "module": "Hrm",
      "description": "Kích hoạt khi tạo nhân viên mới",
      "defaultTitleTemplate": "Nhân viên mới: {{FullName}}",
      "defaultBodyTemplate": "{{FullName}} vừa được thêm vào hệ thống.",
      "isActive": true
    }
  ],
  "totalCount": 10,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### 10.2. Lấy thông tin loại sự kiện theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notification-event-types/{id}`                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.read`                              |
| **Mô tả**       | Lấy chi tiết một loại sự kiện                                 |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong `items` của API 10.1.

---

### 10.3. Tạo loại sự kiện mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/notification-event-types`                               |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.create`                            |
| **Mô tả**       | Tạo loại sự kiện thông báo mới                                |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "eventCode": "hrm.employee.resigned",
  "name": "Nhân viên nghỉ việc",
  "module": "Hrm",
  "description": "Kích hoạt khi nhân viên nghỉ việc",
  "defaultTitleTemplate": "Nhân viên nghỉ việc: {{FullName}}",
  "defaultBodyTemplate": "{{FullName}} đã nghỉ việc từ ngày {{Date}}."
}
```

**Response (201):** Giống response của API 10.2.

---

### 10.4. Cập nhật loại sự kiện

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/notification-event-types/{id}`                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.update`                            |
| **Mô tả**       | Cập nhật thông tin loại sự kiện thông báo                     |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "name": "Nhân viên nghỉ việc (updated)",
  "module": "Hrm",
  "description": "Mô tả mới",
  "defaultTitleTemplate": "Nghỉ việc: {{FullName}}",
  "defaultBodyTemplate": "{{FullName}} đã nghỉ việc.",
  "isActive": true
}
```

**Response (200):** Giống response của API 10.2.

---

### 10.5. Xóa loại sự kiện

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/notification-event-types/{id}`                          |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.delete`                            |
| **Mô tả**       | Xóa (vô hiệu hóa) loại sự kiện thông báo                     |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

### 10.6. Lấy danh sách template theo sự kiện

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notification-event-types/{eventTypeId}/templates`       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.read`                              |
| **Mô tả**       | Lấy danh sách template thông báo của một loại sự kiện         |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
[
  {
    "id": "3fa85f64-...",
    "eventTypeId": "3fa85f64-...",
    "channel": 1,
    "titleTemplate": "Nhân viên mới: {{FullName}}",
    "bodyTemplate": "{{FullName}} vừa được thêm.",
    "isActive": true
  }
]
```

> `channel`: `1` = InApp, `2` = Email, `3` = Push

---

### 10.7. Tạo hoặc cập nhật template

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/notification-event-types/{eventTypeId}/templates/{channel}` |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.event.update`                            |
| **Mô tả**       | Upsert template thông báo cho một kênh cụ thể                 |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "titleTemplate": "Tiêu đề: {{FullName}}",
  "bodyTemplate": "Nội dung: {{FullName}} đã {{Action}}.",
  "isActive": true
}
```

**Response (200):** Giống một phần tử trong response của API 10.6.

---

## 11. NotificationTriggers — Cấu hình trigger thông báo

Base URL: `/api/notification-triggers`

---

### 11.1. Lấy danh sách trigger

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notification-triggers`                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.trigger.read`                            |
| **Mô tả**       | Lấy danh sách tất cả trigger thông báo trong hệ thống         |
|-----------------|---------------------------------------------------------------|

**Query Parameters:**

| Tham số    | Kiểu      | Mô tả                                    |
|------------|-----------|------------------------------------------|
| `page`     | `int`     | Số trang                                 |
| `pageSize` | `int`     | Số bản ghi mỗi trang                     |
| `module`   | `string?` | Lọc theo tên module (vd: `Hrm`, `Task`)  |

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-...",
      "triggerKey": "hrm.employee.created",
      "name": "Tạo nhân viên mới",
      "module": "Hrm",
      "description": "Kích hoạt khi tạo nhân viên mới",
      "eventTypeId": "3fa85f64-...",
      "eventCode": "hrm.employee.created",
      "eventTypeName": "Nhân viên mới được tạo",
      "linkUrlTemplate": "/users/{{UserId}}",
      "recipientRules": {
        "includeSubjectUser": true,
        "includeActor": false,
        "includeSuperAdmins": true,
        "includeDepartmentManager": false,
        "roleIds": [],
        "userIds": []
      },
      "isActive": true
    }
  ],
  "totalCount": 20,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### 11.2. Lấy thông tin trigger theo key

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/notification-triggers/{triggerKey}`                     |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.trigger.read`                            |
| **Mô tả**       | Lấy chi tiết một trigger theo trigger key (string)            |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong `items` của API 11.1.

---

### 11.3. Cập nhật binding trigger

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/notification-triggers/{triggerKey}`                     |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.notification.trigger.update`                          |
| **Mô tả**       | Cập nhật EventType được gắn và cấu hình người nhận thông báo  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "eventTypeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "linkUrlTemplate": "/users/{{UserId}}",
  "recipientRules": {
    "includeSubjectUser": true,
    "includeActor": false,
    "includeSuperAdmins": false,
    "includeDepartmentManager": true,
    "roleIds": ["3fa85f64-..."],
    "userIds": []
  },
  "isActive": true
}
```

> `eventTypeId` và `linkUrlTemplate` là tùy chọn (nullable).

**Response (200):** Giống response của API 11.2.

---

## 12. Permissions — Quản lý quyền hệ thống

Base URL: `/api/permissions`

---

### 12.1. Lấy danh sách quyền

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/permissions`                                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.permission.read`                                      |
| **Mô tả**       | Lấy danh sách tất cả quyền trong hệ thống (phân trang)        |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-...",
      "permissionCode": "hrm.employee.read",
      "permissionName": "Xem nhân viên",
      "module": 1,
      "action": 2,
      "resource": "employee",
      "description": null,
      "isActive": true
    }
  ],
  "totalCount": 30,
  "page": 1,
  "pageSize": 20,
  "totalPages": 2
}
```

---

### 12.2. Lấy thông tin quyền theo ID

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/permissions/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.permission.read`                                      |
| **Mô tả**       | Lấy chi tiết một quyền                                        |
|-----------------|---------------------------------------------------------------|

**Response (200):** Giống một phần tử trong `items` của API 12.1.

---

### 12.3. Tạo quyền mới

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/permissions`                                            |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.permission.create`                                    |
| **Mô tả**       | Tạo quyền mới trong hệ thống                                  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "permissionCode": "hrm.employee.export",
  "permissionName": "Xuất danh sách nhân viên",
  "module": 1,
  "action": 7,
  "resource": "employee",
  "description": "Cho phép xuất Excel danh sách nhân viên"
}
```

> `description` là tùy chọn.

**Response (201):** Giống response của API 12.2.

---

### 12.4. Cập nhật quyền

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/permissions/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.permission.update`                                    |
| **Mô tả**       | Cập nhật tên hiển thị và mô tả của quyền                      |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "permissionName": "Xuất Excel nhân viên (updated)",
  "description": "Cập nhật mô tả",
  "isActive": true
}
```

**Response (200):** Giống response của API 12.2.

---

### 12.5. Xóa quyền

|-----------------|---------------------------------------------------------------|
| **Method**      | `DELETE`                                                      |
| **URL**         | `/api/permissions/{id}`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `system.permission.delete`                                    |
| **Mô tả**       | Xóa (vô hiệu hóa) quyền                                       |
|-----------------|---------------------------------------------------------------|

**Response:** `204 No Content`

---

## 13. Attendances — Chấm công

Base URL: `/api/attendances`

---

### 13.1. Tạo vị trí chấm công

|-----------------|---------------------------------------------------------------|
| **Method**      | `POST`                                                        |
| **URL**         | `/api/attendances/locations`                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `attendances.location.manage`                                 |
| **Mô tả**       | Tạo vị trí chấm công mới, có thể gán cho nhân viên/phòng ban  |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "name": "Văn phòng HCM",
  "latitude": 10.7769,
  "longitude": 106.7009,
  "radiusInMeters": 100,
  "assignedUserIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  ],
  "assignedDepartmentIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa7"
  ]
}
```

> `assignedUserIds` và `assignedDepartmentIds` là tùy chọn, có thể để mảng rỗng.

**Response (201):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Văn phòng HCM",
  "latitude": 10.7769,
  "longitude": 106.7009,
  "radiusInMeters": 100,
  "isActive": true,
  "createdAt": "2026-06-18T08:00:00Z",
  "updatedAt": null,
  "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
  "assignedUserIds": ["3fa85f64-5717-4562-b3fc-2c963f66afa6"],
  "assignedDepartmentIds": ["3fa85f64-5717-4562-b3fc-2c963f66afa7"]
}
```

---

### 13.2. Cập nhật vị trí chấm công

|-----------------|---------------------------------------------------------------|
| **Method**      | `PUT`                                                         |
| **URL**         | `/api/attendances/locations/{id}`                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `attendances.location.manage`                                 |
| **Mô tả**       | Cập nhật thông tin vị trí chấm công, gán lại nhân viên/phòng ban |
|-----------------|---------------------------------------------------------------|

**Request:**

```json
{
  "name": "Văn phòng HCM (cập nhật)",
  "latitude": 10.7769,
  "longitude": 106.7009,
  "radiusInMeters": 150,
  "isActive": true,
  "assignedUserIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  ],
  "assignedDepartmentIds": []
}
```

**Response (200):** Giống response của API 13.1.

---

### 13.3. Chi tiết vị trí chấm công

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/attendances/locations/{id}`                             |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `attendances.location.manage`                                 |
| **Mô tả**       | Lấy thông tin chi tiết một vị trí chấm công theo ID          |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body. `id` truyền trên URL (GUID).

**Response (200):** Giống response của API 13.1.

---

### 13.4. Danh sách vị trí chấm công

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/attendances/locations`                                  |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | `attendances.location.manage`                                 |
| **Mô tả**       | Lấy danh sách vị trí chấm công có phân trang và tìm kiếm     |
|-----------------|---------------------------------------------------------------|

**Query Parameters:**

| Tham số     | Kiểu       | Bắt buộc | Mô tả                              |
|-------------|------------|----------|------------------------------------|
| `page`      | `int`      | Không    | Trang hiện tại (mặc định: 1)       |
| `pageSize`  | `int`      | Không    | Số bản ghi mỗi trang (mặc định: 20)|
| `search`    | `string`   | Không    | Tìm kiếm theo tên vị trí           |
| `userId`    | `guid`     | Không    | Lọc theo nhân viên được gán        |
| `startDate` | `date`     | Không    | Lọc từ ngày (YYYY-MM-DD)           |
| `endDate`   | `date`     | Không    | Lọc đến ngày (YYYY-MM-DD)          |

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Văn phòng HCM",
      "latitude": 10.7769,
      "longitude": 106.7009,
      "radiusInMeters": 100,
      "isActive": true,
      "createdAt": "2026-06-18T08:00:00Z",
      "updatedAt": null,
      "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
      "assignedUserIds": [],
      "assignedDepartmentIds": []
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### 13.5. Chấm công (vào ca / tan ca)

|-----------------|-------------------------------------------------------------------|
| **Method**      | `POST`                                                            |
| **URL**         | `/api/attendances/check-in?latitude={lat}&longitude={lng}`        |
| **Auth**        | `Bearer Token`                                                    |
| **Permission**  | Không yêu cầu quyền đặc biệt (nhân viên tự chấm)                  |
| **Mô tả**       | Chấm công vào ca hoặc tan ca. Tọa độ GPS gửi qua **query string** |
|-----------------|-------------------------------------------------------------------|

> ⚠️ **Lưu ý bảo mật**: Tọa độ GPS (`latitude`, `longitude`) được gửi qua **query parameters** (không phải request body)  
> để tách biệt dữ liệu vị trí khỏi payload nghiệp vụ.

**Query Parameters:**

| Tham số     | Kiểu     | Bắt buộc | Mô tả                               |
|-------------|----------|----------|-------------------------------------|
| `latitude`  | `double` | ✅ Có    | Vĩ độ GPS của người dùng hiện tại   |
| `longitude` | `double` | ✅ Có    | Kinh độ GPS của người dùng hiện tại |

**Request Body:**

```json
{
  "type": 1
}
```

> **AttendanceType Enum:**
> - `1` = `CheckIn` — Chấm vào ca
> - `2` = `CheckOut` — Chấm tan ca

**Response (200) — Thành công:**

```json
{
  "isSuccess": true,
  "message": "Chấm công thành công.",
  "distanceInMeters": 42.5,
  "locationName": "Văn phòng HCM",
  "checkTime": "2026-06-18T08:30:00Z"
}
```

**Response (200) — Thất bại (ngoài bán kính):**

```json
{
  "isSuccess": false,
  "message": "Vượt quá bán kính cho phép.",
  "distanceInMeters": 350.2,
  "locationName": "Văn phòng HCM",
  "checkTime": "2026-06-18T08:30:00Z"
}
```

> API luôn trả `200 OK`. Kiểm tra `isSuccess` để biết chấm công có hợp lệ không.  
> Nếu nhân viên chấm lại trong ngày (vào lại / ra lại), hệ thống sẽ **ghi đè** kết quả cũ.

---

### 13.6. Trạng thái chấm công hôm nay

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/attendances/today`                                      |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | Không yêu cầu quyền đặc biệt                                  |
| **Mô tả**       | Lấy trạng thái chấm công hôm nay của người đang đăng nhập     |
|-----------------|---------------------------------------------------------------|

**Request:** Không có body.

**Response (200) — Đã chấm:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "userFullName": "Nguyễn Văn A",
  "date": "2026-06-18",
  "checkInTime": "2026-06-18T01:30:00Z",
  "checkInLocationName": "Văn phòng HCM",
  "checkOutTime": null,
  "checkOutLocationName": null
}
```

**Response (200) — Chưa chấm:** `null`

---

### 13.7. Lịch sử chấm công

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/attendances/logs`                                       |
| **Auth**        | `Bearer Token`                                                |
| **Permission**  | Không yêu cầu quyền đặc biệt (nhân viên xem lịch sử của mình)|
| **Mô tả**       | Lấy danh sách log chấm công, hỗ trợ lọc theo người dùng, ngày, kết quả |
|-----------------|---------------------------------------------------------------|

**Query Parameters:**

| Tham số     | Kiểu       | Bắt buộc | Mô tả                                      |
|-------------|------------|----------|--------------------------------------------|
| `page`      | `int`      | Không    | Trang hiện tại (mặc định: 1)               |
| `pageSize`  | `int`      | Không    | Số bản ghi mỗi trang (mặc định: 20)        |
| `search`    | `string`   | Không    | Tìm kiếm theo tên nhân viên                |
| `userId`    | `guid`     | Không    | Lọc theo ID nhân viên (quản lý xem hộ)     |
| `isSuccess` | `bool`     | Không    | Lọc theo kết quả (`true`/`false`)          |
| `startDate` | `date`     | Không    | Lọc từ ngày (YYYY-MM-DD)                   |
| `endDate`   | `date`     | Không    | Lọc đến ngày (YYYY-MM-DD)                  |

**Response (200):**

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "userFullName": "Nguyễn Văn A",
      "userEmployeeCode": "NV001",
      "attendanceLocationId": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
      "attendanceLocationName": "Văn phòng HCM",
      "checkTime": "2026-06-18T01:30:00Z",
      "latitude": 10.7769,
      "longitude": 106.7009,
      "distanceInMeters": 42.5,
      "isSuccess": true,
      "type": 1,
      "failureReason": null
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

---

### AttendanceType Enum

|---------|----------|
| Giá trị | Tên      |
|---------|----------|
| `1`     | CheckIn  |
| `2`     | CheckOut |
|---------|----------|

---

## 14. System — Health Check

Base URL: `/api/health`

---

### 14.1. Health Check

|-----------------|---------------------------------------------------------------|
| **Method**      | `GET`                                                         |
| **URL**         | `/api/health`                                                 |
| **Auth**        | Không yêu cầu                                                 |
| **Mô tả**       | Kiểm tra API đang hoạt động                                   |
|-----------------|---------------------------------------------------------------|

**Response (200):**

```json
{
  "status": "healthy",
  "timestamp": "2026-06-16T02:25:00Z"
}
```

---

### NotificationChannel Enum

|---------|--------|
| Giá trị | Tên    |
|---------|--------|
| `1`     | InApp  |
| `2`     | Email  |
| `3`     | Push   |
|---------|--------|
