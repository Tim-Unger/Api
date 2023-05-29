using static Api.Controllers.DAtis.DAtisAirportsClass;

namespace Api.Controllers.DAtis
{
	[ApiController]
	[Route("api")]
	public class DatisController : Controller
	{
		/// <summary>
		/// Get all supported D-ATIS Airports
		/// </summary>
		/// <returns></returns>
		[HttpGet("/datis/airports")]
		[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
		public string GetAirports() => string.Join(Environment.NewLine, DAtisAirports);

		/// <summary>
		/// Get the D-Atis for an American Airport
		/// </summary>
		/// <remarks>
		/// Please use /datis/airports to see a list of supported airports.
		/// Append /textonly to only get the Text of the D-ATIS
		/// </remarks>
		/// <param name="icao">the four or three letter ICAO of the Airport</param>
		/// <param name="textOnly">whether you would like the D-ATIS text only</param>
		/// <returns></returns>
		[HttpGet("/datis/{icao}/{textOnly?}")]
		[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
		public async Task<JsonResult> Get(string icao, string? textOnly = null)
		{
			var client = new HttpClient();
            var stringBuilder = new StringBuilder();

			var returnTextOnly = textOnly?.ToLowerInvariant() == "textonly";

			if (string.IsNullOrEmpty(icao) || icao.Length < 3 || icao.Length > 4)
			{
				return Json("Please provide a valid ICAO (JKF or KJFK)");
			}

			var countryCodes = new[] { 'K', 'P', 'T' };
			if (icao.Length == 4 && !countryCodes.Any(x => icao.ToUpper().StartsWith(x)))
			{
				return Json("Please provide a valid American ICAO-Code (KXXX, PANC, PHNL, or TJSJ)");
			}

			if (icao.Length == 3)
			{
				var icaoCode = icao.ToUpper() switch
				{
					"ANC" or "HNL" => $"P{icao.ToUpper()}",
					"JSJ" => $"T{icao.ToUpper()}",
					_ => $"K{icao.ToUpper()}",
				};

				if (!DAtisAirports.Any(x => x == icaoCode))
				{
					return Json("Your ICAO does not have a D-ATIS");
				}

				var concatAtis = await client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icaoCode}");

				if (concatAtis == null)
				{
					return Json("Error fetching D-ATIS");
				}

				if (returnTextOnly)
				{
					concatAtis.ForEach(x => stringBuilder.AppendLine(x.Datis));

					return Json(stringBuilder.ToString());
				}

				return Json(concatAtis);
			}

			if (!DAtisAirports.Any(x => x == icao.ToUpper()))
			{
				return Json("Your ICAO does not have a D-ATIS");
			}

			var atis = await client.GetFromJsonAsync<List<DAtis>>($"https://datis.clowd.io/api/{icao}");

			if (atis == null || atis.Count == 0)
			{
				return Json("No D-ATIS found for your airpot");
			}

			if (returnTextOnly)
			{
				atis.ForEach(x => stringBuilder.AppendLine(x.Datis));

				return Json(stringBuilder.ToString());
			}

			return Json(atis);
		}
	}
}