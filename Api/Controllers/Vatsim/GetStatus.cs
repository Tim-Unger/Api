using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json;
using static Json.Json;

namespace Api.Controllers.Vatsim
{
    internal class CountServer
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    internal class GetStatus
    {
        //TODO
        /// <summary>
        /// Get the Vatsim-Status by pinging all Servers
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task Status()
        {
            var serverLinks = new List<string>();

            var client = new HttpClient();
            var data = await client.GetStringAsync("https://status.vatsim.net/status.json");

            var root = JsonSerializer.Deserialize<Status>(data);

            root!.data.servers.ToList().ForEach(serverLinks.Add);

            var random = new Random();

            var randomServerIndex = random.Next(0, serverLinks.Count);
           
            var servers = await client.GetStringAsync(serverLinks[randomServerIndex]);

            var vatsimServer = JsonSerializer.Deserialize<List<VatsimServer>>(servers) ?? throw new Exception();
            Rootobject json = GetData.Deserialize();

            var serverCount = new List<string>();

            foreach (var pilot in json.pilots)
            {
                serverCount.Add(pilot.server);
            }

            foreach (var controller in json.controllers)
            {
                serverCount.Add(controller.server);
            }

            foreach (var atis in json.atis)
            {
                serverCount.Add(atis.server);
            }

            var countServers = serverCount.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count);

            var serversList = new List<Server>();
            var isOperational = false;

            foreach (var server in vatsimServer)
            {
                var pingServer = new Ping();
                PingReply pingServerReply = pingServer.Send(server.hostname_or_ip);

                if (pingServerReply.Status == IPStatus.Success)
                {
                    isOperational = true;
                }

                foreach (var currentServer in countServers)
                {
                    if (currentServer.Value == server.name)
                    {
                        var NewServer = new Server { Name = server.name, Ip = server.hostname_or_ip, Operational = isOperational, Connections = currentServer.Count};
                        serversList.Add(NewServer);
                        break;
                    }
                }
                
            }

            Controllers.Data.Servers = serversList;
        }
    }
}
