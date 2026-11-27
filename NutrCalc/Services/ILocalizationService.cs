using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NutrCalc.Services
{
    public interface ILocalizationService : INotifyPropertyChanged
    {
        Language Language { get; set; }

        CultureInfo Culture { get; }

        IReadOnlyList<LanguageOption> Languages { get; }

        string this[string key] { get; }

        string Format(string key, params object[] args);
    }
}
