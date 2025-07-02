using Lamp.Core;
using Lamp.Ports;
using Microsoft.Extensions.Hosting;

namespace Lamp.Services;

public class LampService : ILampService, IHostedService
{
    public LampAgent Lamp { get; }
    public bool IsRunning { get; private set; }
    private readonly CancellationTokenSource _cts = new();

    public LampService()
    {
        Lamp = new LampAgent(new ServerCommunicationProtocolHttpAdapter());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (IsRunning) return Task.CompletedTask;

        else
        {
            Console.WriteLine("Lamp not registered, starting presence announcement loop");
            _ = Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested && !Lamp.Registered)
                {
                    try
                    {
                        await Lamp.AnnouncePresenceAsync();
                        await Task.Delay(5000, _cts.Token);
                        Console.WriteLine("Lamp presence announcement sent");
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }
                if (Lamp.Registered)
                {
                    Lamp.Start(TimeSpan.FromSeconds(30));
                }
            }, _cts.Token);
        }
        IsRunning = true;
        return Task.CompletedTask;
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!IsRunning) return Task.CompletedTask;
        
        _cts.Cancel();
        Lamp.Stop();
        IsRunning = false;
        return Task.CompletedTask;
    }
}
