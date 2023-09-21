using System.Globalization;

namespace Api.Controllers
{
    internal class BetriebsrichtungDecoder
    {
        internal static string Decode(Betriebsrichtung betriebsrichtung)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Betriebsrichtung: {betriebsrichtung.Richtung}").AppendLine();

            var ger = new CultureInfo("de-DE");

            var squashedList = betriebsrichtung.Probabilites.GroupBy(x => x.Richtung);

            foreach (var entry in squashedList)
            {
                stringBuilder
                    .AppendLine(
                        $"Ab {entry.First().ProbabilityStart.ToString("dddd, dd.MM HH:mmZ", ger)}:"
                    )
                    .AppendLine($"{DecodeRichtung(entry.First().Richtung)}")
                    .AppendLine($"Wahrscheinlichkeit: {entry.First().ProbabilityPercent}%");
                    //.AppendLine();

                var probabilities = entry
                    .Skip(1)
                    .Select(x => new { x.ProbabilityStart, x.ProbabilityPercent })
                    .ToList();
                probabilities.ForEach(
                    x =>
                        stringBuilder
                            .AppendLine(
                                $"Ab {x.ProbabilityStart.ToString("dddd, dd.MM HH:mmZ", ger)}: {x.ProbabilityPercent}%"
                            )
                            //.AppendLine($"Wahrscheinlichkeit: {x.ProbabilityPercent}%")
                );

                stringBuilder.AppendLine();
                //entry.Select(x => stringBuilder.AppendLine($"Ab {x.ProbabilityStart.ToString("dddd, dd.MM HH:mmZ", ger)}:").AppendLine($"Wahrscheinlichkeit: {x.ProbabilityPercent}"));
            }

            return stringBuilder.ToString();
        }

        private static string DecodeRichtung(Richtung richtung) =>
            richtung switch
            {
                Richtung.TwoFive => "RWY 25 (Start Richtung Westen).",
                Richtung.ZeroSeven => "RWY 07 (Start Richtung Osten).",
                Richtung.BothPossible => "Beide Richtungen möglich",
                Richtung.ChangingTwoFiveToZeroSeven => "RWY 25, Wechsel zu RWY 07.",
                Richtung.ChangingZeroSevenToTwoFive => "RWY 07, Wechsel zu RWY 25.",
                Richtung.ChangingBetweenBoth => "Wechsel zwischen beiden Richtungen möglich.",
                Richtung.TwoFiveMaybeChanging => "RWY 25, Wechsel zu RWY 07 möglich.",
                Richtung.ZeroSevenMaybeChanging => "RWY 07, Wechsel zu RWY 25 möglich.",
                _ => ""
            };
    }
}
