namespace Api.Controllers.Vatsim
{
    public class VatsimStatus
    {
        public Status Status { get; set; }
        public Server Server { get; set; }
    }
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
}
