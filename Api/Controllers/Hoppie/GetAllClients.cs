using HtmlAgilityPack;

namespace Api.Controllers.Hoppie
{
    public class AllClients
    {
        public static IEnumerable<HoppieStation> Get()
        {
            var html = "http://www.hoppie.nl/acars/system/online.html";

            var web = new HtmlWeb();

            HtmlDocument doc = web.Load(html);

            return doc.DocumentNode
                .SelectNodes("//tr")
                .Where(x => x.ChildNodes.Count == 6)
                .Where(x => int.TryParse(x.ChildNodes[5].InnerText, out _))
                .Select(
                    x =>
                        new HoppieStation()
                        {
                            Network = x.ChildNodes[1].InnerText switch
                            {
                                "None" => Network.None,
                                "IVAO" => Network.IVAO,
                                "VATSIM" => Network.VATSIM,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            Callsign = x.ChildNodes[3].FirstChild.InnerText,
                            MessageCount = int.Parse(x.ChildNodes[5].InnerText),
                            Href = x.ChildNodes[3].FirstChild.Attributes["href"].Value
                        }
                );
        }
    }
}
