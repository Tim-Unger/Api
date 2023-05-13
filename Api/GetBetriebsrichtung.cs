using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Api
{
    internal class GetBetriebsrichtung
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

            var nodeList = new List<string>();
            var nodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("br-img"));

            nodes.ToList().ForEach(x => nodeList.Add(x.InnerHtml));

            if (nodeList.Count == 1)
            {
                return nodeList[0].Contains("br25west") ? "25" : "07";
            }

            var correctNode = nodeList.Where(x => x.Contains(".svg")).First();

            return correctNode.Contains("br25west") ? "25" : "07";
        }
    }
}
