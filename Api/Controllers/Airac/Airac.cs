namespace Api.Controllers.Airac
{
    public class Airac
    {
        public int CycleNumberInYear { get; init; }
        public string Ident { get; init; }
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; set; }
    }
}
