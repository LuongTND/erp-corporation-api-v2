using Microsoft.AspNetCore.Mvc;

namespace API.Base;

/// <summary>
/// Controller gốc — <b>Service-first</b>: inject <c>I*Service</c> qua constructor ở controller con.
/// Luồng chuẩn: Controller → IService → IRepository.
/// MediatR chỉ dùng cho domain event handlers (Outbox), không gọi từ controller mặc định.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase;
