using HtmlAgilityPack;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

            //If you are here then you are somehow later in time than the publication of any airac (or you have time travelled)
            throw new UnreachableException(
                "You are either a time traveller or there are no published AIRAC Cycles"
            );
        }

        [HttpGet("/airacs/next")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetNext()
        {
            var airacs = Airacs.Get();

            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dateNow > airacs.Last().StartDate)
            {
                return Json("The next Airac is not available yet");
            }

            for (var i = 0; i < airacs.Count; i++)
            {
                if (airacs[i + 1].StartDate > dateNow)
                {
                    return Json(airacs[i + 1]);
                }
            }

            //If you are here then you are somehow later in time than the publication of any airac (or you have time travelled)
            throw new UnreachableException(
                "You are either a time traveller or there are no published AIRAC Cycles"
            );
        }

        [HttpGet("/airacs/ident/{ident}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByIdent(int ident) =>
            Json(Airacs.Get().Where(x => x.Ident == ident).FirstOrDefault()) ?? Json("Ident not found");

        [HttpGet("/airacs/year/{year}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByYear(int year) => Json(Airacs.Get().Where(x => x.StartDate.Year == year).ToList()) ?? Json("No Airacs use this year");

        [HttpGet("/airacs/date/{date}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public JsonResult GetByDate(string date)
        {
            var airacs = Airacs.Get();

            var dateRegex = new Regex(
               @"(20[2-3][0-9])(?>_|-|)?(0[1-9]|1[0-2])(?>_|-|)?(0[1-9]|1[0-9]|2[0-9]|3[0-1])");

            if (!dateRegex.IsMatch(date))
            {
                return Json("Inputted date was not valid, please use an ISO9601 compliant date (20231231)");
            }

            var groups = dateRegex.Match(date).Groups;

            var year = int.Parse(groups[1].Value);

            var month = int.Parse(groups[2].Value);

            var day = int.Parse(groups[3].Value);

            var dateOnly = new DateOnly(year, month, day);

            return Json(airacs.Where(x => x.StartDate < dateOnly && x.EndDate > dateOnly).FirstOrDefault()) ?? Json("Date has no Airac");
        }
    }
}
