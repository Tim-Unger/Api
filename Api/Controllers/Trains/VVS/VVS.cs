using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Converters;

namespace Api.Controllers.Trains.VVS
{
    public enum VVSTrainType
    {
        SBahn,
        UBahn,
        Bus,
        Regionalbahn
    }

    public enum Status
    {
        Running,
        Unknown,
        Cancelled,
    }

    public class VVSTimetable
    {
        public DateTime RequestTime { get; set; }
        public string Station { get; set; }
        public List<Departure>? Departures { get; set; }
    }

    public class Departure
    {
        public int CountdownMinutes { get; set; }
        public TimeSpan TimeUntilDeparture { get; set; }
        public DateTime ScheduledDeparture { get; set; }
        public DateTime ActualDeparture { get; set; }
        public int Delay { get; set; }
        public string Platform { get; set; }
        public string Line { get; set; }
        public string Destination { get; set; }
        public string DepartureStation { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public VVSTrainType TrainType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
    }
}

public class DateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _format;

    public DateTimeConverter(string format)
    => _format = format;

    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
    => writer.WriteStringValue(date.ToString(_format));

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    => DateTime.ParseExact(reader.GetString(), _format, null);
}
