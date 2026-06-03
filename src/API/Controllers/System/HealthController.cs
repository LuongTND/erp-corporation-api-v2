using API.Base;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.System;

public class HealthController : BaseApiController
{
    // API check API có hoạt động không
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
