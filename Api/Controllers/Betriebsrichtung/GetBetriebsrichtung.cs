using HtmlAgilityPack;

namespace Api
{
    public class GetBetriebsrichtung
    {
        /// <summary>
        /// Gets the current Runway direction in EDDF
        /// </summary>
        /// <returns></returns>
        internal static string Betriebsrichtung()
        {
            var html = @"https://www.umwelthaus.org/fluglaerm/anwendungen-service/aktuelle-betriebsrichtung-und-prognose/";
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(html);

            var nodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("br-img"));

            var nodeList = nodes.Select(x => x.InnerHtml).ToList();

            if (nodeList.Count == 1)
            {
                return nodeList[0].Contains("br25west") ? "25" : "07";
            }

            var correctNode = nodeList.Where(x => x.Contains(".svg")).First();

            return correctNode.Contains("br25west") ? "25" : "07";
        }
    }
}
