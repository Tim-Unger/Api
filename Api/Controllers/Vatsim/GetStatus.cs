using System.Net.NetworkInformation;

namespace Api.Controllers.Vatsim
{
    internal class CountServer
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class GetStatus : Microsoft.AspNetCore.Mvc.Controller
    {
        //TODO
        /// <summary>
        /// Get the Vatsim-Status by pinging all Servers
        /// </summary>
        /// <returns>a JSON of all current Vatsim-Servers</returns>
        /// <exception cref="Exception"></exception>
        //public async Task<JsonResult> Status()
        //{
        //    var serverLinks = new List<string>();

        //    var client = new HttpClient();
        //    var data = await client.GetStringAsync("https://status.vatsim.net/status.json");

        //    var root = JsonSerializer.Deserialize<Status>(data);

        //    root!.data.servers.ToList().ForEach(serverLinks.Add);

        //    var random = new Random();

        //    var randomServerIndex = random.Next(0, serverLinks.Count);
           
        //    var servers = await client.GetStringAsync(serverLinks[randomServerIndex]);

        //    var vatsimServer = JsonSerializer.Deserialize<List<VatsimServer>>(servers) ?? throw new Exception();

        //    var vatsimData = await client.GetFromJsonAsync<Json.Json.VatsimData>("https://data.vatsim.net/v3/vatsim-data.json");

        //    if(vatsimData == null)
        //    {
        //        return Json("Vatsim-Data could not be read");
        //    }

        //    var serverCountList = new List<string>();

        //    vatsimData.Pilots.ForEach(x => serverCountList.Add(x.server));
        //    vatsimData.Controllers.ForEach(x => serverCountList.Add(x.server));
        //    vatsimData.Atis.ForEach(x => serverCountList.Add(x.server));

        //    //Counts how often each server appears and then sorts them by largest first
        //    var countServers = serverCountList.GroupBy(x => x).Select(x => new { Value = x.Key, Count = x.Count() }).OrderByDescending(x => x.Count);

        //    var serversList = new List<Server>();
        //    var isOperational = false;


        //    serversList.Add(new Server() { Name = "Vatsim Website", Connections = null, Operational = IsServerOperational("https://vatsim.net") });
        //    serversList.Add(new Server() { Name = "Vatsim Forum", Connections = null, Operational = IsServerOperational("https://forum.vatsim.net") });
        //    serversList.Add(new Server() { Name = "Vatsim Metar", Connections = null, Operational = IsServerOperational("https://metar.vatsim.net") });
        //    serversList.Add(new Server() { Name = "Vatsim Data", Connections = null, Operational = IsServerOperational("https://status.vatsim.net/status.json") });

        //    foreach (var server in vatsimServer)
        //    {

        //        isOperational = IsServerOperational(server.hostname_or_ip);

        //        foreach (var currentServer in countServers)
        //        {
        //            if (currentServer.Value == server.name)
        //            {
        //                var NewServer = new Server { Name = server.name, Operational = isOperational, Connections = currentServer.Count};
        //                serversList.Add(NewServer);
        //                break;
        //            }
        //        }
        //    }

        //    return Json(serversList);
        //}

        private static bool IsServerOperational(string ip)
        {
            var pingServer = new Ping();
            PingReply pingServerReply = pingServer.Send(ip);

            return pingServerReply.Status == IPStatus.Success;
        }
    }
}
