using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Lamp.Core;
using Lamp.Services;

[ApiController]
[Route("/")]
public class DomoticASWHttpProtocol(ILampService lampService) : ControllerBase
{
    private readonly BasicLamp lamp = lampService.Lamp;

    [HttpGet("check-status")]
    public IActionResult CheckStatus()
    {
        return Ok(new { IsOn = lamp.IsOn, Brightness = lamp.Brightness, Color = lamp.ColorHex });
    }

    [HttpPost("execute/{deviceActionId}")]
    public IActionResult ExecuteAction(string deviceActionId, [FromBody] ExecuteInput input)
    {
        switch (deviceActionId.ToLower())
        {
            case "turn-on":
                lamp.TurnOn();
                return Ok(new { IsOn = lamp.IsOn });
            case "turn-off":
                lamp.TurnOff();
                return Ok(new { IsOn = lamp.IsOn });
            case "set-brightness":
                if (input?.Input is JsonElement jsonElement && jsonElement.TryGetInt32(out int value))
                {
                    if (value >= 1 && value <= 100)
                    {
                        lamp.SetBrightness(value);
                        return Ok(new { Brightness = lamp.Brightness });
                    }
                    return BadRequest("Brightness must be between 1 and 100");
                }
                return BadRequest("Invalid input for brightness");
            case "set-color":
                if (input?.Input is JsonElement colorElement && colorElement.ValueKind == JsonValueKind.String)
                {
                    string? color = colorElement.GetString();
                    if (color != null && System.Text.RegularExpressions.Regex.IsMatch(color, "^#([0-9A-Fa-f]{6})$"))
                    {
                        lamp.SetColor(color.Trim());
                        return Ok(new { Color = lamp.ColorHex });
                    }
                    return BadRequest("Invalid hex color format. Use format: #RRGGBB");
                }
                return BadRequest("Invalid input for color");
            default:
                return NotFound("Unknown action");
        }
    }

    [HttpPost("register")]
    public IActionResult Register()
    {
        var device = new
        {
            id = "lamp-001",
            name = "Smart Lamp",
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
                    typeConstraints = new {
                        constraint = "IntRange",
                        min = 1,
                        max = 100
                    }
                },
                new {
                    id = "color",
                    name = "Color",
                    value = "#FF00FF",
                    typeConstraints = new {
                        type = "String",
                        constraint = "None"
                    }
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
                        type = "String",
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
