namespace Api.Controllers
{
    internal class BetriebsrichtungDrawer
    {
        internal static string Draw(Betriebsrichtung betriebsrichtung)
        {
            var stringBuilder = new StringBuilder();

            //First Line
            stringBuilder.AppendLine(string.Join("", Enumerable.Repeat("-", betriebsrichtung.Probabilites.Count * 10 + 2)));
            //Second Line
            stringBuilder.AppendLine(string.Join("", Enumerable.Repeat(" ", betriebsrichtung.Probabilites.Count * 10 + 2)));

            stringBuilder.AppendLine(string.Join("", Enumerable.Repeat(" ", betriebsrichtung.Probabilites.Count * 10 + 2)));

            //Third Row
            stringBuilder.Append("07").Append(FourthRow(betriebsrichtung.Probabilites));

            stringBuilder.AppendLine().AppendLine(FithRow(betriebsrichtung.Probabilites));

            stringBuilder.AppendLine().AppendLine(SixthRow(betriebsrichtung.Probabilites));

            stringBuilder.Append("--").Append(SeventhRow(betriebsrichtung.Probabilites));

            //foreach(var entry in betriebsrichtung.Probabilites)
            //{
            //    var bar = entry.Richtung switch
            //    {
            //        Richtung.TwoFive => "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n==========",
            //        Richtung.ZeroSeven => "\r\n\r\n==========",
            //        Richtung.ChangingZeroSevenToTwoFive => "\r\n\r\n=\r\n =\r\n  =\r\n   =\r\n    =\r\n     =\r\n      =",
            //        Richtung.ChangingBetweenBoth => "\r\n\r\n=        =\r\n =      =\r\n  =    =\r\n   =  =\r\n    ==",
            //        Richtung.TwoFiveMaybeChanging => "\r\n\r\n\r\n\r\n         =\r\n        =  \r\n       =\r\n      =\r\n     =\r\n====="
            //    };
            //    stringBuilder.Append(bar);
            //}

            return stringBuilder.ToString();
        }

        private static string FourthRow(IEnumerable<Probability> probabilities)
        {
            var stringBuilder = new StringBuilder();

            foreach(var probability in probabilities)
            {
                stringBuilder.Append(probability.Richtung switch
                {
                    Richtung.TwoFive => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ZeroSeven => string.Join("", Enumerable.Repeat("=", 10)),
                    Richtung.ChangingZeroSevenToTwoFive => $@"⩶{string.Join("", Enumerable.Repeat(" ", 7))}",
                    Richtung.ChangingBetweenBoth => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.TwoFiveMaybeChanging => string.Join("", Enumerable.Repeat(" ", 10)),
                });
            }

            return stringBuilder.ToString();
        }

        private static string FithRow(IEnumerable<Probability> probabilities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var probability in probabilities)
            {
                stringBuilder.Append(probability.Richtung switch
                {
                    Richtung.TwoFive => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ZeroSeven => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ChangingZeroSevenToTwoFive => "    =     ",
                    Richtung.ChangingBetweenBoth => "=        =",
                    Richtung.TwoFiveMaybeChanging => string.Join("", Enumerable.Repeat(" ", 10)),
                });
            }

            return stringBuilder.ToString();
        }

        private static string SixthRow(IEnumerable<Probability> probabilities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var probability in probabilities)
            {
                stringBuilder.Append(probability.Richtung switch
                {
                    Richtung.TwoFive => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ZeroSeven => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ChangingZeroSevenToTwoFive => "     =    ",
                    Richtung.ChangingBetweenBoth => " =      = ",
                    Richtung.TwoFiveMaybeChanging => string.Join("", Enumerable.Repeat(" ", 10)),
                });
            }

            return stringBuilder.ToString();
        }

        private static string SeventhRow(IEnumerable<Probability> probabilities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var probability in probabilities)
            {
                stringBuilder.Append(probability.Richtung switch
                {
                    Richtung.TwoFive => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ZeroSeven => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ChangingZeroSevenToTwoFive => "      =   ",
                    Richtung.ChangingBetweenBoth => "   =     =  ",
                    Richtung.TwoFiveMaybeChanging => $@"{string.Join("", Enumerable.Repeat(" ", 8))}⩶",
                });
            }

            return stringBuilder.ToString();
        }

        private static string EightRow(IEnumerable<Probability> probabilities)
        {
            var stringBuilder = new StringBuilder();

            foreach (var probability in probabilities)
            {
                stringBuilder.Append(probability.Richtung switch
                {
                    Richtung.TwoFive => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ZeroSeven => string.Join("", Enumerable.Repeat(" ", 10)),
                    Richtung.ChangingZeroSevenToTwoFive => "       =  ",
                    Richtung.ChangingBetweenBoth => "    =    =  ",
                    Richtung.TwoFiveMaybeChanging => $@"{string.Join("", Enumerable.Repeat(" ", 6))}",
                });
            }

            return stringBuilder.ToString();
        }
    }
}
