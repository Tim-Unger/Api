using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using static Json.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class BetriebsrichtungJson
    {
        public string? Betriebsrichtung { get; set; }
    }

    public class BetriebsrichtungController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet("/betriebsrichtung")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        public JsonResult Betriebsrichtung()
        {
            string br = GetBetriebsrichtung.Betriebsrichtung();

            BetriebsrichtungJson ReturnBetriebsrichtung = new BetriebsrichtungJson
            {
                Betriebsrichtung = br
            };
            return Json(ReturnBetriebsrichtung);
        }
    }
}
