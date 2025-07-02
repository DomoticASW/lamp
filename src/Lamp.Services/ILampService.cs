using Lamp.Core;

namespace Lamp.Services;

public interface ILampService
{
    LampAgent Lamp { get; }
    bool IsRunning { get; }
}
