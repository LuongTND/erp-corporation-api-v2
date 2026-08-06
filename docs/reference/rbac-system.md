# RBAC + Scope System
> Permission = làm được gì (ngang) · Scope = thấy bao nhiêu (dọc) · Department = thuộc đơn vị nào

## Luồng request

```
JWT → IUserContext(UserId)
→ [HasPermission("x:y")] → PermissionService [Redis 10min] → 403?
→ Handler → DataScopeService.ApplyScopeAsync(query, userId)
          → ScopeOverride ?? JobLevel.DefaultScopeType → filter
→ IUnitOfWork.Repository<T>()
```

## 1. Role & Permission

```
User ──< UserRoles >── Roles ──< RolePermissions >── Permissions
```
- Permission code: `{resource}:{action}` vd `employees:view`
- Auto-seed startup: scan `[HasPermission]` attrs → INSERT missing → assign ALL to Admin
- Gán role: `POST /api/users/{id}/roles` · Thu hồi: `DELETE /api/users/{id}/roles/{roleId}`
- Gỡ = set `RevokedAt + IsActive=false`, KHÔNG xóa · `IsValid() => IsActive && RevokedAt==null && ExpiresAt>UtcNow`
- Cache invalid: `InvalidateCacheAsync(roleId)` sau khi assign/revoke permission

## 2. Data Scope

| ScopeType | Filter | Dùng cho |
|---|---|---|
| Own=1 | `u.Id == me` | Staff, Intern |
| Team=2 | `u.Id==me \|\| u.ManagerId==me` | Senior, Team lead |
| Department=3 | BFS subtree phòng chính | Manager |
| All=4 | không filter | Director, Admin |

**Ưu tiên:** `User.ScopeOverride (nullable)` → có → dùng · null → `JobLevel.DefaultScopeType`  
**Set override:** `PUT /api/users/{id}/scope` body `{ "scopeOverride": 4 }` (null = xóa override)  
**BFS:** load all Departments 1 lần → queue từ phòng chính IsPrimary=true → collect subtree Guid set

## 3. JobLevel mặc định

| Level | Order | DefaultScope |
|---|---|---|
| Director | 1 | All |
| Manager | 2 | Department |
| Senior | 3 | Team |
| Staff | 4 | Own |
| Intern | 5 | Own |

## 4. Department / Org

- User gắn phòng qua `UserDepartments` — **không FK trực tiếp trên Users**
- `IsPrimary=true` = phòng chính (1 dòng active/user)
- Kiêm nhiệm: `POST /api/users/{id}/departments` (IsPrimary=false)
- Chuyển phòng: `PUT /api/users/{id}/departments/transfer` — transaction: terminate cũ → insert mới
- Cây đa cấp: Công ty → Miền → Cửa hàng/Phòng ban (leaf)

## 5. Ví dụ

| Người | JobLevel | Role | Scope |
|---|---|---|---|
| CEO | Director | admin | All |
| Trưởng phòng HR | Manager | hr | Department (phòng HR + con) |
| Nhân viên HR | Staff | hr | Own |
| Quản lý CH HN01 | Manager | store-manager | Department (CH HN01) |

## 6. Dynamic config — tất cả ✅

`api/roles` CRUD · `PUT roles/{id}/permissions` · `POST/DELETE users/{id}/roles`  
`api/job-levels` CRUD · `api/departments` CRUD+tree  
`POST/PUT users/{id}/departments[/transfer]` · `PUT users/{id}/scope`

## 7. Entities compact

```csharp
User:           JobLevelId, ManagerId?, ScopeOverride?
JobLevel:       LevelOrder, DefaultScopeType, BaseSalaryMin?, BaseSalaryMax?
Department:     ParentDepartmentId?, ManagerId?, DepartmentCode(unique), IsActive
UserDepartment: UserId, DepartmentId, IsPrimary, StartDate, EndDate?, IsActive
UserRole:       UserId, RoleId, AssignedAt, AssignedBy?, ExpiresAt?, RevokedAt?, IsActive
Role:           RoleName, IsSystemRole, IsActive
Permission:     PermissionCode("{resource}:{action}"), IsActive
```
