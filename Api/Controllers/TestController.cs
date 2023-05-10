using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Api.Controllers
{
    public class Dns
    {
        public string DnsAdapter { get; set; }
    }
    public class TestController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet("/test")]
        public JsonResult Test()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            List<Dns> Adapters = new List<Dns>();
            foreach (NetworkInterface adapter in adapters)
            {

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                if (dnsServers.Count > 0)
                {
                    Console.WriteLine(adapter.Description);
                    foreach (IPAddress dns in dnsServers)
                    {
                        Dns DnsClass = new Dns
                        {
                            DnsAdapter = dns.ToString()
                        };
                        Adapters.Add(DnsClass);
                    }
                }
            }

            return Json(Adapters);

        }
    }
}
