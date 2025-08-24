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
        private readonly string _lanHostname;
        private Thread? _workerThread;
        public BasicLamp Lamp { get; private set; }
        private bool _lastIsOn;
        private int _lastBrightness;
        private ColorType _lastColor;
        private readonly int _devicePort;
        private volatile bool _isRunning;
        public bool Registered { get; set; } = false;

        public LampAgent(ServerCommunicationProtocolHttpAdapter server)
        {
            _lanHostname = Environment.GetEnvironmentVariable("LAN_HOSTNAME") 
               ?? throw new ArgumentException("LAN_HOSTNAME environment variable is not set.");

            _devicePort = int.Parse(Environment.GetEnvironmentVariable("DEVICE_PORT") ?? "8093");
            string? serverAddress = Environment.GetEnvironmentVariable("SERVER_ADDRESS");

            if (serverAddress is not null && serverAddress.Contains(':'))
            {
                string[] parts = serverAddress.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int port))
                {
                    _serverAddress = new ServerAddress(parts[0], port);
                    Registered = true;
                }
                else
                {
                    throw new ArgumentException("Invalid SERVER_ADDRESS format. Expected format: 'host:port'.");
                }
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
                await _server.Announce(_discoveryBroadcastAddress, _devicePort, Lamp.Id, Lamp.Name, _lanHostname);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send announcement: {ex.Message}");
                return false;
            }
        }

        public void Start()
        {
            _isRunning = true;
            _workerThread = new Thread(UpdateAndSend)
            {
                IsBackground = true,
                Name = "LampAgentWorker"
            };
            
            _workerThread.Start();
            Console.WriteLine($"Lamp agent started");
        }

        public void Stop()
        {
            _isRunning = false;
            _workerThread?.Join();
        }

        private async void UpdateAndSend(object? state)
        {
            while (_isRunning)
            {
                if (Lamp.IsOn != _lastIsOn)
                {
                    var evt = Lamp.IsOn ? "turned-on" : "turned-off";
                    await _server.SendEvent(_serverAddress!, evt, Lamp.Id);
                    _lastIsOn = Lamp.IsOn;
                    await _server.UpdateState(_serverAddress!, "state", Lamp.IsOn, Lamp.Id);
                }

                if (Lamp.Brightness != _lastBrightness)
                {
                    await _server.SendEvent(_serverAddress!, "brightness-changed", Lamp.Id);
                    _lastBrightness = Lamp.Brightness;
                    await _server.UpdateState(_serverAddress!, "brightness", Lamp.Brightness, Lamp.Id);
                }

                if (Lamp.Color != _lastColor)
                {
                    await _server.SendEvent(_serverAddress!, "color-changed", Lamp.Id);
                    _lastColor = Lamp.Color;
                    await _server.UpdateState(_serverAddress!, "color", Lamp.Color, Lamp.Id);
                }

                await Task.Delay(200);
            }
        }

        public void SetServerAddress(string host, int port)
        {
            _serverAddress = new ServerAddress(host, port);
        }
    }
}
