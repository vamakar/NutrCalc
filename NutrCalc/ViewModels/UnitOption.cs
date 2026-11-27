using System.Collections.Generic;
using System.ComponentModel;
using NutrCalc.Infrastructure;
using NutrCalc.Services;

namespace NutrCalc.ViewModels
{
    public sealed class UnitOption : ViewModelBase
    {
        private readonly ILocalizationService localization;
        private readonly string displayKey;

        public UnitOption(string code, string displayKey, ILocalizationService localization)
        {
            Code = code;
            this.displayKey = displayKey;
            this.localization = localization;

            localization.PropertyChanged += OnLocalizationChanged;
        }

        public string Code { get; }

        public string Display => localization[displayKey];

        public static IReadOnlyList<UnitOption> CreateWeightOptions(ILocalizationService localization)
        {
            return new[]
            {
                new UnitOption(Units.Kilograms, "UnitKilogram", localization),
                new UnitOption(Units.Pounds, "UnitPound", localization),
            };
        }

        public static IReadOnlyList<UnitOption> CreateHeightOptions(ILocalizationService localization)
        {
            return new[]
            {
                new UnitOption(Units.Centimeters, "UnitCentimeter", localization),
                new UnitOption(Units.Meters, "UnitMeter", localization),
                new UnitOption(Units.Inches, "UnitInch", localization),
            };
        }

        public override string ToString()
        {
            return Display;
        }

        private void OnLocalizationChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Display));
        }
    }
}
