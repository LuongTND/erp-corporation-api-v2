# API Documentation — ERP Corporation API v2

> Tài liệu liệt kê tất cả API endpoints trong hệ thống.  
> **Cập nhật liên tục** mỗi khi thêm/sửa API mới.  
> Cập nhật lần cuối: **2026-06-03**

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

|-----|---------------------------|-----------|-----------------------------------|-------------------------------------------------|
| #   | Tên API                   | Method    | URL                               | Chức năng                                       |
|-----|---------------------------|-----------|-----------------------------------|-------------------------------------------------|
| 5   | DS người dùng             | `GET`     | `/api/users`                      | Lấy danh sách tất cả người dùng                 |
| 6   | Chi tiết người dùng       | `GET`     | `/api/users/{id}`                 | Lấy thông tin người dùng theo ID                |
| 7   | Tạo người dùng            | `POST`    | `/api/users`                      | Tạo người dùng mới                              |
| 8   | Cập nhật người dùng       | `PUT`     | `/api/users/{id}`                 | Cập nhật thông tin người dùng                   |
| 9   | Xóa người dùng            | `DELETE`  | `/api/users/{id}`                 | Xóa (vô hiệu hóa) người dùng                    |
| 10  | Gán vai trò               | `POST`    | `/api/users/{id}/roles`                       | Gán danh sách vai trò cho người dùng            |
| 11  | Đặt lại mật khẩu          | `POST`    | `/api/users/{id}/reset-password`              | Đặt lại mật khẩu cho người dùng                 |
| 12  | DS phòng ban kiêm nhiệm   | `GET`     | `/api/users/{id}/departments`                 | Lấy danh sách phòng ban kiêm nhiệm              |
| 13  | Gán phòng ban kiêm nhiệm  | `POST`    | `/api/users/{id}/departments`                 | Gán phòng ban kiêm nhiệm mới                    |
| 14  | Gỡ phòng ban kiêm nhiệm   | `DELETE`  | `/api/users/{id}/departments/{departmentId}`  | Gỡ bỏ phòng ban kiêm nhiệm                     |
|-----|---------------------------|-----------|-----------------------------------|-------------------------------------------------|

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

---

## Chi tiết từng API

### Mục lục chi tiết

1. [Auth — Xác thực](#1-auth--xác-thực)
2. [Users — Quản lý người dùng](#2-users--quản-lý-người-dùng)
3. [Departments — Quản lý phòng ban](#3-departments--quản-lý-phòng-ban)
4. [Roles — Quản lý vai trò](#4-roles--quản-lý-vai-trò)
5. [JobLevels — Quản lý cấp bậc chức danh](#5-joblevels--quản-lý-cấp-bậc-chức-danh)

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
  "email": "admin@company.com",
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
  "email": "admin@company.com",
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
