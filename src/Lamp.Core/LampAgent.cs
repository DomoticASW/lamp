using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Lamp.Ports;

namespace Lamp.Core
{
    public class LampAgent(ServerCommunicationProtocolHttpAdapter server, BasicLamp lamp)
    {
        private readonly HttpClient _httpClient = new();
        private readonly ServerCommunicationProtocolHttpAdapter _server = server;
        private ServerAddress _serverAddress = new(
            Environment.GetEnvironmentVariable("SERVER_ADDRESS") ?? null,
            int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT") ?? null)
        );
        private Timer? _timer;
        private bool _lastIsOn;
        private int _lastBrightness;
        private string _lastColorHex;
        public BasicLamp lamp = new();

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
            if (_lamp.IsOn != _lastIsOn)
            {
                var evt = _lamp.IsOn ? "turned-on" : "turned-off";
                await _server.SendEvent(_serverAddress, evt, _lamp.Id);
                _lastIsOn = _lamp.IsOn;
            }

            if (_lamp.Brightness != _lastBrightness)
            {
                await _server.SendEvent(_serverAddress, "brightness-changed", _lamp.Id);
                _lastBrightness = _lamp.Brightness;
            }

            if (_lamp.ColorHex != _lastColorHex)
            {
                await _server.SendEvent(_serverAddress, "color-changed", _lamp.Id);
                _lastColorHex = _lamp.ColorHex;
            }

            await _server.UpdateState(_serverAddress, "state", _lamp.IsOn, _lamp.Id);
            await _server.UpdateState(_serverAddress, "brightness", _lamp.Brightness, _lamp.Id);
            await _server.UpdateState(_serverAddress, "color", _lamp.ColorHex, _lamp.Id);
        }

        public void SetServerAddress(string host, int port)
        {
            _serverAddress = new ServerAddress(host, port);
        }
    }
}
