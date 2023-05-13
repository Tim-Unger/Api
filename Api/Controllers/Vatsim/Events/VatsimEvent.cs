using System.Text.Json.Serialization;

namespace Api.Controllers.Vatsim.Events
{
    public class VatsimEvent
    {
        public class Event
        {
            public Data[] data { get; set; }
        }

        public class Data
        {
            public int id { get; set; }
            public string type { get; set; }
            public object vso_name { get; set; }
            public string name { get; set; }
            public string link { get; set; }
            public Organiser[] organisers { get; set; }
            public Airport[] airports { get; set; }
            public Route[] routes { get; set; }
            public DateTime start_time { get; set; }
            public DateTime end_time { get; set; }
            public string short_description { get; set; }
            public string description { get; set; }
            public string banner { get; set; }
        }

        public class Organiser
        {
            public string region { get; set; }
            public string division { get; set; }
            public object subdivision { get; set; }
            public bool organised_by_vatsim { get; set; }
        }

        public class Airport
        {
            public string icao { get; set; }
        }

        public class Route
        {
            public string departure { get; set; }
            public string arrival { get; set; }
            public string route { get; set; }
        }

    }
}
