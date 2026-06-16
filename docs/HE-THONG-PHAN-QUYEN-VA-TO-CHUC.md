# Hệ thống phân quyền, tổ chức & nhân sự — Đặc tả chi tiết

> Tài liệu mô tả **thiết kế đã chốt** cho module RBAC + org chart + user HR trong `erp-corporation-api-v2`.  
> Dùng làm nguồn tham chiếu khi viết entity, migration, service và API.  
> Cập nhật khi thay đổi quy tắc — **cùng PR** với thay đổi code (xem `CHECKLIST-PR.md`).

---

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Nguyên tắc thiết kế đã chốt](#2-nguyên-tắc-thiết-kế-đã-chốt)
3. [Enum trong Domain (không lưu bảng DB)](#3-enum-trong-domain-không-lưu-bảng-db)
4. [Quy ước cột audit & domain base](#4-quy-ước-cột-audit--domain-base)
5. [Danh sách bảng](#5-danh-sách-bảng)
6. [Chi tiết từng bảng](#6-chi-tiết-từng-bảng)
7. [Logic phân quyền (Permission)](#7-logic-phân-quyền-permission)
8. [Logic phạm vi dữ liệu (Data Scope)](#8-logic-phạm-vi-dữ-liệu-data-scope)
9. [Logic tổ chức & phòng ban](#9-logic-tổ-chức--phòng-ban)
10. [Logic nhân viên & tài khoản](#10-logic-nhân-viên--tài-khoản)
11. [Logic database & migration](#11-logic-database--migration)
12. [Kiến trúc code (khớp repo)](#12-kiến-trúc-code-khớp-repo)
13. [Phase 2 & việc chưa chốt](#13-phase-2--việc-chưa-chốt)
14. [Sơ đồ quan hệ (ERD mô tả)](#14-sơ-đồ-quan-hệ-erd-mô-tả)

---

## 1. Tổng quan

Hệ thống tách **ba trụ độc lập**:

|---------------------------|---------------------------------------------|---------------------------------------------------------|
| Trụ                       | Trả lời câu hỏi                             | Lưu ở đâu                                               |
|---------------------------|---------------------------------------------|---------------------------------------------------------|
| **Role + Permission**     | User **được làm gì**?                       | `Roles`, `Permissions`, `RolePermissions`, `UserRoles`  |
| **Job Level + Scope**     | User **được thấy dữ liệu ở phạm vi nào**?   | `JobLevels.DefaultScopeType` + thuật toán scope         |
| **Department + Manager**  | User **thuộc tổ chức nào**, báo cáo cho ai? | `Departments`, `Users`, `UserDepartments`               |
|---------------------------|---------------------------------------------|---------------------------------------------------------|

**Luồng kiểm tra khi gọi API (đọc/ghi dữ liệu):**

```
Request → Xác thực (JWT / Account) → Lấy UserId
       → Kiểm tra Permission (action) → 403 nếu không có quyền
       → Áp Data Scope (filter query) → chỉ trả dữ liệu trong phạm vi
       → Service / Repository → Database
```

**Luồng nghiệp vụ API (Service-first):**

```
Controller → I*Service → I*Repository → DbContext
```

---

## 2. Nguyên tắc thiết kế đã chốt

### 2.1. Phân quyền (Permission vs Scope)

- **Phương án A:** Scope **tách** khỏi permission.
- **Permission** chỉ mô tả **hành động** (`create`, `read`, `update`, `delete`, `approve`, `export`, …).
- **Không** dùng permission dạng `view_team`, `view_department` (tránh trùng với `ScopeType`).
- **Scope** lấy từ `JobLevel.DefaultScopeType` (enum).
- **Role** chỉ gán permission; **không** gán scope trên role.
- **Ngoại lệ:** Role đặc biệt (vd Super Admin) → `BypassDataScope = true` hoặc scope cố định `All`.

### 2.2. Tổ chức

- **Phương án 2:** Một **phòng ban chính** trên `Users` + bảng phụ **`UserDepartments`** cho kiêm nhiệm.

### 2.3. Đăng nhập

- **Mô hình 2:** Tách **`Users`** (hồ sơ HR) và **`UserAccounts`** (đăng nhập).

### 2.4. Enum

- **Không** tạo bảng `ScopeTypes`, `UserStatuses`, `PermissionModules`, … trong DB.
- Chỉ khai báo **enum C#** trong `Domain/Enums/`.

### 2.5. Audit (phase 1)

|-----------------|---------------------------------------------------------------|
| Hạng mục        | Quyết định                                                    |
|-----------------|---------------------------------------------------------------|
| Thời gian       | `CreatedAt`, `UpdatedAt` — lưu **UTC**                        |
| Người thao tác  | `CreatedBy`, `UpdatedBy` — **nullable**, ghi bằng interceptor |
| Vô hiệu hóa     | `IsActive`                                                    |
| Junction        | `RevokedAt` / `RevokedBy` — **không** xóa cứng hàng           |
| Soft delete     | **Không** dùng phase 1                                        |
| Domain          | `AuditableEntity` kế thừa `BaseEntity`                        |
| Audit sâu       | Bảng `AuditLogs` — **phase 2** (Role / Permission)            |
|-----------------|---------------------------------------------------------------|

---

## 3. Enum trong Domain (không lưu bảng DB)

Đặt tại `Domain/Enums/`. Lưu DB dạng `int` (hoặc `byte`) tương ứng enum.

### 3.1. `ScopeType`

|-------------|---------------|---------------------------------------|-------|
| Giá trị     | Code          | Ý nghĩa                               | Sort  |
|-------------|---------------|---------------------------------------|-------|
| Own         | `Own`         | Chỉ dữ liệu gắn với chính user        | 1     |
| Team        | `Team`        | Cấp dưới trực tiếp (`ManagerId = me`) | 2     |
| Department  | `Department`  | Theo phòng ban (quy tắc ở mục 8)      | 3     |
| All         | `All`         | Toàn công ty (trong tenant)           | 4     |
|-------------|---------------|---------------------------------------|-------|

### 3.2. `UserStatus`

|-----------------|-------------------|-----------------|
| Giá trị         | Code              | Mô tả HR        |
|-----------------|-------------------|-----------------|
| Active          | `Active`          | Đang làm việc   |
| Probation       | `Probation`       | Thử việc        |
| Resigned        | `Resigned`        | Đã nghỉ việc    |
| Terminated      | `Terminated`      | Bị chấm dứt HĐ  |
| Suspended       | `Suspended`       | Tạm đình chỉ    |
| MaternityLeave  | `MaternityLeave`  | Nghỉ thai sản   |
|-----------------|-------------------|-----------------|

**Map sang `IsActive` (phase 1 — cần team xác nhận dòng MaternityLeave):**

|---------------------------------|---------------------------------------|
| UserStatus                      | IsActive (mặc định)                   |
|---------------------------------|---------------------------------------|
| Active, Probation               | `true`                                |
| MaternityLeave                  | `true` hoặc `false` (chốt nghiệp vụ)  |
| Suspended, Resigned, Terminated | `false`                               |
|---------------------------------|---------------------------------------|

### 3.3. `PermissionModule`

|-------------|---------------------------|
| Giá trị     | Mô tả                     |
|-------------|---------------------------|
| Hrm         | Quản lý nhân sự           |
| Lms         | Đào tạo                   |
| Payroll     | Lương & tài chính         |
| Task        | Công việc                 |
| Chat        | Chat nội bộ               |
| Report      | Báo cáo                   |
| System      | Quản trị hệ thống         |
| Kpi         | KPI                       |
| Onboarding  | Onboarding / Offboarding  |
|-------------|---------------------------|

### 3.4. `PermissionAction`

|-----------------|-------------------------------|
| Giá trị         | Mô tả                         |
|-----------------|-------------------------------|
| Create          | Tạo mới                       |
| Read            | Xem / liệt kê                 |
| Update          | Cập nhật                      |
| Delete          | Xóa (logic hoặc vô hiệu hóa)  |
| Approve         | Phê duyệt                     |
| Assign          | Gán người                     |
| Export          | Xuất báo cáo                  |
| ManageTemplate  | Quản lý template              |
|-----------------|-------------------------------|

### 3.5. `PermissionCode` (quy ước chuỗi)

Không phải enum riêng — **chuỗi duy nhất** trên bảng `Permissions`:

```
{module}.{resource}.{action}
```

Ví dụ: `hrm.employee.read`, `lms.course.create`, `system.role.update`.

---

## 4. Quy ước cột audit & domain base

### 4.1. `BaseEntity` (đã có trong repo)

|---------------|-----------------|-----------------------------------------|
| Cột           | Kiểu            | Ý nghĩa                                 |
|---------------|-----------------|-----------------------------------------|
| Id            | GUID            | Khóa chính                              |
| CreatedAt     | DateTime UTC    | Thời điểm tạo                           |
| UpdatedAt     | DateTime? UTC   | Lần cập nhật cuối; `null` nếu chưa sửa  |
| DomainEvents  | (trong memory)  | Sự kiện domain, publish qua Outbox      |
|---------------|-----------------|-----------------------------------------|

### 4.2. `AuditableEntity : BaseEntity`

|----------|------|-----------------------------------------------------------|
| Cột thêm | Kiểu | Ý nghĩa                                                   |
|----------|------|-----------------------------------------------------------|
| IsActive | bool | `false` = ngừng dùng trong nghiệp vụ mới; vẫn giữ lịch sử |
|----------|------|-----------------------------------------------------------|

### 4.3. `TrackedEntity : AuditableEntity` (tuỳ chọn — master quan trọng)

|-----------|-------|-----------------------|
| Cột thêm  | Kiểu  | Ý nghĩa               |
|-----------|-------|-----------------------|
| CreatedBy | GUID? | UserId người tạo      |
| UpdatedBy | GUID? | UserId người sửa cuối |
|-----------|-------|-----------------------|

Áp dụng gợi ý: `Departments`, `JobLevels`, `Roles`, `Permissions`, `Users`.

### 4.4. Ghi audit tự động

- `AppDbContext` / interceptor: khi `SaveChanges`, set `UpdatedAt` cho entity `BaseEntity` đang `Modified`.
- Interceptor đọc `ICurrentUserService.UserId` → gán `CreatedBy` / `UpdatedBy` khi thêm/sửa.

---

## 5. Danh sách bảng

|-----|-----------------|-------------|-------------------------------|
| #   | Bảng            | Nhóm        | Mô tả ngắn                    |
|-----|-----------------|-------------|-------------------------------|
| 1   | Departments     | Tổ chức     | Cây phòng ban                 |
| 2   | JobLevels       | Tổ chức     | Cấp bậc + scope mặc định      |
| 3   | Roles           | Phân quyền  | Vai trò hệ thống              |
| 4   | Permissions     | Phân quyền  | Quyền chi tiết (action)       |
| 5   | RolePermissions | Junction    | Role ↔ Permission             |
| 6   | Users           | Nhân sự     | Hồ sơ nhân viên               |
| 7   | UserAccounts    | Identity    | Đăng nhập                     |
| 8   | UserDepartments | Junction    | Kiêm nhiệm phòng ban          |
| 9   | UserRoles       | Junction    | User ↔ Role                   |
| 10  | OutboxMessages  | Kỹ thuật    | Outbox domain events (đã có)  |
| 11  | AuditLogs       | Phase 2     | Nhật ký thay đổi nhạy cảm     |
|-----|-----------------|-------------|-------------------------------|

**Không có bảng:** ScopeTypes, UserStatuses, PermissionModules, PermissionActions (chỉ enum C#).

---

## 6. Chi tiết từng bảng

### 6.1. `Departments`

**Ý nghĩa bảng:** Đơn vị tổ chức (phòng/ban/bộ phận). Hỗ trợ **cây** qua phòng ban cha.

|-----------------------|---------------|:---------:|-----------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                           |
|-----------------------|---------------|:---------:|-----------------------------------|
| Id                    | GUID          | v         | PK (BaseEntity)                   |
| DepartmentName        | nvarchar(200) | v         | Tên hiển thị (vd: Phòng Nhân sự)  |
| DepartmentCode        | nvarchar(50)  | v         | Mã ngắn, unique (HR, IT, SALES)   |
| ParentDepartmentId    | GUID          |           | FK → Departments.Id; null = gốc   |
| ManagerId             | GUID          |           | FK → Users.Id; trưởng đơn vị      |
| Description           | nvarchar(max) |           | Mô tả                             |
| IsActive              | bit           | v         | Ngừng hoạt động                   |
| CreatedAt, UpdatedAt  | datetime2     | v         | Audit UTC                         |
| CreatedBy, UpdatedBy  | GUID          |           | Nullable                          |
|-----------------------|---------------|:---------:|-----------------------------------|

**Ràng buộc / logic:**

- `DepartmentCode` unique trong phạm vi tenant (nếu sau này multi-tenant).
- Không cho `ParentDepartmentId` tạo **chu trình** (validate khi lưu).
- `ManagerId` có thể thuộc phòng ban khác — cần quy tắc nghiệp vụ (thường vẫn hợp lệ).

---

### 6.2. `JobLevels`

**Ý nghĩa bảng:** Cấp bậc chức danh (seniority). Quyết định **scope dữ liệu mặc định** và tham chiếu lương.

|-----------------------|---------------|:---------:|-------------------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                                   |
|-----------------------|---------------|:---------:|-------------------------------------------|
| Id                    | GUID          | v         | PK                                        |
| LevelName             | nvarchar(100) | v         | Tên (Staff, Manager, Director…)           |
| LevelOrder            | int           | v         | Thứ tự so sánh: **số nhỏ = cấp cao hơn**  |
| DefaultScopeType      | int           | v         | Enum `ScopeType` — scope mặc định         |
| Description           | nvarchar(max) |           | Mô tả                                     |
| BaseSalaryMin         | decimal(18,2) |           | Lương tối thiểu tham chiếu                |
| BaseSalaryMax         | decimal(18,2) |           | Lương tối đa tham chiếu                   |
| IsActive              | bit           | v         |                                           |
| CreatedAt, UpdatedAt  | datetime2     | v         |                                           |
| CreatedBy, UpdatedBy  | GUID          |           |                                           |
|-----------------------|---------------|:---------:|-------------------------------------------|

**Logic:**

- Khi user có nhiều role, **scope** vẫn lấy từ **JobLevel** (không từ role), trừ role `BypassDataScope`.
- Có thể map `LevelOrder` → `DefaultScopeType` khi seed (vd Order 1–2 → All, 3–4 → Department…).

---

### 6.3. `Roles`

**Ý nghĩa bảng:** Vai trò gán cho user; gom tập **permission**.

|-----------------------|---------------|:---------:|---------------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                               |
|-----------------------|---------------|:---------:|---------------------------------------|
| Id                    | GUID          | v         | PK                                    |
| RoleName              | nvarchar(100) | v         | Tên kỹ thuật (ROLE_HR_ADMIN)          |
| DisplayName           | nvarchar(200) | v         | Tên hiển thị                          |
| Description           | nvarchar(max) |           |                                       |
| IsSystemRole          | bit           | v         | Role hệ thống — không xóa trên UI     |
| BypassDataScope       | bit           | v         | `true` = luôn scope All (Super Admin) |
| IsActive              | bit           | v         |                                       |
| CreatedAt, UpdatedAt  | datetime2     | v         |                                       |
| CreatedBy, UpdatedBy  | GUID          |           |                                       |
|-----------------------|---------------|:---------:|---------------------------------------|

**Logic:**

- User có **nhiều role** → permission = **hợp** (union) tất cả permission của các role active.
- Role **không** chứa scope.

---

### 6.4. `Permissions`

**Ý nghĩa bảng:** Một quyền hành động cụ thể trên tài nguyên.

|-----------------------|---------------|:---------:|-----------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                           |
|-----------------------|---------------|:---------:|-----------------------------------|
| Id                    | GUID          | v         | PK                                |
| PermissionCode        | nvarchar(200) | v         | Unique, vd `hrm.employee.read`    |
| PermissionName        | nvarchar(200) | v         | Tên hiển thị                      |
| Module                | int           | v         | Enum `PermissionModule`           |
| Action                | int           | v         | Enum `PermissionAction`           |
| Resource              | nvarchar(100) | v         | Tên resource (employee, course…)  |
| Description           | nvarchar(max) |           |                                   |
| IsActive              | bit           | v         |                                   |
| CreatedAt, UpdatedAt  | datetime2     | v         |                                   |
|-----------------------|---------------|:---------:|-----------------------------------|

**Logic:**

- Seed danh sách permission khi deploy; ít khi tạo tay trên prod.
- Kiểm tra quyền: `IAuthorizationService.HasPermission(userId, "hrm.employee.read")`.

---

### 6.5. `RolePermissions`

**Ý nghĩa bảng:** Nhiều-nhiều Role ↔ Permission.

|---------------|-----------|:---------:|---------------------------------|
| Trường        | Kiểu      | Bắt buộc  | Ý nghĩa                         |
|---------------|-----------|:---------:|---------------------------------|
| RoleId        | GUID      | v         | FK → Roles                      |
| PermissionId  | GUID      | v         | FK → Permissions                |
| AssignedAt    | datetime2 | v         | Thời điểm gán                   |
| AssignedBy    | GUID      |           | User gán (nullable system seed) |
|---------------|-----------|:---------:|---------------------------------|

**PK:** `(RoleId, PermissionId)`.

**Logic:** Thay đổi quyền role = thêm/xóa dòng (hoặc soft qua bảng riêng phase 2). Phase 1 có thể DELETE vì ít thay đổi và có seed backup.

---

### 6.6. `Users`

**Ý nghĩa bảng:** Hồ sơ **nhân viên** (HR). Không chứa password.

|-----------------------|---------------|:---------:|---------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                         |
|-----------------------|---------------|:---------:|---------------------------------|
| Id                    | GUID          | v         | PK                              |
| EmployeeCode          | nvarchar(50)  | v         | Mã NV, unique                   |
| FullName              | nvarchar(200) | v         | Họ tên                          |
| Email                 | nvarchar(256) | v         | Email công ty, unique           |
| AvatarUrl             | nvarchar(500) |           | Ảnh đại diện                    |
| DepartmentId          | GUID          | v         | FK — **phòng ban chính**        |
| JobLevelId            | GUID          | v         | FK — cấp bậc                    |
| ManagerId             | GUID          |           | FK → Users (quản lý trực tiếp)  |
| DateOfJoin            | date          | v         | Ngày vào làm                    |
| Status                | int           | v         | Enum `UserStatus`               |
| IsActive              | bit           | v         | Còn dùng hệ thống / tài khoản   |
| CreatedAt, UpdatedAt  | datetime2     | v         |                                 |
| CreatedBy, UpdatedBy  | GUID          |           |                                 |
|-----------------------|---------------|:---------:|---------------------------------|

**Logic:**

- `Status` = trạng thái HR; `IsActive` = còn truy cập hệ thống (map theo bảng mục 3.2).
- Cây quản lý: `ManagerId` (self-reference).
- Không lưu password tại đây.

---

### 6.7. `UserAccounts`

**Ý nghĩa bảng:** Thông tin **đăng nhập** (Identity), tách khỏi HR.

|-----------------------|---------------|:---------:|-------------------------------------------|
| Trường                | Kiểu          | Bắt buộc  | Ý nghĩa                                   |
|-----------------------|---------------|:---------:|-------------------------------------------|
| Id                    | GUID          | v         | PK (AccountId)                            |
| UserId                | GUID          | v         | FK → Users, unique (1:1)                  |
| LoginEmail            | nvarchar(256) | v         | Email đăng nhập, unique                   |
| PasswordHash          | nvarchar(max) |           | Hash (null nếu chỉ SSO sau này)           |
| IsLocked              | bit           | v         | Khóa tài khoản                            |
| LastLoginAt           | datetime2     |           | Đăng nhập cuối                            |
| FailedLoginCount      | int           |           | Chống brute force (tuỳ chọn)              |
| RefreshToken          | nvarchar(max) |           | Refresh token hiện tại (opaque string)    |
| RefreshTokenExpiresAt | datetime2     |           | Thời điểm hết hạn refresh token           |
| CreatedAt, UpdatedAt  | datetime2     | v         |                                           |
|-----------------------|---------------|:---------:|-------------------------------------------|

**Logic:**

- JWT chứa `AccountId` (hoặc `sub`) → resolve `UserId`.
- Nghỉ việc: khóa account (`IsLocked` / xóa session) + `Users.IsActive = false`.

---

### 6.8. `UserDepartments`

**Ý nghĩa bảng:** Kiêm nhiệm / gắn thêm phòng ban (ngoài phòng chính trên `Users`).

|-----------------------|-----------|:---------:|---------------------------------------------------------|
| Trường                | Kiểu      | Bắt buộc  | Ý nghĩa                                                 |
|-----------------------|-----------|:---------:|---------------------------------------------------------|
| Id                    | GUID      | v         | PK (surrogate)                                          |
| UserId                | GUID      | v         | FK → Users                                              |
| DepartmentId          | GUID      | v         | FK → Departments                                        |
| IsPrimary             | bit       | v         | `true` chỉ cho một dòng/user (trùng Users.DepartmentId) |
| StartDate             | date      | v         | Ngày bắt đầu kiêm nhiệm                                 |
| EndDate               | date      |           | Null = còn hiệu lực                                     |
| IsActive              | bit       | v         |                                                         |
| CreatedAt, UpdatedAt  | datetime2 | v         |                                                         |
|-----------------------|-----------|:---------:|---------------------------------------------------------|

**Unique gợi ý:** `(UserId, DepartmentId)` where active.

**Logic scope (chốt với BA):**

- Mặc định `ScopeType.Department` = **phòng chính** + cây con của phòng đó.
- Kiêm nhiệm: chỉ mở rộng scope khi có **role đặc biệt** hoặc rule module riêng (ghi rõ trong từng module).

---

### 6.9. `UserRoles`

**Ý nghĩa bảng:** Gán role cho user (nhiều-nhiều).

|-------------|-----------|:---------:|-----------------------|
| Trường      | Kiểu      | Bắt buộc  | Ý nghĩa               |
|-------------|-----------|:---------:|-----------------------|
| Id          | GUID      | v         | PK                    |
| UserId      | GUID      | v         | FK → Users            |
| RoleId      | GUID      | v         | FK → Roles            |
| AssignedAt  | datetime2 | v         | Ngày gán              |
| AssignedBy  | GUID      |           | Người gán (HR)        |
| RevokedAt   | datetime2 |           | Null = còn hiệu lực   |
| RevokedBy   | GUID      |           | Người gỡ              |
| IsActive    | bit       | v         | `false` khi đã revoke |
|-------------|-----------|:---------:|-----------------------|

**Logic:**

- **Không DELETE** — gỡ role = set `RevokedAt`, `IsActive = false`.
- Permission hiệu lực = union các role có `IsActive = true` và `RevokedAt is null`.

---

### 6.10. `OutboxMessages` (đã triển khai)

**Ý nghĩa bảng:** Lưu domain events để publish tin cậy sau `SaveChanges`. Không thuộc nghiệp vụ HR.

---

### 6.11. `AuditLogs` (Phase 2)

**Ý nghĩa bảng:** Lịch sử thay đổi chi tiết (Role, Permission, gán quyền).

|-------------|---------------|-------------------------------------|
| Trường      | Kiểu          | Ý nghĩa                             |
|-------------|---------------|-------------------------------------|
| Id          | GUID          | PK                                  |
| EntityName  | nvarchar(100) | Tên bảng/entity                     |
| EntityId    | GUID          | Id bản ghi                          |
| Action      | nvarchar(50)  | Create, Update, Delete, AssignRole… |
| OldValues   | nvarchar(max) | JSON                                |
| NewValues   | nvarchar(max) | JSON                                |
| UserId      | GUID          | Người thực hiện                     |
| OccurredAt  | datetime2     | UTC                                 |
| TraceId     | nvarchar(100) | Khớp `extensions.traceId` API       |
|-------------|---------------|-------------------------------------|

---

## 7. Logic phân quyền (Permission)

### 7.1. Tập permission của user

```
permissions = DISTINCT
  SELECT p.PermissionCode
  FROM UserRoles ur
  JOIN Roles r ON r.Id = ur.RoleId AND r.IsActive = 1
  JOIN RolePermissions rp ON rp.RoleId = r.Id
  JOIN Permissions p ON p.Id = rp.PermissionId AND p.IsActive = 1
  WHERE ur.UserId = @userId
    AND ur.IsActive = 1
    AND ur.RevokedAt IS NULL
```

### 7.2. Kiểm tra một action

```
HasPermission(userId, permissionCode):
  if permissionCode in permissions(userId) → true
  else → false → API 403
```

### 7.3. Super Admin / Bypass

```
if user has any role with BypassDataScope = true:
  skip scope filter (coi như ScopeType.All)
```

### 7.4. Không trộn scope vào permission

|-------------------------------------|---------------------------|
| Đúng                                | Sai                       |
|-------------------------------------|---------------------------|
| `hrm.employee.read` + filter scope  | `hrm.employee.read.team`  |
|-------------------------------------|---------------------------|

---

## 8. Logic phạm vi dữ liệu (Data Scope)

### 8.1. Scope hiệu lực

```
effectiveScope = JobLevels.DefaultScopeType của user
if any active role has BypassDataScope → All
```

**Chưa gộp scope từ role** (đã chốt).

### 8.2. Filter theo `ScopeType` (ví dụ resource `Employee` / Users)

|-----------------|-------------------------------------------------------------------------------------------------|
| ScopeType       | Điều kiện filter (gợi ý)                                                                        |
|-----------------|-------------------------------------------------------------------------------------------------|
| **Own**         | `Users.Id = currentUserId`                                                                      |
| **Team**        | `Users.ManagerId = currentUserId` (chỉ **cấp 1** — chốt BA)                                     |
| **Department**  | `Users.DepartmentId` thuộc tập phòng ban: phòng chính + **toàn bộ con** trong cây `Departments` |
| **All**         | Không filter (hoặc filter tenant sau này)                                                       |
|-----------------|-------------------------------------------------------------------------------------------------|

### 8.3. Cây phòng ban

```
DepartmentSubtree(rootDepartmentId):
  -- đệ quy hoặc CTE theo ParentDepartmentId
  trả về tất cả DepartmentId con + root
```

### 8.4. Kết hợp Permission + Scope trong service

```csharp
// Pseudocode
await _auth.EnsurePermissionAsync(userId, "hrm.employee.read");
var scope = await _dataScope.GetEffectiveScopeAsync(userId);
var query = _userRepository.Query();
query = _dataScope.Apply(query, scope, userId);
return await query.ToListAsync();
```

### 8.5. Việc cần chốt với BA

|-----------------------------------------------------------------|-----------------------------------|
| Câu hỏi                                                         | Gợi ý mặc định                    |
|-----------------------------------------------------------------|-----------------------------------|
| TEAM = 1 cấp hay cả cây manager?                                | 1 cấp (`ManagerId`)               |
| DEPARTMENT có tính kiêm nhiệm?                                  | Chỉ phòng chính + subtree         |
| Trưởng phòng `Departments.ManagerId` có auto scope Department?  | Có thể coi như Manager job level  |
|-----------------------------------------------------------------|-----------------------------------|

---

## 9. Logic tổ chức & phòng ban

1. Mỗi user có **đúng một** `Users.DepartmentId` (phòng chính).
2. `UserDepartments` ghi thêm phòng kiêm nhiệm; `IsPrimary = true` trùng phòng chính.
3. `Departments.ManagerId` = user đứng đầu đơn vị (không nhất thiết thuộc phòng đó — rule HR).
4. Báo cáo headcount theo phòng: thường theo **phòng chính** trừ báo cáo kiêm nhiệm riêng.

---

## 10. Logic nhân viên & tài khoản

### 10.1. Tạo nhân viên mới

1. Insert `Users` (Status = Probation/Active, IsActive = true).
2. Insert `UserDepartments` (phòng chính, IsPrimary = true).
3. (Tuỳ chọn) Insert `UserAccounts` khi cấp tài khoản.
4. Gán `UserRoles` (vd ROLE_EMPLOYEE).

### 10.2. Nghỉ việc

1. `Users.Status` = Resigned / Terminated.
2. `Users.IsActive` = false.
3. `UserAccounts.IsLocked` = true.
4. Revoke các `UserRoles` (RevokedAt, IsActive = false).

### 10.3. Đăng nhập & Refresh Token

**Đăng nhập (`POST /auth/login`):**

1. Xác thực `UserAccounts` (email + password).
2. Sinh Access Token (JWT) + Refresh Token (chuỗi ngẫu nhiên, lưu trên `UserAccounts`).
3. Trả về cả hai token cho client.

**Làm mới token (`POST /auth/refresh`):**

1. Client gửi refresh token.
2. Tìm `UserAccount` theo `RefreshToken`, kiểm tra `RefreshTokenExpiresAt`.
3. Sinh cặp Access Token + Refresh Token mới, ghi đè refresh token cũ.

**Thu hồi token (`POST /auth/revoke`):**

1. Xóa `RefreshToken` và `RefreshTokenExpiresAt` trên `UserAccounts`.
2. Dùng khi user logout.

**Middleware:** JWT → map `ICurrentUserService`.

---

## 11. Logic database & migration

### 11.1. Thứ tự tạo bảng (FK)

```
JobLevels, Departments (Department.Parent nullable)
  → Users (FK Department, JobLevel, Manager)
  → UserAccounts
  → Roles, Permissions
  → RolePermissions
  → UserRoles, UserDepartments
  → AuditLogs (phase 2)
```

### 11.2. Index gợi ý

|---------------|-----------------------------------------------------------|
| Bảng          | Index                                                     |
|---------------|-----------------------------------------------------------|
| Users         | Unique EmployeeCode, Email; Index ManagerId, DepartmentId |
| UserAccounts  | Unique UserId, LoginEmail                                 |
| Permissions   | Unique PermissionCode                                     |
| UserRoles     | Index UserId, RoleId, filter IsActive                     |
| Departments   | Index ParentDepartmentId                                  |
|---------------|-----------------------------------------------------------|

### 11.3. Migration & EF

- Entity kế thừa `AuditableEntity` / `TrackedEntity` tùy bảng.
- Cấu hình Fluent API: `Infrastructure/Persistence/Configurations/`.
- Enum lưu `int` với `.HasConversion<int>()` hoặc value converter.
- Sau khi thêm migration: **kiểm tra `Up()` không rỗng** (xem `HUONG-DAN-MIGRATION.md` nếu có).

### 11.4. Seed dữ liệu ban đầu

- `JobLevels` + `DefaultScopeType`.
- `Roles` (Super Admin `BypassDataScope = true`, HR Admin, Manager, Employee…).
- `Permissions` theo module/action.
- `RolePermissions` map role ↔ permission.

---

## 12. Kiến trúc code (khớp repo)

### 12.1. Cấu trúc thư mục

```
Domain/
  Base/           BaseEntity, AuditableEntity, TrackedEntity
  Entities/       User, UserAccount, Department, ...
  Enums/          ScopeType, UserStatus, PermissionModule, PermissionAction
  Events/         (domain events nghiệp vụ)

Application/
  Interfaces/
    Services/
      IAuthorizationService.cs
      IDataScopeService.cs
      ICurrentUserService.cs
      IUserService.cs, ...
    Repositories/
  DTOs/
  Validators/

Infrastructure/
  Implementations/
    Services/       AuthorizationService, DataScopeService, UserService
    Repositories/
  Persistence/
    AppDbContext.cs
    Configurations/
    Interceptors/   AuditSaveChangesInterceptor
  Migrations/

API/
  Controllers/      Service-first
  Middlewares/      ExceptionMiddleware, (JWT middleware)
```

### 12.2. Interface gợi ý

**`IAuthorizationService`**

- `Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken ct)`
- `Task EnsurePermissionAsync(...)` → throw `ForbiddenException`

**`IDataScopeService`**

- `Task<ScopeType> GetEffectiveScopeAsync(Guid userId, CancellationToken ct)`
- `IQueryable<T> ApplyScope<T>(IQueryable<T> query, ScopeType scope, Guid userId)` (hoặc specification)

**`ICurrentUserService`**

- `Guid? UserId`, `Guid? AccountId` (từ HTTP context)

### 12.3. Luồng API (Service-first)

```
EmployeeController.GetList()
  → IEmployeeService.GetListAsync()
       → _auth.EnsurePermissionAsync(..., "hrm.employee.read")
       → _scope.Apply(..., await _scope.GetEffectiveScopeAsync(...))
       → _employeeRepository.ListAsync(filteredQuery)
```

MediatR: dùng cho **domain event handlers** (Outbox), **không** gọi từ controller mặc định.

---

## 13. Phase 2 & việc chưa chốt

### Phase 2

- Bảng `AuditLogs` + ghi khi đổi Role/Permission/UserRoles.
- SSO nâng cao trên `UserAccounts`.
- Multi-tenant (nếu có).

### Cần chốt với BA / team

|---|-------------------------------------------------------------|
| # | Nội dung                                                    |
|---|-------------------------------------------------------------|
| 1 | `MaternityLeave` → `IsActive` true hay false?               |
| 2 | TEAM: chỉ cấp 1 hay cả cây `ManagerId`?                     |
| 3 | Scope Department có gồm phòng kiêm nhiệm không?             |
| 4 | `Departments.ManagerId` bắt buộc thuộc phòng ban đó không?  |
|---|-------------------------------------------------------------|

---

## 14. Sơ đồ quan hệ (ERD mô tả)

```
Departments ──< Users.DepartmentId (phòng chính)
     │ self ParentDepartmentId
     │
     ├── ManagerId ──> Users
     │
JobLevels ──< Users.JobLevelId

Users ──< Users.ManagerId (self)
Users ──1:1── UserAccounts
Users ──< UserDepartments >── Departments
Users ──< UserRoles >── Roles ──< RolePermissions >── Permissions

Permissions: Module (enum), Action (enum), Resource, PermissionCode
JobLevels: DefaultScopeType (enum)
Users: Status (enum), IsActive
```

---

## Tài liệu liên quan

- `CAU-TRUC-THU-MUC.md` — cấu trúc thư mục & quy tắc layer
- `CHECKLIST-PR.md` — checklist trước khi merge
- `HUONG-DAN-MIGRATION.md` — EF migration (nếu có trong repo)

---

*Phiên bản: 1.0 — phản ánh quyết định thiết kế đã chốt trong discussion.*
