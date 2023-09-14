namespace Api.Controllers.Hoppie
{
    internal class MessageType
    {
        internal static RequestType Get(string raw) => raw.ToLowerInvariant() switch
        {
            "progress" => RequestType.Progress,
            "cpdlc" => RequestType.CPDLC,
            "telex" => RequestType.Telex,
            "ping" => RequestType.Ping,
            "inforeq" => RequestType.Inforeq,
            "posreq" => RequestType.Posreq,
            "position" => RequestType.Position,
            "datareq" => RequestType.Datareq,
            "poll" => RequestType.Poll,
            "peek" => RequestType.Peek,
            _ => RequestType.Unknown
        };
    }
}
