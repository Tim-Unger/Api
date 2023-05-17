using System.Net;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using static Json.Json;
namespace Api.Controllers.Vatsim
{
    public class GetData
    {
        internal static async Task<string> GetVatsimData() => await new HttpClient().GetStringAsync("https://data.vatsim.net/v3/vatsim-data.json");
    }
}
