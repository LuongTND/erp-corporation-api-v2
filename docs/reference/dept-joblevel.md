# Department & JobLevel — Checklist
> Pattern: CQRS/MediatR · `IUnitOfWork.Repository<T>()` · `feat/huytq` 2026-07-31  
> `[x]` done · `[ ]` todo

## Done (đừng làm lại)

- [x] Domain entities: Department, JobLevel, UserDepartment
- [x] EF configs: DepartmentConfiguration, JobLevelConfiguration, UserDepartmentConfiguration
- [x] Migrations đã chạy
- [x] ScopeType enum
- [x] Department: Create/Update/Delete commands + GetById/GetList/GetTree queries + DepartmentsController

## TODO — JobLevel

### Commands ✅
- [x] `CreateJobLevelCommand` + Handler + Validator
- [x] `UpdateJobLevelCommand` + Handler + Validator
- [x] `DeleteJobLevelCommand` + Handler

### Queries ✅
- [x] `GetJobLevelByIdQuery` + Handler
- [x] `GetJobLevelsQuery` (paged) + Handler

### Controller ✅
- [x] `JobLevelsController` — route `api/job-levels`

| Endpoint | Permission |
|---|---|
| GET / | `job-levels:view` |
| GET /{id} | `job-levels:view` |
| POST / | `job-levels:create` |
| PUT /{id} | `job-levels:update` |
| DELETE /{id} | `job-levels:delete` |

## TODO — UserDepartment

- [ ] `AddUserDepartmentCommand` (kiêm nhiệm): validate User+Dept tồn tại; check không trùng active; IsPrimary=false; insert
- [ ] `TransferUserDepartmentCommand` (chuyển phòng chính, cần transaction):  
  terminate IsPrimary=true cũ (EndDate=today, IsActive=false) → insert dòng mới IsPrimary=true

## TODO — DataScopeService

```csharp
// Application/Interfaces/IDataScopeService.cs
Task<ScopeType> GetUserScopeAsync(Guid userId, CancellationToken ct);
IQueryable<User> ApplyScope(IQueryable<User> q, ScopeType scope, Guid userId, IEnumerable<Guid>? deptIds);
Task<IEnumerable<Guid>> GetAccessibleDepartmentIdsAsync(Guid userId, CancellationToken ct);
```
Logic: load user.JobLevel.DefaultScopeType → BFS subtree từ phòng chính → apply filter

## TODO — Seed

```csharp
// JobLevel seed
{ "Director",  1, ScopeType.All },
{ "Manager",   2, ScopeType.Department },
{ "Senior",    3, ScopeType.Team },
{ "Staff",     4, ScopeType.Own },
{ "Intern",    5, ScopeType.Own },

// Department root
{ DepartmentName="Công ty", DepartmentCode="ROOT" }
```

## Ghi chú

- FK → Departments/JobLevels dùng `DeleteBehavior.Restrict`
- Application layer không dùng `.Include()` — Mapster conflict. Dùng separate queries + in-memory join.
