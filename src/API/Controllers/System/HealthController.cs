namespace API;

public class HealthController : BaseApiController
{
    // API check API có hoạt động không
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}