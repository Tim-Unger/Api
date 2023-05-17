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
    internal class BetriebsrichtungJson
    {
        internal string? Betriebsrichtung { get; set; }
    }

    public class BetriebsrichtungController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/betriebsrichtung")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [EnableCors("AllowOrigin")]
        internal JsonResult Betriebsrichtung()
        {
            bool is25 = GetBetriebsrichtung.Betriebsrichtung() == "25";

            var ReturnBetriebsrichtung = new Betriebsrichtung
            {
                Is25 = is25
            };

            return Json(ReturnBetriebsrichtung);
        }
    }
}
