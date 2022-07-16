using System.Net;
using System.Text;

namespace Api.Controllers.Vatsim
{
    public class GetData
    {
        public static string GetVatsimData()
        {
            WebRequest Request = WebRequest.Create(("https://data.vatsim.net/v3/vatsim-data.json"));
            Request.Proxy = null;
            WebResponse Response = Request.GetResponse();
            using (Stream dataStream = Response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                return responseFromServer;
            }
            //System.Net.WebClient wc = new System.Net.WebClient();
            //byte[] raw = null;
            //raw = wc.DownloadData("https://data.vatsim.net/v3/vatsim-data.json");
            //string webData = Encoding.UTF8.GetString(raw);
            //string Data = webData;
        }
    }
}
