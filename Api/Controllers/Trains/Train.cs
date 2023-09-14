using Newtonsoft.Json.Converters;

namespace Api.Controllers
{
    public enum TrainType
    {
        ICE,
        EC,
        IC,
        IRE,
        Other
    }

    public class Train
    {
        public string FullTrainName { get; set; }

        public int TrainNumber { get; set; }

        //TODO
        //[JsonConverter(typeof(StringEnumConverter))]
        public TrainType TrainType { get; set; }

        public string? RollingStock { get; set; }

        public string Departure { get; set; }

        public string Destination { get; set; }

        //TODO
        //public List<string>? Stops { get; set; }

        public List<string> OperatingDays { get; set; }

        public int TopSpeedKpH { get; set; }

        public List<TrainCar> Cars { get; set; }
    }

    public class TrainCar
    {
        public int Number { get; set; }

        public string Type { get; set; }
    }
}