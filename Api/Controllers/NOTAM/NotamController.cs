using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Api.Controllers.NOTAM
{
    internal class Notam
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class NotamController : Controller
    {
        [HttpGet("/notam/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string icao)
        {

            if(icao.Length != 4)
            {
                return Json("Please use a four letter ICAO");
            }

            var html = $@"https://ourairports.com/airports/{icao}/notams.html";
            var web = new HtmlWeb();

            HtmlDocument doc = web.Load(html);

            var notamsRaw = doc.DocumentNode
                .Descendants()
                .Where(x => x.HasClass("col-sm-9"))
                .First()
                .Descendants()
                .Where(x => x.Id.Contains("notam"));

            var notams = notamsRaw
                .Select(
                    x =>
                        new Notam()
                        {
                            Name = Regex.Replace(x.ChildNodes[1].InnerText, "\n", " ").Trim(),
                            Description = Regex.Replace(
                                x.Descendants().Where(x => x.HasClass("notam")).First().InnerHtml,
                                "\n",
                                " "
                            )
                        }
                )
                .ToList();

            return Json(notams, Options.JsonOptions);
        }
    }
}
