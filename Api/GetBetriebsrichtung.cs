using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Api
{
    public class GetBetriebsrichtung
    {
        public static string Betriebsrichtung()
        {
            var html = @"https://www.umwelthaus.org/fluglaerm/anwendungen-service/aktuelle-betriebsrichtung-und-prognose/";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(html);

            string BetriebsrichtungString = null;
            List<string> NodeList = new List<string>();
            var nodes = doc.DocumentNode.Descendants().Where(n => n.HasClass("br-img"));

            foreach (var node in nodes)
            {
                NodeList.Add(node.InnerHtml);
            }

            string CorrectNode = null;

            if (NodeList.Count == 1)
            {
                if (NodeList[0].Contains("br25west"))
                {
                    BetriebsrichtungString = "25";
                }

                else
                {
                    BetriebsrichtungString = "07";
                }
            }

            else
            {
                //TOD fix
                foreach (var ListItem in NodeList)
                {
                    if (ListItem.Contains(".svg"))
                    {
                        CorrectNode = ListItem;
                    }
                }

                if (CorrectNode.Contains("br25west"))
                {
                    BetriebsrichtungString = "25";
                }

                else
                {
                    BetriebsrichtungString = "07";
                }
            }

            return BetriebsrichtungString;
        }
    }
}
