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

    internal class BetriebsrichtungController : Microsoft.AspNetCore.Mvc.Controller
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
            string br = GetBetriebsrichtung.Betriebsrichtung();

            var ReturnBetriebsrichtung = new BetriebsrichtungJson
            {
                Betriebsrichtung = br
            };
            return Json(ReturnBetriebsrichtung);
        }
    }
}
