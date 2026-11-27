using System;
using NutrCalc.Models;

namespace NutrCalc.Services
{
    public static class Calculator
    {
        public static double ComputeBasalMetabolicRate(double weightKg, double heightCm, short age, Gender gender)
        {
            double heightInMeters = heightCm / 100;

            return weightKg * 9.99 + heightInMeters * 625 - age * 4.92 + GetGenderCoefficient(gender);
        }

        public static double ComputeBodyMassIndex(double weightKg, double heightCm)
        {
            double heightInMeters = heightCm / 100;

            return weightKg / heightInMeters / heightInMeters;
        }

        public static double ComputeDailyCalorieIndex(double basalMetabolicRate, double pal, Sign sign, double deviationPercent)
        {
            return basalMetabolicRate * pal * (1 + GetSignCoefficient(sign) * deviationPercent / 100);
        }

        private static double GetGenderCoefficient(Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return 5;
                case Gender.Female:
                    return -161;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gender));
            }
        }

        private static double GetSignCoefficient(Sign sign)
        {
            switch (sign)
            {
                case Sign.Gain:
                    return 1;
                case Sign.Keep:
                    return 0;
                case Sign.Loss:
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sign));
            }
        }
    }
}
