using CsvHelper;
using Microsoft.JSInterop.Implementation;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Api.Controllers.Airports
{
    public class AirportsController : Controller
    {
        private static readonly List<char> _regionCodes = new()
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'W', 'Y', 'Z'
        };

        [HttpGet("/airports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetAll() => Json(ReadAirports());

        [HttpGet("/airports/{icao}")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult Get(string icao)
        {
            if(icao.Length != 4) 
            {
                return Json("Please provide a valid four letter ICAO Code");
            }

            return Json(ReadAirports().FirstOrDefault(x => x.Icao == icao.ToUpper()));
        }

        [HttpGet("/airports/icaoairports")]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        [Produces("application/json")]
        public JsonResult GetIcaoAirports(string icao)
        {
            return Json(ReadAirports()
                        .Where(x => _regionCodes.Any(y => x.Icao.StartsWith(y))
                              && x.Icao.Length == 4
                              && x.Icao.All(char.IsLetter)));
                              
        }

        private static List<Airport> ReadAirports()
        {
            var reader = new StreamReader($"{Environment.CurrentDirectory}/airports.csv");
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<AirportDTO>().ToList();

            var airports = records.Select(x => new Airport()
            {
                Icao = x.ident,
                Iata = x.iata_code ?? "",
                Name = x.name,
                Coordinates = new List<decimal>() { decimal.Parse(x.latitude_deg), decimal.Parse(x.longitude_deg) },
                Elevation = x.elevation_ft != "" ? long.Parse(x.elevation_ft) : 0,
                Country = GetCountry(x.iso_country),
                City = x.municipality,
                HasScheduledService = x.scheduled_service == "yes"
            })
            .ToList();

            return airports;
        }

        private static string GetCountry(string isoCode) => isoCode switch
        {
            "AG" => "Solomon Islands",
            "AN" => "Nauru",
            "AY" => "Papua New Guinea",
            "BG" => "Greenland",
            "BI" => "Iceland",
            "BK" => "Kosovo",
            "C" => "Canada",
            "DA" => "Algeria",
            "DB" => "Benin",
            "DF" => "Burkina Faso",
            "DG" => "Ghana",
            "DI" => "Ivory Coast",
            "DN" => "Nigeria",
            "DR" => "Niger",
            "DT" => "Tunisia",
            "DX" => "Togo",
            "EB" => "Belgium",
            "ED" => "Germany",
            "EE" => "Estonia",
            "EF" => "Finland",
            "EG" => "United Kingdom",
            "EH" => "Netherlands"
        };
    }
}
