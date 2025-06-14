using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class DomoticASWHttpProtocol : ControllerBase
{
    private static bool _isOn = false;
    private static int _brightness = 50;

    [HttpGet("check-status")]
    public IActionResult CheckStatus()
    {
        return Ok(); // If it's reachable, return 200 OK
    }

    [HttpPost("execute/{deviceActionId}")]
    public IActionResult ExecuteAction(string deviceActionId, [FromBody] ExecuteInput input)
    {
        switch (deviceActionId.ToLower())
        {
            case "turn-on":
                _isOn = true;
                return Ok();
            case "turn-off":
                _isOn = false;
                return Ok();
            case "set-brightness":
                if (input?.Input is JsonElement jsonElement && jsonElement.TryGetInt32(out int value))
                {
                    if (value >= 1 && value <= 100)
                    {
                        _brightness = value;
                        return Ok();
                    }
                    return BadRequest("Brightness must be between 1 and 100");
                }
                return BadRequest("Invalid input for brightness");
            default:
                return NotFound("Unknown action");
        }
    }

    [HttpPost("register")]
    public IActionResult Register()
    {
        var device = new
        {
            id = "lamp001",
            name = "Smart Lamp",
            actions = new[]
            {
                new { id = "turn-on", name = "Turn On", inputTypeConstraints = new { }, description = "Turns the lamp on" },
                new { id = "turn-off", name = "Turn Off", inputTypeConstraints = new { }, description = "Turns the lamp off" },
                new { id = "set-brightness", name = "Set Brightness", inputTypeConstraints = new { type = "number", min = 1, max = 100 }, description = "Sets the lamp brightness" }
            },
            events = new object[] { },
            properties = new[]
            {
                new {
                    id = "brightness",
                    name = "Brightness",
                    value = _brightness,
                    setterActionId = "set-brightness"
                }
            }
        };

        return Ok(device);
    }
}
