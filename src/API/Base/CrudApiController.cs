using API.Filters;
using Application.Common.Models;
using Application.Interfaces.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Base;

/// <summary>
/// Base CRUD: GET list/detail, POST, PUT, DELETE — gọi <see cref="ICrudService{TDto,TCreate,TUpdate}"/>.
/// Controller con chỉ cần inject service và khai báo <see cref="Permissions"/>.
/// </summary>
public abstract class CrudApiController<TService, TDto, TCreate, TUpdate> : BaseApiController, ICrudPermissionProvider
    where TService : ICrudService<TDto, TCreate, TUpdate>
    where TDto : IHasGuidId
{
    protected readonly TService Service;

    protected CrudApiController(TService service, CrudPermissions permissions)
    {
        Service = service;
        Permissions = permissions;
    }

    public CrudPermissions Permissions { get; }

    [HttpGet]
    [RequireCrudPermission(CrudOperation.Read)]
    public virtual async Task<ActionResult<IReadOnlyList<TDto>>> GetAll(CancellationToken ct)
    {
        var items = await Service.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [RequireCrudPermission(CrudOperation.Read)]
    public virtual async Task<ActionResult<TDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await Service.GetByIdAsync(id, ct);
        return Ok(item);
    }

    [HttpPost]
    [RequireCrudPermission(CrudOperation.Create)]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TCreate request, CancellationToken ct)
    {
        var item = await Service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = GetDtoId(item) }, item);
    }

    [HttpPut("{id:guid}")]
    [RequireCrudPermission(CrudOperation.Update)]
    public virtual async Task<ActionResult<TDto>> Update(Guid id, [FromBody] TUpdate request, CancellationToken ct)
    {
        var item = await Service.UpdateAsync(id, request, ct);
        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    [RequireCrudPermission(CrudOperation.Delete)]
    public virtual async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await Service.DeleteAsync(id, ct);
        return NoContent();
    }

    protected virtual Guid GetDtoId(TDto dto) => dto.Id;
}
