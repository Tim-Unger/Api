namespace Api.Controllers.Hoppie
{
    public enum Network
    {
        None,
        VATSIM,
        IVAO
    }

    public class HoppieStation
    {
        public string Callsign { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Network Network { get; set; }

        public int MessageCount { get; set; }

        internal string Href { get; set; }
    }
}
