using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace NutrCalc.Services
{
    public sealed class LocalizationService : ILocalizationService
    {
        private static readonly IReadOnlyList<LanguageOption> AvailableLanguages = new[]
        {
            new LanguageOption(Language.English, "English"),
            new LanguageOption(Language.Russian, "Русский"),
        };

        private static readonly IReadOnlyDictionary<string, string> English = new Dictionary<string, string>
        {
            ["MainWindowTitle"] = "Nutrition Calculator",
            ["ProfilesWindowTitle"] = "Profiles",
            ["CalculateButton"] = "Calculate",
            ["LoadButton"] = "Load",
            ["SaveButton"] = "Save",
            ["SelectButton"] = "Select",
            ["DeleteButton"] = "Delete",
            ["PalLabel"] = "PAL:",
            ["BmrLabel"] = "Basal metabolic rate:",
            ["BmiLabel"] = "Body mass index:",
            ["DailyCalorieIndexGain"] = "Daily calorie index for weight gain:",
            ["DailyCalorieIndexKeep"] = "Daily calorie index to keep weight:",
            ["DailyCalorieIndexLoss"] = "Daily calorie index for weight loss:",
            ["HintWeight"] = "weight",
            ["HintHeight"] = "height",
            ["HintAge"] = "age",
            ["HintDeviation"] = "deviation",
            ["GenderMale"] = "M",
            ["GenderFemale"] = "F",
            ["HintProfileId"] = "Profile",
            ["FieldWeight"] = "weight",
            ["FieldHeight"] = "height",
            ["FieldAge"] = "age",
            ["FieldDeviation"] = "deviation",
            ["FieldPal"] = "PAL",
            ["InvalidInputTitle"] = "Invalid input",
            ["InvalidInputMessage"] = "Please enter numbers for: {0}.{1}The decimal separator can be a comma or a dot, e.g. 70,5.",
            ["ProfileNameRequiredTitle"] = "Profile name required",
            ["ProfileNameRequiredMessage"] = "Enter a profile name in the Profile field before saving.",
            ["SavedTitle"] = "Saved",
            ["SavedMessage"] = "Profile \"{0}\" saved.",
            ["SaveFailedTitle"] = "Save failed",
            ["SaveFailedMessage"] = "Profile \"{0}\" was not saved.",
            ["DatabaseErrorTitle"] = "Nutrition Calculator",
            ["DatabaseErrorMessage"] = "Could not open the profiles database.{0}{1}",
            ["UnitKilogram"] = "kg",
            ["UnitPound"] = "lb",
            ["UnitCentimeter"] = "cm",
            ["UnitMeter"] = "m",
            ["UnitInch"] = "in",
        };

        private static readonly IReadOnlyDictionary<string, string> Russian = new Dictionary<string, string>
        {
            ["MainWindowTitle"] = "Калькулятор питания",
            ["ProfilesWindowTitle"] = "Профили",
            ["CalculateButton"] = "Рассчитать",
            ["LoadButton"] = "Загрузить",
            ["SaveButton"] = "Сохранить",
            ["SelectButton"] = "Выбрать",
            ["DeleteButton"] = "Удалить",
            ["PalLabel"] = "КФА:",
            ["BmrLabel"] = "Базальный метаболизм:",
            ["BmiLabel"] = "Индекс массы тела:",
            ["DailyCalorieIndexGain"] = "Суточный калорийный индекс для набора веса:",
            ["DailyCalorieIndexKeep"] = "Суточный калорийный индекс для поддержания веса:",
            ["DailyCalorieIndexLoss"] = "Суточный калорийный индекс для снижения веса:",
            ["HintWeight"] = "вес",
            ["HintHeight"] = "рост",
            ["HintAge"] = "возраст",
            ["HintDeviation"] = "отклонение",
            ["GenderMale"] = "М",
            ["GenderFemale"] = "Ж",
            ["HintProfileId"] = "Профиль",
            ["FieldWeight"] = "вес",
            ["FieldHeight"] = "рост",
            ["FieldAge"] = "возраст",
            ["FieldDeviation"] = "отклонение",
            ["FieldPal"] = "КФА",
            ["InvalidInputTitle"] = "Неверный ввод",
            ["InvalidInputMessage"] = "Введите числа для полей: {0}.{1}Разделитель дробной части — запятая или точка, например 70,5.",
            ["ProfileNameRequiredTitle"] = "Требуется имя профиля",
            ["ProfileNameRequiredMessage"] = "Введите имя профиля в поле «Профиль», прежде чем сохранять.",
            ["SavedTitle"] = "Сохранено",
            ["SavedMessage"] = "Профиль «{0}» сохранён.",
            ["SaveFailedTitle"] = "Не удалось сохранить",
            ["SaveFailedMessage"] = "Профиль «{0}» не сохранён.",
            ["DatabaseErrorTitle"] = "Калькулятор питания",
            ["DatabaseErrorMessage"] = "Не удалось открыть базу данных профилей.{0}{1}",
            ["UnitKilogram"] = "кг",
            ["UnitPound"] = "фунт",
            ["UnitCentimeter"] = "см",
            ["UnitMeter"] = "м",
            ["UnitInch"] = "дюйм",
        };

        private Language language;

        public LocalizationService(Language initialLanguage = Language.Russian)
        {
            language = initialLanguage;
            Culture = GetCulture(language);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyList<LanguageOption> Languages => AvailableLanguages;

        public Language Language
        {
            get => language;
            set
            {
                if (language == value)
                {
                    return;
                }

                language = value;
                Culture = GetCulture(language);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Language)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Culture)));

                // "Item[]" заставляет WPF перечитать все привязки к индексатору.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            }
        }

        public CultureInfo Culture { get; private set; }

        public string this[string key]
        {
            get
            {
                IReadOnlyDictionary<string, string> dictionary = Language == Language.Russian ? Russian : English;

                return dictionary.TryGetValue(key, out string value)
                    ? value
                    : "!" + key + "!";
            }
        }

        public string Format(string key, params object[] args)
        {
            return string.Format(Culture, this[key], args);
        }

        private static CultureInfo GetCulture(Language language)
        {
            return CultureInfo.GetCultureInfo(language == Language.Russian ? "ru-RU" : "en-US");
        }
    }
}
