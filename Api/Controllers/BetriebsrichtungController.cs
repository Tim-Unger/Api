using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;

namespace Api.Controllers
{
    [Route("betriebsrichtung")]
    [ApiController]
    public class BetriebsrichtungController : ControllerBase
    {
        public static string Betriebsrichtung { get; set; }
        
        [HttpGet(Name = "GetBetriesrichtung")]
        public string Get()
        {
            GetBetriebsrichtung.Betriebsrichtung();
            return Betriebsrichtung;
        }
    }
}
