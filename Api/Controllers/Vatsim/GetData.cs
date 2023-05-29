namespace Api.Controllers.Vatsim
{
    public class GetData
    {
        internal static async Task<string> GetVatsimData() => await new HttpClient().GetStringAsync("https://data.vatsim.net/v3/vatsim-data.json");
    }
}
