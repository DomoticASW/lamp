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
            case "turn-on":
                Console.WriteLine($"Executing action: {deviceActionId}");
                _lamp.TurnOn();
                return Ok(new { IsOn = _lamp.IsOn });
            case "turn-off":
                Console.WriteLine($"Executing action: {deviceActionId}");
                _lamp.TurnOff();
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
                    return BadRequest("Brightness must be between 1 and 100");
                }
                return BadRequest("Invalid input for brightness");
            case "set-color":
                Console.WriteLine($"Executing action: {deviceActionId} with input: {input?.Input}");
                if (input?.Input is JsonElement colorElement)
                {
                    if (colorElement.TryGetProperty("r", out var rProp) && rProp.ValueKind == JsonValueKind.Number &&
                        colorElement.TryGetProperty("g", out var gProp) && gProp.ValueKind == JsonValueKind.Number &&
                        colorElement.TryGetProperty("b", out var bProp) && bProp.ValueKind == JsonValueKind.Number &&
                        rProp.TryGetInt32(out int r) && gProp.TryGetInt32(out int g) && bProp.TryGetInt32(out int b))
                    {
                        _lamp.SetColor(r, g, b);
                        return Ok(new { Color = _lamp.Color });
                    }
                }
                return BadRequest("Invalid color format. Use format: { \"r\": 255, \"g\": 255, \"b\": 255 }");
            default:
                return NotFound("Unknown action");
        }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] JsonElement Input)
    {
        int port = Input.GetProperty("serverPort").GetInt32();
        if (port <= 0 || port > 65535)
        {
            return BadRequest("Invalid port number");
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
                    typeConstraints = new {
                        type = "Boolean",
                        constraint = "None"
                    }
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
                    id = "turn-on",
                    name = "Turn On",
                    description = "Turns the lamp on",
                    inputTypeConstraints = new {
                        type = "Void",
                        constraint = "None"
                    }
                },
                new {
                    id = "turn-off",
                    name = "Turn Off",
                    description = "Turns the lamp off",
                    inputTypeConstraints = new {
                        type = "Void",
                        constraint = "None"
                    }
                },
                new {
                    id = "set-brightness",
                    name = "Set Brightness",
                    description = "Sets the brightness level",
                    inputTypeConstraints = new {
                        constraint = "IntRange",
                        min = 1,
                        max = 100
                    }
                },
                new {
                    id = "set-color",
                    name = "Set Color",
                    description = "Changes the lamp color",
                    inputTypeConstraints = new {
                        type = "ColorType",
                        constraint = "None"
                    }
                }
            },
            events = new[] { "turned-on", "turned-off", "brightness-changed", "color-changed" }
        };

        return Ok(device);
    }

    public class ExecuteInput
    {
        public JsonElement Input { get; set; }
    }
}
