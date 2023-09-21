
namespace Api.Controllers.Airlines
{
    internal class AirlineResult
    {
        public IEnumerable<SearchParameter> Parameters { get; set; }
        public List<Airline> Airlines { get; set; }
    }
}
