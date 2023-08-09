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

        public TrainType TrainType { get; set; }

        public string RollingStock { get; set; }

        public string Departure { get; set; }

        public string Destination { get; set; }

        public List<string> Stops { get; set; }

        public List<string> OperatingDays { get; set; }

        public int TopSpeed { get; set; }
    }
}
