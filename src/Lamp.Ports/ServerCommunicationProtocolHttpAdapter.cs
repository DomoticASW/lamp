using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lamp.Ports
{
    public class ServerCommunicationProtocolHttpAdapter : IServerCommunicationProtocol
    {
        private readonly HttpClient _httpClient;

        public ServerCommunicationProtocolHttpAdapter()
        {
            _httpClient = new HttpClient();
        }

        public async Task SendEvent(ServerAddress serverAddress, string eventName, string deviceId)
        {
            var url = $"http://{serverAddress.Host}:{serverAddress.ServerPort}/api/devices/{deviceId}/events";
            var payload = JsonSerializer.Serialize(new { @event = eventName });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            Console.WriteLine($"CLIENT: Sending event to {url}");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                Console.WriteLine($"CLIENT: Event response: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CLIENT ERROR: {ex.Message}");
            }
        }

        public async Task UpdateState(ServerAddress serverAddress, string propertyName, object propertyValue, string deviceId)
        {
            var url = $"http://{serverAddress.Host}:{serverAddress.ServerPort}/api/devices/{deviceId}/properties/{propertyName}";
            var payload = JsonSerializer.Serialize(new { value = propertyValue });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            Console.WriteLine($"CLIENT: Updating state {propertyName}={propertyValue} to {url}");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                Console.WriteLine($"CLIENT: Update response: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CLIENT ERROR: {ex.Message}");
            }
        }
        
        public async Task Announce(ServerAddress discoveryBroadcastAddress, int devicePort, string lampId, string lampName)
        {
            var message = new 
            {
                id = lampId,
                name = lampName,
                port = devicePort
            };
            string broadcastIp = discoveryBroadcastAddress.Host;
            int broadcastPort = discoveryBroadcastAddress.ServerPort;

            using var udpClient = new UdpClient();
            try
            {
                udpClient.EnableBroadcast = true;

                udpClient.Client.ReceiveTimeout = 1000;
                udpClient.Client.SendTimeout = 1000;

                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] data = Encoding.UTF8.GetBytes(jsonMessage);

                await udpClient.SendAsync(data, data.Length, broadcastIp, broadcastPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during broadcast: {ex.Message}");
            }
        }
    }
}
