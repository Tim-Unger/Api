using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BetriebsrichtungController : ControllerBase
    {
        public static string Betriebsrichtung { get; set; }
        private static void GetBetriebsrichtung()
        {
            //HtmlWeb client = new HtmlWeb();
            //HtmlDocument doc = client.Load("https://www.fraport.com/de/umwelt/schallschutz/flugbetrieb--verfahren/betriebsrichtung.html");

            //string tag = doc.GetElementbyId("betriebsrichtung");
        }
        
        [HttpGet(Name = "GetBetriesrichtung")]
        public string Get()
        {
            GetBetriebsrichtung();
            return Betriebsrichtung;
        }
    }
}
