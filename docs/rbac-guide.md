# RBAC — Phân quyền động trong Controller

## Cách hoạt động

```
[HasPermission("orders:create")] trên action
    → PermissionAuthorizationHandler
    → PermissionService.GetPermissionsAsync(userId)   ← Redis cache 10 phút
    → HashSet<string>.Contains("orders:create")
    → 200 OK hoặc 403 Forbidden
```

---

## Dùng trong Controller

```csharp
[Authorize]
[ApiController]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    [HasPermission("orders:read")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) { ... }

    [HasPermission("orders:create")]
    [HttpPost]
    public async Task<IActionResult> Create(...) { ... }

    [HasPermission("orders:update")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(...) { ... }

    [HasPermission("orders:delete")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(...) { ... }
}
```

### Permission code convention

```
"{Resource}:{Action}"

Resource  →  PermissionResources.cs (Hrm.User, Task.Item, ...)
Action    →  PermissionAction enum  (read, create, update, delete, approve, assign, export)
```

Ví dụ: `"User.Read"`, `"Department.Create"`, `"Leave.Approve"`

---

## Thêm permission mới

**Chỉ cần đặt `[HasPermission("...")]` lên action — không cần làm gì thêm.**

Lúc app restart, `AppData.SyncPermissionsAsync()` tự:
1. Scan toàn bộ `[HasPermission]` trong assembly
2. Insert permission mới vào bảng `Permissions`
3. Gán tất cả permissions cho role `Admin`

---

## Lấy UserId trong action

```csharp
var userId = User.GetUserId(); // ClaimsPrincipalExtensions
```

---

## Invalidate cache khi thay đổi quyền

Gọi sau khi assign/revoke permission cho role:

```csharp
await permissionService.InvalidateCacheAsync(roleId);
```

Cache user liên quan sẽ bị xóa, lần request tiếp theo lấy lại từ DB.

---

## Bảng liên quan

| Table | Mục đích |
|---|---|
| `Permissions` | Danh sách permission codes |
| `Roles` | Danh sách roles |
| `RolePermissions` | Role được gán permission nào |
| `UserRoles` | User được gán role nào (có ExpiresAt, RevokedAt) |
