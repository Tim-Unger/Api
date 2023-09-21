using CsvHelper;
using System.Globalization;

namespace Api.Controllers.Airports
{
    internal class Airports
    {
        internal static List<Airport> Read()
        {
            var reader = new StreamReader($"{Environment.CurrentDirectory}/Data/airports.csv");
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

        //TODO
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
            "EH" => "Netherlands",
            _ => ""
            //_ => throw new NotImplementedException()
        };
    }
}
