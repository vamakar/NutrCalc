using System.Collections.Generic;

namespace NutrCalc.Services
{
    public static class Units
    {
        public const string Kilograms = "kg";
        public const string Pounds = "lb";
        public const string Centimeters = "cm";
        public const string Meters = "m";
        public const string Inches = "in";

        public const double PoundsPerKilogram = 2.20462;

        public const double CentimetersPerInch = 2.54;

        public static IReadOnlyList<string> WeightUnits { get; } = new[] { Kilograms, Pounds };

        public static IReadOnlyList<string> HeightUnits { get; } = new[] { Centimeters, Meters, Inches };

        public static double WeightToKilograms(double weight, string unit)
        {
            return unit == Pounds ? weight / PoundsPerKilogram : weight;
        }

        public static double HeightToCentimeters(double height, string unit)
        {
            switch (unit)
            {
                case Meters:
                    return height * 100;
                case Inches:
                    return height * CentimetersPerInch;
                default:
                    return height;
            }
        }
    }
}
