namespace Api.Controllers.Airlines
{
    internal class AirlinesJson
    {
        internal static List<Airline> ReadJson()
        {
            var airlinesPath = $"{Environment.CurrentDirectory}/airlines.json";
            return JsonSerializer.Deserialize<List<Airline>>(
                File.ReadAllText(airlinesPath)
            )!;
        }
    }
}
