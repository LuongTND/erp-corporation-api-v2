# Permission Codes
> Format: **`{resource}:{action}`** · viết thường kebab-case  
> Tự động seed qua `AppData.SyncPermissionsAsync()` khi startup — không cần thêm tay vào DB.

## Quy ước

```
resource = danh từ số nhiều, kebab-case  (vd: job-levels)
action   = create | view | update | delete | assign-{x} | export | approve
```

## Hiện có

| Resource | Codes | Status |
|---|---|---|
| `roles` | view, create, update, delete, assign-permission | ✅ |
| `permissions` | view | ✅ |
| `departments` | view, create, update, delete | ✅ |

## Chưa implement

| Resource | Codes |
|---|---|
| `job-levels` | view, create, update, delete |
| `users` | view, create, update, delete, assign-department, transfer-department |

## Thêm permission mới

1. `[HasPermission("resource:action")]` trên controller action
2. Startup tự seed
3. Gán vào role: `PUT /api/roles/{id}/permissions`
4. Cập nhật bảng trên

## Seed mechanism

```csharp
// SyncPermissionsAsync: scan [HasPermission] attrs → insert missing → assign all to Admin role
```
