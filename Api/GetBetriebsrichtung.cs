using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;

namespace Api
{
    public class GetBetriebsrichtung
    {
        public static void Betriebsrichtung()
        {
            var html = @"https://www.umwelthaus.org/fluglaerm/anwendungen-service/aktuelle-betriebsrichtung-und-prognose/";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(html);

            string Br = null;
            foreach (HtmlNode Node in doc.DocumentNode.SelectNodes("//span[@class=\"br-img\"]"))
            {
                Br = Node.InnerHtml;
            }
        }
    }
}
