using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Lamp.Core;
using Lamp.Services;

[ApiController]
[Route("/")]
public class DomoticASWHttpProtocol : ControllerBase
{
     private readonly ILampService _lampService;
    private readonly LampAgent _lampAgent;
    private readonly BasicLamp _lamp;

    public DomoticASWHttpProtocol(ILampService lampService)
    {
        _lampService = lampService;
        _lampAgent = _lampService.Lamp;
        _lamp = _lampAgent.Lamp;
    }

    [HttpGet("check-status")]
    public IActionResult CheckStatus()
    {
        return Ok(new { IsOn = _lamp.IsOn, Brightness = _lamp.Brightness, Color = _lamp.Color });
    }

    [HttpPost("execute/{deviceActionId}")]
    public IActionResult ExecuteAction(string deviceActionId, [FromBody] ExecuteInput input)
    {
        switch (deviceActionId.ToLower())
        {
            case "switch":
                Console.WriteLine($"Executing action: {deviceActionId}");
                _lamp.Switch();
                return Ok(new { IsOn = _lamp.IsOn });
            case "set-brightness":
                Console.WriteLine($"Executing action: {deviceActionId} with input: {input?.Input}");
                if (input?.Input is JsonElement jsonElement && jsonElement.TryGetInt32(out int value))
                {
                    if (value >= 1 && value <= 100)
                    {
                        _lamp.SetBrightness(value);
                        return Ok(new { Brightness = _lamp.Brightness });
                    }
                    return BadRequest(new { cause = "Brightness must be between 1 and 100" });
                }
                return BadRequest(new { cause = "Invalid input for brightness" });
            case "set-color":
                Console.WriteLine($"Executing action: {deviceActionId} with input: {input?.Input}");
                if (input?.Input is JsonElement colorElement)
                {
                    if (colorElement.TryGetProperty("r", out var rProp) && rProp.ValueKind == JsonValueKind.Number &&
                        colorElement.TryGetProperty("g", out var gProp) && gProp.ValueKind == JsonValueKind.Number &&
                        colorElement.TryGetProperty("b", out var bProp) && bProp.ValueKind == JsonValueKind.Number &&
                        rProp.TryGetInt32(out int r) && gProp.TryGetInt32(out int g) && bProp.TryGetInt32(out int b) &&
                        r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255)
                    {
                        _lamp.SetColor(r, g, b);
                        return Ok(new { Color = _lamp.Color });
                    }
                }
                return BadRequest(new { cause = "Invalid color format. Use format: { \"r\": 255, \"g\": 255, \"b\": 255 }" }); 
            default:
                return NotFound(new { cause = "Unknown action" });
        }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] JsonElement Input)
    {
        int port = Input.GetProperty("serverPort").GetInt32();
        if (port <= 0 || port > 65535)
        {
            return BadRequest(new { cause = "Invalid port number" });
        }
        {
            _lampAgent.SetServerAddress(Request.HttpContext.Connection.RemoteIpAddress!.ToString(), port);
            Console.WriteLine($"Lamp registered at {Request.HttpContext.Connection.RemoteIpAddress}:{port}");
            _lampAgent.Registered = true;
        }
        var device = new
        {
            id = _lamp.Id,
            name = _lamp.Name,
            properties = new object[]
            {
                new {
                    id = "state",
                    name = "State",
                    value = false,
                    setterActionId = "switch",
                },
                new {
                    id = "brightness",
                    name = "Brightness",
                    value = 100,
                    setterActionId = "set-brightness",
                },
                new {
                    id = "color",
                    name = "Color",
                    value = new {
                        r = 255,
                        g = 255,
                        b = 255,
                    },
                    setterActionId = "set-color",
                }
            },
            actions = new object[]
            {
                new {
                    id = "switch",
                    name = "Switch",
                    description = "Switches the lamp on or off",
                    inputTypeConstraints = new {
                        type = "Boolean",
                        constraint = "None"
                    }
                },
                new {
                    id = "set-brightness",
                    name = "Set brightness",
                    description = "Sets the brightness level",
                    inputTypeConstraints = new {
                        constraint = "IntRange",
                        min = 1,
                        max = 100
                    }
                },
                new {
                    id = "set-color",
                    name = "Set color",
                    description = "Changes the lamp color",
                    inputTypeConstraints = new {
                        type = "Color",
                        constraint = "None"
                    }
                }
            },
            events = new[] { "turned-on", "turned-off", "brightness-changed", "color-changed" }
        };

        return Ok(device);
    }

    [HttpPost("unregister")]
    public IActionResult Unregister()
    {
        _lampService.Restart();
        Console.WriteLine($"Lamp unregistered");
        return Ok();
    }

    public class ExecuteInput
    {
        public JsonElement Input { get; set; }
    }
}
