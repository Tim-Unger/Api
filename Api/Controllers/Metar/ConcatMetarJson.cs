using MetarSharp;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Api.Controllers.Metar
{
    public class ConcatMetarJson
    {
        internal static string GetJson(MetarSharp.Metar metar) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.MetarRaw));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Airport));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.ReportingTime));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.IsAutomatedReport));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Wind));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Visibility));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.RunwayVisibilities));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Weather));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Clouds));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Temperature));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Pressure));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.Trends));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.RunwayConditions));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.ReadableReport));
            stringBuilder.AppendLine(JsonSerializer.Serialize(metar.AdditionalInformation));

            return stringBuilder.ToString();
        }
    }
}
