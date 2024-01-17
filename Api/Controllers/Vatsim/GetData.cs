namespace Api.Controllers.Vatsim
{
    public class GetData
    {
        private static readonly HttpClient _client = new();
        internal static async Task<string> GetVatsimData() => await _client.GetStringAsync("https://data.vatsim.net/v3/vatsim-data.json");
    }
}
