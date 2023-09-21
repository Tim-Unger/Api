namespace Api.Controllers.Airlines
{
    internal class AirlinesJson
    {
        internal static List<Airline> ReadJson()
        {
            var airlinesPath = $"{Environment.CurrentDirectory}/Data/airlines.json";
            return JsonSerializer.Deserialize<List<Airline>>(
                File.ReadAllText(airlinesPath)
            )!;
        }
    }
}
