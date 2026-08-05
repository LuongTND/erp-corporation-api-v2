# Coding Standard
> .NET 10 · Clean Architecture · CQRS/MediatR — đọc trước khi code feature mới.

## Folder Structure

```
API/Controllers/          Auth/ · RBAC/ · HRM/ · LMS/
Application/Features/     Auth/ · RBAC/ · HRM/{Domain}/ · LMS/{Domain}/
  HRM/Departments/        Shared/(DTO) · Create/ · Update/ · Delete/ · GetById/ · GetList/
Domain/Entities/          Shared/(Audit,Chat,Notifications,Tasks) · RBAC/ · HRM/ · LMS/
Infrastructure/Data/      Configurations/(mirror Domain) · Migrations/ · SeedData/
```

## Naming

| Loại | Pattern |
|---|---|
| Command | `{Action}{Entity}Command` |
| Handler | `{Action}{Entity}Command/QueryHandler` |
| Validator | `{Action}{Entity}CommandValidator` |
| Query | `Get{Entity}[By{Field}]Query` |
| DTO | `{Entity}Response` |
| Controller | `{Entity}sController` |
| EF Config | `{Entity}Configuration` |

## 1. Entity

```csharp
namespace Domain;
public class Foo : AuditableEntityBase<Guid>, ISoftDeletable  // hoặc EntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
```
- `AuditableEntityBase` = CreatedAt/UpdatedAt/CreatedBy/UpdatedBy
- Soft delete → `ISoftDeletable` + global query filter `!IsDeleted`
- Business logic đơn giản đặt trong entity (vd: `ChangeStatus()`), không đặt trong Handler

## 2. EF Config

```csharp
namespace Infrastructure;
public class FooConfiguration : AuditableEntityConfiguration<Foo, Guid>  // hoặc SoftDeleteEntityConfiguration
{
    public override void Configure(EntityTypeBuilder<Foo> builder)
    {
        base.Configure(builder);
        builder.ToTable("Foos");
        builder.HasIndex(f => f.Name).IsUnique();
        builder.Property(f => f.Name).IsRequired().HasMaxLength(255);
        // FK: OnDelete(DeleteBehavior.Restrict) — mặc định, tránh Cascade
    }
}
```

## 3. Command

```csharp
// Create → IRequest<Guid>  |  Update/Delete → IRequest<Unit>
public sealed record CreateFooCommand(string Name, Guid? ParentId) : IRequest<Guid>;
public sealed record UpdateFooCommand(Guid FooId, string Name) : IRequest<Unit>;  // FooId set bởi controller
public sealed record DeleteFooCommand(Guid FooId) : IRequest<Unit>;
```
- `record` + `sealed` · Id không nằm trong body — set bằng `cmd with { FooId = id }`

## 4. Validator

```csharp
public sealed class CreateFooCommandValidator : AbstractValidator<CreateFooCommand>
{
    public CreateFooCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        // Chỉ check format/required — business validation (unique, FK) đặt trong Handler
    }
}
```

## 5. Command Handler

```csharp
public sealed class CreateFooCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateFooCommand, Guid>
{
    public async Task<Guid> Handle(CreateFooCommand cmd, CancellationToken ct)
    {
        // 1. Business validation (unique, FK exists, business rules)
        if (await unitOfWork.Repository<Foo>().AnyAsync(f => f.Name == cmd.Name, ct))
            throw new ConflictException(ExceptionMessages.AlreadyExists("Name", cmd.Name));

        // 2. Create entity
        var foo = new Foo { Id = Guid.NewGuid(), Name = cmd.Name };

        // 3. Save
        await unitOfWork.Repository<Foo>().AddAsync(foo);
        await unitOfWork.EnsureSaveAsync(ct);
        return foo.Id;
    }
}
```
- Primary constructor injection · `FindTrackedAsync` khi update/delete · `FindAsync` khi chỉ validate
- **Không dùng `.Include()`** — Mapster conflict → dùng separate queries + in-memory join
- Transaction (2+ bảng atomic):
  ```csharp
  await unitOfWork.BeginTransactionAsync(ct);
  try { ...; await unitOfWork.EnsureSaveAsync(ct); await unitOfWork.CommitTransactionAsync(); }
  catch { await unitOfWork.RollbackTransactionAsync(ct); throw; }
  ```

## 6. Query Handler

```csharp
// Query
public sealed record GetFoosQuery(QueryInfo QueryInfo) : IRequest<QueryResult<FooResponse>>;
public sealed record GetFooByIdQuery(Guid FooId) : IRequest<FooResponse>;

// Handler pattern — separate queries, join in-memory
var result = await unitOfWork.Repository<Foo>().GetPagedAsync(queryInfo, filter: f => ..., orderBy: q => q.OrderBy(f => f.Name), ct: ct);
var relatedIds = result.Items.Select(f => f.RelatedId).Distinct().ToList();
var related = (await unitOfWork.Repository<Related>().GetPagedAsync(new QueryInfo { Top = relatedIds.Count }, filter: r => relatedIds.Contains(r.Id), ct: ct)).Items.ToDictionary(r => r.Id);
var items = result.Items.Select(f => new FooResponse { ..., RelatedName = related.TryGetValue(f.RelatedId, out var r) ? r.Name : null });
```
- Read-only · Không return entity — luôn map sang DTO
- `QueryInfo`: Top, Skip, SearchText, IsActive, NeedTotalCount

## 7. Response DTO

```csharp
public sealed class FooResponse  // class, không phải record
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? RelatedName { get; set; }  // denormalized — không để client tự join
}
```

## 8. Controller

```csharp
namespace API;
[Authorize][ApiController][Route("api/foos")]
public sealed class FoosController(ISender sender) : ControllerBase
{
    [HasPermission("foos:view")][HttpGet]
    public async Task<ActionResult<ApiResponse<QueryResult<FooResponse>>>> GetList([FromQuery] QueryInfo q, CancellationToken ct)
        => Ok(ApiResponse<QueryResult<FooResponse>>.Ok(await sender.Send(new GetFoosQuery(q), ct)));

    [HasPermission("foos:view")][HttpGet("{fooId:guid}")]
    public async Task<ActionResult<ApiResponse<FooResponse>>> GetById(Guid fooId, CancellationToken ct)
        => Ok(ApiResponse<FooResponse>.Ok(await sender.Send(new GetFooByIdQuery(fooId), ct)));

    [HasPermission("foos:create")][HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateFooCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Guid>.Ok(await sender.Send(cmd, ct)));

    [HasPermission("foos:update")][HttpPut("{fooId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Update(Guid fooId, [FromBody] UpdateFooCommand cmd, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(cmd with { FooId = fooId }, ct)));

    [HasPermission("foos:delete")][HttpDelete("{fooId:guid}")]
    public async Task<ActionResult<ApiResponse<Unit>>> Delete(Guid fooId, CancellationToken ct)
        => Ok(ApiResponse<Unit>.Ok(await sender.Send(new DeleteFooCommand(fooId), ct)));
}
```
- Chỉ gọi `sender.Send()` · Mọi action có `[HasPermission]` · `CancellationToken ct` cuối cùng

## 9. Conventions

**Permission code:** `{resource}:{action}` — `departments:view`, `job-levels:create` (kebab-case, lowercase)  
**Actions:** `view | create | update | delete | assign-{x} | approve | export`

**Exceptions:**
| Tình huống | Exception |
|---|---|
| Không tìm thấy | `NotFoundException(ExceptionMessages.NotFound("Entity", id))` |
| Trùng unique | `ConflictException(ExceptionMessages.AlreadyExists("Field", value))` |
| Vi phạm business rule | `BadRequestException("message")` |
| DB save fail | `EnsureSaveAsync()` tự throw |

**CQRS pipeline:** `Request → Tracking → Performance → Validation → Handler`  
**Validator auto-discovered** bởi DI scan — đặt tên `{Command}Validator` là đủ.

## Checklist feature mới

```
[ ] Domain: Entity (base class, ISoftDeletable?) + EF Config + Migration
[ ] Application: Response DTO · Create · Update · Delete · GetById · GetList
[ ] API: Controller + [HasPermission] trên mọi action
[ ] dotnet build → 0 errors
[ ] Cập nhật docs/reference/permissions.md
```
