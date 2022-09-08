using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using static Json.Json;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Route("Betriebsrichtung")]
    [ApiController]
    public class BetriebsrichtungJson
    {
        public string Betriebsrichtung { get; set; }
    }
    public class BetriebsrichtungController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet("/betriebsrichtung")]
        [EnableCors("AllowOrigin")]
        public JsonResult Betriebsrichtung()
        {
            string Br = GetBetriebsrichtung.Betriebsrichtung();

            BetriebsrichtungJson ReturnBetriebsrichtung = new BetriebsrichtungJson
            {
                Betriebsrichtung = Br
            };
            return Json(ReturnBetriebsrichtung);
        }
    }
}
