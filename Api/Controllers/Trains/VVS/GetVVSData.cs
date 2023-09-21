using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;
using System.Xml;

namespace Api.Controllers.Trains.VVS
{
    public class VVSData
    {
        public static VVSTimetable Get(string station)
        {
            var client = new HttpClient();

            var data = client
                .GetStringAsync(
                    $"https://efastatic.vvs.de/OpenVVSDay/XML_DM_REQUEST?laguage=de&typeInfo_dm=stopID&nameInfo_dm={station}&deleteAssignedStops_dm=1&useRealtime=1&mode=direct"
                )
                .Result;

            client.Dispose();

            var timetable = new VVSTimetable();

            var xml = new XmlDocument();

            xml.LoadXml(data);

            var requestTime =
                xml.SelectSingleNode("/itdRequest")?.Attributes?["now"]?.Value
                ?? throw new Exception("Request Time not found");

            timetable.RequestTime = DateTime.Parse(requestTime);

            var stationName = xml.SelectSingleNode("/itdRequest/itdDepartureMonitorRequest/itdOdv/itdOdvName/odvNameElem").Attributes["objectName"].Value;
            timetable.Station = stationName;

            var departures =
                xml.SelectSingleNode("/itdRequest/itdDepartureMonitorRequest/itdDepartureList")
                    .ChildNodes.Cast<XmlNode>()
                //.Where(x => x.Attributes?["pointType"]?.InnerText == "Gleis")
                ?? throw  new Exception("Departures not found");

            var departureTime = departures.First().SelectSingleNode("//itdDateTime");

            var attributes = departures.Select(x => x.Attributes).ToList().First();

            var departuresConverted = new List<Departure>();

            foreach (var departure in departures)
            {
                var dep = new Departure();

                var itdServingLine = departure.SelectSingleNode("itdServingLine") ?? throw new Exception();
                var itdNoTrain = itdServingLine.SelectSingleNode("itdNoTrain") ?? throw new Exception();

                dep.CountdownMinutes = int.Parse(
                        departure.Attributes?["countdown"]?.InnerText ?? throw new Exception()
                    );

                dep.TimeUntilDeparture = TimeSpan.FromMinutes(dep.CountdownMinutes);

                var delay = int.Parse(
                        itdNoTrain.Attributes?["delay"]?.InnerText
                            ?? "0"
                    );

                dep.Delay = delay;

                var scheduledDepAttributes = departure.SelectSingleNode("itdDateTime/itdTime") ?? throw new Exception();
                var actualDepAttributes = delay == 0 ? scheduledDepAttributes : departure.SelectSingleNode("itdRTDateTime/itdTime") ?? throw new Exception();

                (var scheduledHour, var scheduledMinute) =
                    (
                    int.Parse(scheduledDepAttributes?.Attributes?["hour"]?.InnerText ?? throw new Exception()),
                    int.Parse(scheduledDepAttributes?.Attributes?["minute"]?.InnerText ?? throw new Exception())
                    );

                (var actualHour,
                var actualMinute) =
                    (
                    int.Parse(actualDepAttributes.Attributes?["hour"]?.InnerText ?? throw new Exception()),
                    int.Parse(actualDepAttributes.Attributes?["minute"]?.InnerText ?? throw new Exception())
                    );

                var now = DateTime.Now;
                dep.ScheduledDeparture = new DateTime(now.Year, now.Month, now.Day, scheduledHour, scheduledMinute, 0);

                dep.ActualDeparture = new DateTime(now.Year, now.Month, now.Day, actualHour, actualMinute, 0);


                dep.Platform = departure.Attributes["platformName"].InnerText;
                dep.Line = itdServingLine.Attributes["number"].InnerText;
                dep.Destination = itdServingLine.Attributes["direction"].InnerText;
                dep.DepartureStation = itdServingLine.Attributes["directionFrom"].InnerText;
                dep.TrainType = itdNoTrain.Attributes["name"].InnerText switch
                {
                    "S-Bahn" => VVSTrainType.SBahn,
                    "U-Bahn" or "Stadtbahn" => VVSTrainType.UBahn,
                    "Bus" => VVSTrainType.Bus,
                    "R-Bahn" => VVSTrainType.Regionalbahn,
                    _ => VVSTrainType.Bus
                };

                var statusAttributes = new Regex(@"([A-Z]{1,})(?>(?=\||$))")
                    .Matches(departure.Attributes?["realtimeTripStatus"]?.InnerText ?? "")
                    .Select(x => x.Value) ?? null;

                //TODO
                dep.Status =
                        statusAttributes
                        == null
                        ? Status.Unknown
                        : statusAttributes switch
                        {
                            IEnumerable<string> when statusAttributes.Any(x => x == "MONITORED") => Status.Running,
                            IEnumerable<string> when statusAttributes.Any(x => x == "TRIP_CANCELLED") => Status.Cancelled,
                            _ => Status.Unknown

                        };

                departuresConverted.Add(dep);
            }

            timetable.Departures = departuresConverted.OrderBy(x => x.CountdownMinutes).ToList();

            return timetable;
        }
    }
}
