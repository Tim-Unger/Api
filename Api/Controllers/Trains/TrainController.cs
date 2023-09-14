using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Api.Controllers
{
    public static class IntExtensions
    {
        public static bool IsBetween(this int value, int lowerBound, int upperBound) =>
            upperBound > lowerBound
                ? value >= lowerBound && value <= upperBound
                : throw new ArgumentOutOfRangeException();
    }

    [Route("api")]
    [ApiController]
    public class TrainController : Controller
    {
        //TODO
        //[HttpGet("/train/{trainNumber}")]
        //[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        //[Produces("application/json")]
        private JsonResult GetTrain(string trainNumber = "")
        {
            var isJustNumber = int.TryParse(trainNumber, out var number);

            var trainNumberInt = 0;
            if (!isJustNumber)
            {
                var trainNumberArray = trainNumber.ToCharArray();

                var trainType = string.Concat(trainNumberArray.Where(char.IsLetter));
                trainNumberInt = int.Parse(string.Concat(trainNumberArray.Where(char.IsNumber)));
            }

            var numberParsed = isJustNumber ? int.Parse(trainNumber) : trainNumberInt;

            //TODO automate
            var numberRangeIndex = numberParsed switch
            {
                int when numberParsed.IsBetween(2, 99) => "2-99",
                int when numberParsed.IsBetween(100, 199) => "100-199",
                int when numberParsed.IsBetween(200, 299) => "200-299",
                int when numberParsed.IsBetween(300, 399) => "300-399",
                int when numberParsed.IsBetween(400, 499) => "400-499",
                int when numberParsed.IsBetween(500, 599) => "500-599",
                int when numberParsed.IsBetween(600, 699) => "600-699",
                int when numberParsed.IsBetween(700, 799) => "700-799",
                int when numberParsed.IsBetween(800, 899) => "800-899",
                int when numberParsed.IsBetween(900, 999) => "900-999",
                int when numberParsed.IsBetween(1000, 1099) => "1000-1099",
                int when numberParsed.IsBetween(1100, 1199) => "1100-1199",
                int when numberParsed.IsBetween(1200, 1299) => "1200-1299",
                int when numberParsed.IsBetween(1500, 1599) => "1500-1599",
                int when numberParsed.IsBetween(1600, 1699) => "1600-1699",
                int when numberParsed.IsBetween(1700, 1799) => "1700-1799",
                int when numberParsed.IsBetween(1900, 1999) => "1900-1999",
                int when numberParsed.IsBetween(2000, 2099) => "2000-2099",
                int when numberParsed.IsBetween(2100, 2199) => "2100-12199",
                int when numberParsed.IsBetween(2200, 2299) => "2200-2299",
                int when numberParsed.IsBetween(2300, 2399) => "2300-2399",
                int when numberParsed.IsBetween(2400, 2599) => "2400-2599",
                int when numberParsed.IsBetween(9500, 9599) => "9500-9599",
                _ => throw new ArgumentOutOfRangeException()
            };

            var html =
                $"https://www.fernbahn.de/datenbank/suche/?fahrplan_jahr=2023&zug_bereich=&zug_gattung=&zug_nummer={numberRangeIndex}&zug_linie=&ice_typ=&wagengattung=&wagengattung_suche=AND&fv_suche_reihungsverzeichnis=1#fv_suche_reihungsverzeichnis";

            var web = new HtmlWeb();

            HtmlDocument doc = web.Load(html);

            //var trains = doc.GetElementbyId("reihungsverzeichnis-liste")
            //   .ChildNodes.Where(x => x.HasClass("zugnr"));
            //.First(x => x.InnerText.Contains(numberParsed.ToString(), StringComparison.InvariantCultureIgnoreCase));


            var correctTrain = doc.DocumentNode
                .DescendantNodes()
                .Where(x => x.HasClass("zugnr"))
                .FirstOrDefault(
                    x => x.ChildNodes.First().InnerText.Contains(numberParsed.ToString())
                );

            if (correctTrain is null)
            {
                return Json("Train does not exist", Options.JsonOptions);
            }

            var train = new Train();

            var fullInfoHtml = correctTrain.ParentNode;

            train.FullTrainName = Regex.Replace(correctTrain.InnerText, "(\\t)*", string.Empty);

            train.TrainNumber = int.Parse(
                string.Concat(train.FullTrainName.ToCharArray().Where(char.IsDigit))
            );

            var rollingStock = Regex.Replace(
                fullInfoHtml.Descendants().First(x => x.HasClass("tfz")).InnerText,
                "(\\t)*",
                string.Empty
            );

            train.RollingStock = rollingStock == @"\t\t" ? null : rollingStock;

            var fromTo = fullInfoHtml.Descendants().First(x => x.HasClass("vonbis")).InnerText;

            GroupCollection splitFromToGroups = Regex.Match(fromTo, @"(.*)&mdash;(.*)").Groups;

            train.Departure = splitFromToGroups[1].Value.Trim();

            train.Destination = splitFromToGroups[2].Value.Trim();

            train.Cars = fullInfoHtml
                .Descendants()
                .First(x => x.HasClass("wagenreihung"))
                .ChildNodes.Where(x => x.HasClass("wagen"))
                .Select(
                    x =>
                        new TrainCar()
                        {
                            Number = int.Parse(
                                x.ChildNodes.First(x => x.HasClass("wagen-nummer")).InnerText
                            ),
                            Type = x.ChildNodes.First(x => x.HasClass("wagen-gattung")).InnerText
                        }
                )
                .ToList();

            train.TopSpeedKpH = int.Parse(
                fullInfoHtml.Descendants().First(x => x.HasClass("vmax")).FirstChild.InnerText
            );

            return Json(train, Options.JsonOptions);
        }
    }
}
