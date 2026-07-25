# CQRS + MediatR Guide

## Pipeline (auto-configured)

```
Request → Tracking → Performance → Validation → Handler
```

## Folder structure

```
Application/Features/<Domain>/<Feature>/
├── XxxCommand.cs
├── XxxCommandHandler.cs
├── XxxCommandValidator.cs   ← chỉ cho Command
└── XxxQuery.cs
    XxxQueryHandler.cs
```

## Command (write)

```csharp
public record CreateTransferCommand(Guid From, Guid To, List<Item> Items) : IRequest<Guid>;

public class CreateTransferCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateTransferCommand, Guid>
{
    public async Task<Guid> Handle(CreateTransferCommand req, CancellationToken ct)
    {
        var entity = Transfer.Create(req.From, req.To, req.Items);
        await uow.Repository<Transfer>().AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return entity.Id;
    }
}

public class CreateTransferCommandValidator : AbstractValidator<CreateTransferCommand>
{
    public CreateTransferCommandValidator()
    {
        RuleFor(x => x.From).NotEmpty();
        RuleFor(x => x.To).NotEmpty().NotEqual(x => x.From);
        RuleFor(x => x.Items).NotEmpty();
    }
}
```

## Query (read)

```csharp
public record GetReportQuery(Guid WarehouseId) : IRequest<ReportDto>;

public class GetReportQueryHandler(AppDbContext db) : IRequestHandler<GetReportQuery, ReportDto>
{
    public async Task<ReportDto> Handle(GetReportQuery req, CancellationToken ct)
        => await db.Reports
            .Where(r => r.WarehouseId == req.WarehouseId)
            .Select(r => new ReportDto(r.WarehouseId, r.Total))
            .FirstOrDefaultAsync(ct)
           ?? throw new NotFoundException(nameof(Report), req.WarehouseId);
}
```

## Controller

```csharp
[ApiController, Route("api/transfers")]
public class TransferController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransferCommand cmd, CancellationToken ct)
        => CreatedAtAction(nameof(Create), new { id = await sender.Send(cmd, ct) }, null);

    [HttpGet("report")]
    public async Task<IActionResult> Report([FromQuery] GetReportQuery q, CancellationToken ct)
        => Ok(await sender.Send(q, ct));
}
```

## Rules

| Rule | Lý do |
|---|---|
| Command trả `Guid` / `Unit` | Không mix write/read model |
| Query trả DTO, không trả entity | Tách read model khỏi domain |
| Controller chỉ gọi `sender.Send()` | Thin dispatcher |
| Validator tên = `<Command>Validator` | Auto-discovered bởi DI scan |
