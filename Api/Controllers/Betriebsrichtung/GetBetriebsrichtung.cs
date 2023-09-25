namespace Api
{
    internal class GetBetriebsrichtung
    {
        /// <summary>
        /// Gets the current Runway direction in EDDF
        /// </summary>
        /// <returns></returns>
        internal static Betriebsrichtung Get()
        {
            var betriebsrichtung = new Betriebsrichtung();

            var html = @"https://www.umwelthaus.org/fluglaerm/anwendungen-service/aktuelle-betriebsrichtung-und-prognose/";
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(html);

            var nodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("br-img"));

            var nodeList = nodes.Select(x => x.InnerHtml).ToList();

            if (nodeList.Count == 1)
            {
                betriebsrichtung.Richtung = nodeList[0].Contains("br25west") ? "25" : "07";
            }

            var correctNode = nodeList.Where(x => x.Contains(".svg")).First();

            betriebsrichtung.Richtung = correctNode.Contains("br25west") ? "25" : "07";

            var probabilitiesRawHtml = doc.DocumentNode
                .SelectNodes("//script")
                .First(x => x.Attributes["data-xml-id"] is not null)
                .InnerText;

            var onlyData = new Regex(@"<period[\s\S]*(?=(?><info>))", RegexOptions.Multiline).Match(probabilitiesRawHtml).Value;

            //"" is how C# displays \" in html, we need to replace \" since C# Regex is doing weird shit otherwise (Cost me 2 hours of my life to figure out why my perfectly fine Regex wasn't capturing)
            onlyData = onlyData.Replace(@"""", " ");

            var probabilityRegex = new Regex(@"<period\sfrom=\s(?>Mon|Tue|Wed|Thu|Fri|Sat|Sun)\s(((Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s(\d{1,2})\s(?>(\d{2}):\d{2}:\d{2})\s)\+(\d{2})(?>:\d{2})\s(20\d{2})\s\s)(?>state=\s)(\d{1,2})\s\sprobability=\s\.(\d{1,2})", RegexOptions.Multiline);

            var probabilityMatches = probabilityRegex.Matches(onlyData);

            foreach (Match match in probabilityMatches.Cast<Match>())
            {
                var probability = new Probability();

                var groups = match.Groups;


                probability.ProbabilityPercent = int.Parse(groups[9].Value);

                var day = int.Parse(groups[4].Value);
                
                var utcOffset = int.Parse(groups[6].Value);
                var hour = int.Parse(groups[5].Value) - utcOffset;

                var month = GetMonth(groups[3].Value);

                var now = DateTime.UtcNow;

                probability.ProbabilityStart = new DateTime(now.Year, month, day, hour, 00, 00);

                probability.ProbabilityStartDate = DateOnly.FromDateTime(probability.ProbabilityStart);
                probability.ProbabiltyStartTime = TimeOnly.FromDateTime(probability.ProbabilityStart);

                //1 = 25
                //21 = Chnging 07 to 25
                //13 = Changing between both
                //32 = 25 > maybe changing
                var state = int.Parse(groups[8].Value);
                probability.Richtung = state switch
                {
                    1 => Richtung.TwoFive,
                    2 => Richtung.BothPossible,
                    3 => Richtung.ZeroSeven,
                    12 => Richtung.TwoFiveMaybeChanging,
                    13 => Richtung.ChangingBetweenBoth,
                    21 => Richtung.ChangingZeroSevenToTwoFive,
                    23 => Richtung.ChangingTwoFiveToZeroSeven,
                    32 => Richtung.ZeroSevenMaybeChanging,
                    _ => throw new NotImplementedException()
                };

                //probability.Richtung = (Richtung)state;

                betriebsrichtung.Probabilites.Add(probability);
            }

            return betriebsrichtung;
        }

        private static int GetMonth(string month) => month.ToUpperInvariant() switch
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
    }
}
