using Lamp.Models;

namespace Lamp.Services;

public class LampService : ILampService
{
    public BasicLamp Lamp { get; } = new BasicLamp();
}
