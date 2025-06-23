using Lamp.Core;

namespace Lamp.Services;

public class LampService : ILampService
{
    public LampAgent Lamp { get; } = new LampAgent(new Ports.ServerCommunicationProtocolHttpAdapter(), new BasicLamp());
}
