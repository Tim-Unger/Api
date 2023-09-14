using System.Net.NetworkInformation;

namespace Api.Controllers.Vatsim
{
    internal class CountServer
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class GetStatus : Controller
    {
        //TODO

        private static bool IsServerOperational(string ip) => new Ping().Send(ip).Status == IPStatus.Success;
    }
}
