using System.Net;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static Json.Json;
namespace Api.Controllers.Vatsim
{
    public class GetData
    {
        public static async Task<string> GetVatsimData()
        {
            HttpClient client =new HttpClient();
            return await client.GetStringAsync("https://data.vatsim.net/v3/vatsim-data.json");
        }

        public static Rootobject Deserialize()
        {
            return null;
        }
    }
}
