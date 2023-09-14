namespace Api.Controllers.Hoppie
{
    public enum RequestType
    {
        Progress,
        CPDLC,
        Telex,
        Ping,
        Inforeq,
        Posreq,
        Position,
        Datareq,
        Poll,
        Peek,
        Unknown,
    }

    public class HoppieDetails
    {
        public string Callsign { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Network Network { get; set; }
        public int MessageCount { get; set; }
        public List<HoppieMessage> Messages { get; set; } = Enumerable.Empty<HoppieMessage>().ToList();
    }

    public class HoppieMessage
    {
        public int MessageId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Network Network { get; set; }
        public HoppieStation Sender { get; set; }
        public HoppieStation? Receiver { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RequestType Type { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime? RelayTime { get; set; }
        public string Message { get; set; }
    }
}
