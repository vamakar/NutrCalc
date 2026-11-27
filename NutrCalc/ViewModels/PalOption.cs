using System.Collections.Generic;
using System.Globalization;

namespace NutrCalc.ViewModels
{
    public sealed class PalOption
    {
        private static readonly double[] DefaultValues =
        {
            1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0,
        };

        public PalOption(double value)
        {
            Value = value;
            Display = value.ToString("0.0", CultureInfo.InvariantCulture);
        }

        public double Value { get; }

        public string Display { get; }

        public static IReadOnlyList<PalOption> CreateDefaults()
        {
            var options = new List<PalOption>();

            foreach (double value in DefaultValues)
            {
                options.Add(new PalOption(value));
            }

            return options;
        }

        public override string ToString()
        {
            return Display;
        }
    }
}
