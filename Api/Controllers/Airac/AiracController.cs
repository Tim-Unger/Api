using HtmlAgilityPack;
using System.Diagnostics;

namespace Api.Controllers.Airac
{
    [Route("api")]
    [ApiController]
    public class AiracController : Controller
    {
        [HttpGet("/airacs")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult Get() => Json(Airacs.Get());

        [HttpGet("/airacs/current")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetCurrent()
        {
            var airacs = Airacs.Get();

            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

            for (var i = 0; i < airacs.Count; i++)
            {
                if (airacs[i + 1].StartDate > dateNow)
                {
                    return Json(airacs[i]);
                }
            }

            throw new UnreachableException();
        }

        [HttpGet("/airacs/next")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetNext()
        {
            var airacs = Airacs.Get();

            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

            for (var i = 0; i < airacs.Count; i++)
            {
                if (airacs[i + 1].StartDate > dateNow)
                {
                    return Json(airacs[i + 1]);
                }
            }

            //If you are here then you are somehow later in time than the publication of any airac (or you have time travelled)
            throw new UnreachableException("You are either a time traveller or there are no published AIRAC Cycles");
        }
    }
}
