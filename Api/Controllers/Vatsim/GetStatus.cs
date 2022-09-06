using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using static Json.Json;

namespace Api.Controllers.Vatsim
{
    public class Status
    {
        public Data data { get; set; }
        public string[] user { get; set; }
        public string[] metar { get; set; }
    }

    public class Data
    {
        public string[] v3 { get; set; }
        public string[] transceivers { get; set; }
        public string[] servers { get; set; }
        public string[] servers_sweatbox { get; set; }
        public string[] servers_all { get; set; }
    }

    public class Server
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public bool Operational { get; set; }
        public int Connections { get; set; }
    }

    public class VatsimServer
    {
        public string ident { get; set; }
        public string hostname_or_ip { get; set; }
        public string location { get; set; }
        public string name { get; set; }
        public int clients_connection_allowed { get; set; }
        public bool client_connections_allowed { get; set; }
        public bool is_sweatbox { get; set; }
    }

    public class CountServer
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class GetStatus
    {
        public static void Status()
        {
            List<string> ServerLinks = new List<string>();
            Uri VatsimData = new Uri("https://status.vatsim.net/status.json", UriKind.Absolute);
            WebClient wc = new WebClient();

            Stream DataStream = wc.OpenRead(VatsimData);
            StreamReader ReadData = new StreamReader(DataStream);
            string Data = ReadData.ReadToEnd();

            Status Root = JsonConvert.DeserializeObject<Status>(Data);
            foreach (var DataUrl in Root.data.servers)
            {
                ServerLinks.Add(DataUrl);
            }

            Random R = new Random();

            int ServerIndex = R.Next(0, ServerLinks.Count);

            WebClient wc2 = new WebClient();
            byte[] raw = wc.DownloadData(ServerLinks[ServerIndex]);
            string ServersRaw = Encoding.UTF8.GetString(raw);

            var VatsimServer = JsonConvert.DeserializeObject<List<VatsimServer>>(ServersRaw);

            Rootobject Raw = GetData.Deserialize();

            List<string> ServerCount = new List<string>();

            foreach (var Pilot in Raw.pilots)
            {
                ServerCount.Add(Pilot.server);
            }

            foreach (var Controller in Raw.controllers)
            {
                ServerCount.Add(Controller.server);
            }

            foreach (var Atis in Raw.atis)
            {
                ServerCount.Add(Atis.server);
            }

            var CountServers = ServerCount.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count);

            List<Server> Servers = new List<Server>();
            bool IsOperational = false;

            foreach (var Server in VatsimServer)
            {
                Ping PingServer = new Ping();
                PingReply PingServerReply = PingServer.Send(Server.hostname_or_ip);

                if (PingServerReply.Status == IPStatus.Success)
                {
                    IsOperational = true;
                }

                foreach (var CurrentServer in CountServers)
                {
                    if (CurrentServer.Value == Server.name)
                    {
                        Server NewServer = new Server { Name = Server.name, Ip = Server.hostname_or_ip, Operational = IsOperational, Connections = CurrentServer.Count};
                        Servers.Add(NewServer);
                        break;
                    }
                }
                
            }


            Controllers.Data.Servers = Servers;
        }
    }
}
