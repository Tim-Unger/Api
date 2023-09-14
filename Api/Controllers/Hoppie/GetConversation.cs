namespace Api.Controllers.Hoppie
{
    public class Conversation
    {
        internal static string Get(string stationOne, string? stationTwo)
        {
            var hoppieMessages = stationTwo is null ?
                    SingleStation
                    .Get(stationOne)
                    .Messages
                    :
                    SingleStation
                    .Get(stationOne)
                    .Messages
                    .Where(x => x.Receiver is not null)
                    .Where(x => x.Receiver!.Callsign.Equals(stationTwo, StringComparison.InvariantCultureIgnoreCase))
                    .Concat(SingleStation.Get(stationTwo).Messages.Where(x => x.Receiver is not null).Where(x => x.Receiver!.Callsign.Equals(stationOne, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

            hoppieMessages = hoppieMessages.OrderBy(x => x.ReceiveTime).ToList();

            var stringBuilder = new StringBuilder();

            hoppieMessages.ForEach(
                x =>
                    stringBuilder.AppendLine(
                        $"From {x.Sender.Callsign} to {x.Receiver?.Callsign ?? "Unknown Station"} at {x.ReceiveTime.ToString("HH:mm:ss")}Z:\r\n{x.Message}\r\n"
                    )
            );

            return stringBuilder.ToString();
        }
    }
}
