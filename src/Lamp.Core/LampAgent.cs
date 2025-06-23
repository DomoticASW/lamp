using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Lamp.Ports;

namespace Lamp.Core
{
    public class LampAgent
    {
        private readonly HttpClient _httpClient = new();
        private readonly ServerCommunicationProtocolHttpAdapter _server;
        private ServerAddress _serverAddress = new(
            Environment.GetEnvironmentVariable("SERVER_ADDRESS") ?? "",
            int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT") ?? "")
        );
        private Timer? _timer;
        private bool _lastIsOn;
        private int _lastBrightness;
        private string _lastColorHex;
        public BasicLamp lamp = new();

        public LampAgent(ServerCommunicationProtocolHttpAdapter server)
        {
            _server = server;
            _lastIsOn = lamp.IsOn;
            _lastBrightness = lamp.Brightness;
            _lastColorHex = lamp.ColorHex;
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
            if (lamp.IsOn != _lastIsOn)
            {
                var evt = lamp.IsOn ? "turned-on" : "turned-off";
                await _server.SendEvent(_serverAddress, evt, lamp.Id);
                _lastIsOn = lamp.IsOn;
            }

            if (lamp.Brightness != _lastBrightness)
            {
                await _server.SendEvent(_serverAddress, "brightness-changed", lamp.Id);
                _lastBrightness = lamp.Brightness;
            }

            if (lamp.ColorHex != _lastColorHex)
            {
                await _server.SendEvent(_serverAddress, "color-changed", lamp.Id);
                _lastColorHex = lamp.ColorHex;
            }

            await _server.UpdateState(_serverAddress, "state", lamp.IsOn, lamp.Id);
            await _server.UpdateState(_serverAddress, "brightness", lamp.Brightness, lamp.Id);
            await _server.UpdateState(_serverAddress, "color", lamp.ColorHex, lamp.Id);
        }

        public void SetServerAddress(string host, int port)
        {
            _serverAddress = new ServerAddress(host, port);
        }
    }
}
