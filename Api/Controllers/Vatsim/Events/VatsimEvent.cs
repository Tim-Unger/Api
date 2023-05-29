namespace Api.Controllers.Vatsim.Events
{
    internal class VatsimEvent
    {
        internal class Event
        {
            public Data[] data { get; set; }
        }

        internal class Data
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

        internal class Organiser
        {
            public string region { get; set; }
            public string division { get; set; }
            public object subdivision { get; set; }
            public bool organised_by_vatsim { get; set; }
        }

        internal class Airport
        {
            public string icao { get; set; }
        }

        internal class Route
        {
            public string departure { get; set; }
            public string arrival { get; set; }
            public string route { get; set; }
        }
    }
}