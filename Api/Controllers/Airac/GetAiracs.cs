using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Api.Controllers.Airac
{
    public class Airac
    {
        public int CycleNumberInYear { get; init; }
        public int Ident { get; init; }
        public DateOnly StartDate { get; init; }
        //public DateOnly EndDate { get; init; }
    }

    public class Airacs
    {
        public static List<Airac> Get()
        {
            var html = @"https://www.nm.eurocontrol.int/RAD/common/airac_dates.html";
            var web = new HtmlWeb();

            HtmlDocument doc = web.Load(html);

            //Get all table nodes
            var nodes = doc.DocumentNode.SelectNodes("//tr");

            //Only gets the Table rows that have the actual airac dates, the TryParse checks whether the Column has the cycle number, result
            //can be discarded as we only need to know that it is a number, not yet which number
            return 
                nodes.Where(x => x.ChildNodes.Count == 11)
                .Where(x => int.TryParse(x.ChildNodes[1].InnerText, out _))
                .Select(x => new Airac()
                {
                    CycleNumberInYear = int.Parse(x.ChildNodes[1].InnerText),
                    Ident = int.Parse(x.ChildNodes[3].InnerText),
                    StartDate = ConvertDate(x.ChildNodes[9].InnerText)
                })
                .ToList();
        }

        private static DateOnly ConvertDate (string raw)
        {
            var dateRegex = new Regex("(0[0-9]|1[0-9]|2[0-9]|3[0-1])\\s(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)\\s([2-3][0-9])");
            
            if(!dateRegex.IsMatch(raw))
            {
                throw new Exception();
            }

            var groups = dateRegex.Match(raw).Groups;

            var day = int.Parse(groups[1].Value);

            var month = groups[2].Value switch
            {
                "JAN" => 1,
                "FEB" => 2,
                "MAR" => 3,
                "APR" => 4,
                "MAY" => 5,
                "JUN" => 6,
                "JUL" => 7,
                "AUG" => 8,
                "SEP" => 9,
                "OCT" => 10,
                "NOV" => 11,
                "DEC" => 12,
                _ => throw new ArgumentOutOfRangeException()
            };

            var year = int.Parse($"20{groups[3].Value}");

            return new DateOnly(year, month, day);
        }
    }
}
