using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lamp.Ports;

namespace Lamp.Core
{
    public class LampAgent
    {
        private readonly ServerCommunicationProtocolHttpAdapter _server;
        private ServerAddress? _serverAddress;
        private readonly ServerAddress _discoveryBroadcastAddress;
        private Timer? _timer;
        public BasicLamp Lamp { get; private set; }
        private bool _lastIsOn;
        private int _lastBrightness;
        private ColorType _lastColor;
        private readonly int _devicePort;
        public bool Registered { get; set; } = false;

        public LampAgent(ServerCommunicationProtocolHttpAdapter server)
        {
            _discoveryBroadcastAddress = new ServerAddress("255.255.255.255", 30000);
            _devicePort = int.Parse(Environment.GetEnvironmentVariable("DEVICE_PORT") ?? "8080");
            string? serverAddress = Environment.GetEnvironmentVariable("SERVER_ADDRESS");
            string? serverPort = Environment.GetEnvironmentVariable("SERVER_PORT");

            if (serverAddress is not null && serverPort is not null)
            {
                _serverAddress = new ServerAddress(serverAddress, int.Parse(serverPort));
                Registered = true;
            }

            string? discoveryAddress = Environment.GetEnvironmentVariable("DISCOVERY_ADDRESS");
            string? discoveryPort = Environment.GetEnvironmentVariable("DISCOVERY_PORT");

            if (discoveryAddress is not null && discoveryPort is not null)
            {
                _discoveryBroadcastAddress = new ServerAddress(discoveryAddress, int.Parse(discoveryPort));
            }
            else
            {
                _discoveryBroadcastAddress = new ServerAddress("255.255.255.255", 30000);
            }            

            _server = server;
            Lamp = new BasicLamp();
            _lastIsOn = Lamp.IsOn;
            _lastBrightness = Lamp.Brightness;
            _lastColor = Lamp.Color;
            _ = AnnouncePresenceAsync();
        }

        public async Task<bool> AnnouncePresenceAsync()
        {
            try
            {
                await _server.Announce(_discoveryBroadcastAddress, _devicePort, Lamp.Id, Lamp.Name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send announcement: {ex.Message}");
                return false;
            }
        }

        public void Start(TimeSpan interval)
        {
            _timer = new Timer(UpdateAndSend, null, TimeSpan.Zero, interval);
            Console.WriteLine($"Lamp agent started. Sending updates every {interval.TotalSeconds} seconds.");
        }

        public void Stop()
        {
            _timer?.Dispose();
        }

        private async void UpdateAndSend(object? state)
        {
            if (Lamp.IsOn != _lastIsOn)
            {
                var evt = Lamp.IsOn ? "turned-on" : "turned-off";
                await _server.SendEvent(_serverAddress!, evt, Lamp.Id);
                _lastIsOn = Lamp.IsOn;
            }

            if (Lamp.Brightness != _lastBrightness)
            {
                await _server.SendEvent(_serverAddress!, "brightness-changed", Lamp.Id);
                _lastBrightness = Lamp.Brightness;
            }

            if (Lamp.Color != _lastColor)
            {
                await _server.SendEvent(_serverAddress!, "color-changed", Lamp.Id);
                _lastColor = Lamp.Color;
            }

            await _server.UpdateState(_serverAddress!, "turned-on", Lamp.IsOn, Lamp.Id);
            await _server.UpdateState(_serverAddress!, "brightness", Lamp.Brightness, Lamp.Id);
            await _server.UpdateState(_serverAddress!, "color", Lamp.Color, Lamp.Id);
        }

        public void SetServerAddress(string host, int port)
        {
            _serverAddress = new ServerAddress(host, port);
        }
    }
}
