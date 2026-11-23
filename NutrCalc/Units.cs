namespace NutrCalc
{
    /// <summary>
    /// Профиль хранится в килограммах и сантиметрах, поэтому ввод в других единицах
    /// приводится к ним перед расчётом и сохранением.
    /// </summary>
    public static class Units
    {
        public const double PoundsPerKilogram = 2.20462;

        public const double CentimetersPerInch = 2.54;

        public static double WeightToKilograms(double weight, string unit)
        {
            return unit == "lb" ? weight / PoundsPerKilogram : weight;
        }

        public static double HeightToCentimeters(double height, string unit)
        {
            switch (unit)
            {
                case "m":
                    return height * 100;
                case "in":
                    return height * CentimetersPerInch;
                default:
                    return height;
            }
        }
    }
}
