namespace Api.Controllers.Hoppie
{
    public class SingleStation
    {
        public static HoppieDetails Get(string station)
        {
            var allStations = AllClients.Get();
            if (
                !allStations.Any(
                    x => x.Callsign.Equals(station, StringComparison.InvariantCultureIgnoreCase)
                )
            )
            {
                throw new KeyNotFoundException("Station not found");
            }

            var hoppieStation = allStations.First(
                x => x.Callsign.Equals(station, StringComparison.InvariantCultureIgnoreCase)
            );

            var html = $"http://www.hoppie.nl/acars/system/{hoppieStation.Href}";
            var web = new HtmlWeb();

            HtmlDocument doc = web.Load(html);

            var details = new HoppieDetails()
            {
                Callsign = hoppieStation.Callsign,
                Network = hoppieStation.Network,
                MessageCount = hoppieStation.MessageCount
            };

            if (hoppieStation.MessageCount == 0)
            {
                return details;
            }

            var messagesRaw = doc.DocumentNode
                .SelectNodes("//tr")
                .Where(x => x.ChildNodes.Count == 19)
                .Select(x => x.ChildNodes)
                .Skip(1); //Skips the Table Header

            foreach (var message in messagesRaw)
            {
                var hoppieMessage = new HoppieMessage
                {
                    MessageId = int.Parse(message[1].InnerText),
                    Network = hoppieStation.Network,
                    Sender =
                        allStations.FirstOrDefault(
                            x => x.Callsign == message[5].FirstChild.InnerText
                        )
                        ?? new HoppieStation() //Station is not connected anymore
                        {
                            Callsign = message[5].FirstChild.InnerText,
                            Network = Network.None,
                            MessageCount = 0
                        },
                    //TODO
                    Receiver = allStations.FirstOrDefault(
                        x => x.Callsign == message[7].FirstChild.InnerText
                    ),
                    Type = MessageType.Get(message[9].InnerText)
                };

                var timeRegex = new Regex(@"(\d{2})-(\d{2}):(\d{2})Z");
                var now = DateTime.UtcNow;

                var receiveTimeRaw = message[11].InnerText;

                var receiveMatch = timeRegex.Match(receiveTimeRaw).Groups;

                var receiveDay = int.Parse(receiveMatch[1].Value);
                var receiveHour = int.Parse(receiveMatch[2].Value);
                var receiveMinute = int.Parse(receiveMatch[3].Value);

                hoppieMessage.ReceiveTime = new DateTime(
                    now.Year,
                    now.Month,
                    receiveDay,
                    receiveHour,
                    receiveMinute,
                    0
                );

                var isRelayed = message[13].InnerText != "";

                if (isRelayed)
                {
                    var relayTimeRaw = message[13].InnerText;

                    var relayMatch = timeRegex.Match(relayTimeRaw);

                    var relayDay = int.Parse(receiveMatch[1].Value);
                    var relayHour = int.Parse(receiveMatch[2].Value);
                    var relayMinute = int.Parse(receiveMatch[3].Value);

                    hoppieMessage.RelayTime = new DateTime(
                        now.Year,
                        now.Month,
                        relayDay,
                        relayHour,
                        relayMinute,
                        0
                    );
                }

                hoppieMessage.Message = message[17].InnerText;

                details.Messages.Add(hoppieMessage);
            }

            return details;
        }
    }
}
