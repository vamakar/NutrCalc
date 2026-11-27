using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using NutrCalc.Infrastructure;
using NutrCalc.Models;
using NutrCalc.Services;

namespace NutrCalc.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private static readonly Dictionary<string, string> FieldKeys = new Dictionary<string, string>
        {
            [nameof(WeightText)] = "FieldWeight",
            [nameof(HeightText)] = "FieldHeight",
            [nameof(AgeText)] = "FieldAge",
            [nameof(DeviationText)] = "FieldDeviation",
            [nameof(SelectedPal)] = "FieldPal",
        };

        private readonly IProfileStore store;
        private readonly IDialogService dialogs;
        private readonly ILocalizationService localization;
        private readonly Dictionary<string, string> errors = new Dictionary<string, string>();

        private string profileId = string.Empty;
        private string weightText = string.Empty;
        private string heightText = string.Empty;
        private string ageText = string.Empty;
        private string deviationText = string.Empty;
        private UnitOption selectedWeightUnit;
        private UnitOption selectedHeightUnit;
        private PalOption selectedPal;
        private LanguageOption selectedLanguage;
        private Gender selectedGender = Gender.Male;
        private Sign selectedSign = Sign.Keep;
        private string bmrText = string.Empty;
        private string bmiText = string.Empty;
        private string dailyCalorieIndexText = string.Empty;
        private Profile lastResultProfile;

        public MainWindowViewModel(IProfileStore store, IDialogService dialogs, ILocalizationService localization)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            this.localization = localization ?? throw new ArgumentNullException(nameof(localization));

            WeightUnits = UnitOption.CreateWeightOptions(localization);
            HeightUnits = UnitOption.CreateHeightOptions(localization);
            PalOptions = PalOption.CreateDefaults();

            selectedWeightUnit = WeightUnits[0];
            selectedHeightUnit = HeightUnits[0];
            selectedPal = PalOptions[0];
            selectedLanguage = localization.Languages.First(option => option.Language == localization.Language);

            localization.PropertyChanged += OnLocalizationChanged;

            CalculateCommand = new RelayCommand(Calculate);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand(Load);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ILocalizationService Localization => localization;

        public IReadOnlyList<LanguageOption> Languages => localization.Languages;

        public IReadOnlyList<UnitOption> WeightUnits { get; }

        public IReadOnlyList<UnitOption> HeightUnits { get; }

        public IReadOnlyList<PalOption> PalOptions { get; }

        public ICommand CalculateCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand LoadCommand { get; }

        public LanguageOption SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (Set(ref selectedLanguage, value) && value != null)
                {
                    localization.Language = value.Language;
                }
            }
        }

        public string ProfileId
        {
            get => profileId;
            set => Set(ref profileId, value);
        }

        public string WeightText
        {
            get => weightText;
            set => Set(ref weightText, value);
        }

        public string HeightText
        {
            get => heightText;
            set => Set(ref heightText, value);
        }

        public string AgeText
        {
            get => ageText;
            set => Set(ref ageText, value);
        }

        public string DeviationText
        {
            get => deviationText;
            set => Set(ref deviationText, value);
        }

        public UnitOption SelectedWeightUnit
        {
            get => selectedWeightUnit;
            set => Set(ref selectedWeightUnit, value);
        }

        public UnitOption SelectedHeightUnit
        {
            get => selectedHeightUnit;
            set => Set(ref selectedHeightUnit, value);
        }

        public PalOption SelectedPal
        {
            get => selectedPal;
            set => Set(ref selectedPal, value);
        }

        public Gender SelectedGender
        {
            get => selectedGender;
            set => Set(ref selectedGender, value);
        }

        public Sign SelectedSign
        {
            get => selectedSign;
            set
            {
                if (Set(ref selectedSign, value))
                {
                    OnPropertyChanged(nameof(DailyCalorieIndexLabel));
                }
            }
        }

        public string BmrText
        {
            get => bmrText;
            private set => Set(ref bmrText, value);
        }

        public string BmiText
        {
            get => bmiText;
            private set => Set(ref bmiText, value);
        }

        public string DailyCalorieIndexText
        {
            get => dailyCalorieIndexText;
            private set => Set(ref dailyCalorieIndexText, value);
        }

        public string DailyCalorieIndexLabel => localization[GetDailyCalorieIndexKey(SelectedSign)];

        public bool HasErrors => errors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return errors.Values.ToList();
            }

            return errors.TryGetValue(propertyName, out string message)
                ? new[] { message }
                : null;
        }

        private static string GetDailyCalorieIndexKey(Sign sign)
        {
            switch (sign)
            {
                case Sign.Gain:
                    return "DailyCalorieIndexGain";
                case Sign.Loss:
                    return "DailyCalorieIndexLoss";
                default:
                    return "DailyCalorieIndexKeep";
            }
        }

        private void OnLocalizationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ILocalizationService.Language))
            {
                LanguageOption current = localization.Languages
                    .FirstOrDefault(option => option.Language == localization.Language);

                if (current != null && !ReferenceEquals(current, selectedLanguage))
                {
                    selectedLanguage = current;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }

            OnPropertyChanged(nameof(DailyCalorieIndexLabel));

            if (lastResultProfile != null)
            {
                DisplayResults(lastResultProfile);
            }
        }

        private void Calculate()
        {
            if (!TryBuildProfile(out Profile profile))
            {
                return;
            }

            DisplayResults(profile);
        }

        private void Save()
        {
            string id = (ProfileId ?? string.Empty).Trim();

            if (id.Length == 0)
            {
                dialogs.ShowMessage(
                    localization["ProfileNameRequiredMessage"],
                    localization["ProfileNameRequiredTitle"],
                    MessageKind.Warning);

                return;
            }

            if (!TryBuildProfile(out Profile profile))
            {
                return;
            }

            profile.Id = id;

            try
            {
                store.Save(profile);
            }
            catch (Exception exception)
            {
                dialogs.ShowMessage(
                    localization.Format("SaveFailedMessage", id) + Environment.NewLine + exception.Message,
                    localization["SaveFailedTitle"],
                    MessageKind.Error);

                return;
            }

            dialogs.ShowMessage(
                localization.Format("SavedMessage", id),
                localization["SavedTitle"],
                MessageKind.Information);
        }

        private void Load()
        {
            Profile profile = dialogs.SelectProfile(store);

            if (profile != null)
            {
                ApplyProfile(profile);
            }
        }

        private void ApplyProfile(Profile profile)
        {
            ProfileId = profile.Id;

            SelectedWeightUnit = FindUnit(WeightUnits, Units.Kilograms);
            WeightText = profile.Weight.ToString(localization.Culture);

            SelectedHeightUnit = FindUnit(HeightUnits, Units.Centimeters);
            HeightText = profile.Height.ToString(localization.Culture);

            AgeText = profile.Age.ToString(localization.Culture);
            DeviationText = profile.Deviation.ToString(localization.Culture);

            SelectedPal = PalOptions.FirstOrDefault(option => Math.Abs(option.Value - profile.Pal) < 0.0001)
                          ?? PalOptions[0];

            SelectedGender = profile.Gender;
            SelectedSign = profile.Sign;
        }

        private void DisplayResults(Profile profile)
        {
            lastResultProfile = profile;

            double basalMetabolicRate = Calculator.ComputeBasalMetabolicRate(
                profile.Weight,
                profile.Height,
                profile.Age,
                profile.Gender);

            double bodyMassIndex = Calculator.ComputeBodyMassIndex(profile.Weight, profile.Height);

            double dailyCalorieIndex = Calculator.ComputeDailyCalorieIndex(
                basalMetabolicRate,
                profile.Pal,
                profile.Sign,
                profile.Deviation);

            BmrText = basalMetabolicRate.ToString("####", localization.Culture);
            BmiText = bodyMassIndex.ToString("##.##", localization.Culture);
            DailyCalorieIndexText = dailyCalorieIndex.ToString("####", localization.Culture);
        }

        private bool TryBuildProfile(out Profile profile)
        {
            profile = null;

            ClearErrors();

            var invalidFields = new List<string>();

            if (!TryParseDouble(WeightText, out double weight))
            {
                AddError(nameof(WeightText), invalidFields);
            }

            if (!TryParseDouble(HeightText, out double height))
            {
                AddError(nameof(HeightText), invalidFields);
            }

            if (!short.TryParse(AgeText, NumberStyles.Integer, CultureInfo.CurrentCulture, out short age))
            {
                AddError(nameof(AgeText), invalidFields);
            }

            if (!TryParseDouble(DeviationText, out double deviation))
            {
                AddError(nameof(DeviationText), invalidFields);
            }

            if (SelectedPal == null)
            {
                AddError(nameof(SelectedPal), invalidFields);
                weight = height = deviation = 0;
            }

            if (invalidFields.Count > 0)
            {
                dialogs.ShowMessage(
                    localization.Format("InvalidInputMessage", string.Join(", ", invalidFields), Environment.NewLine),
                    localization["InvalidInputTitle"],
                    MessageKind.Warning);

                return false;
            }

            profile = new Profile
            {
                Id = (ProfileId ?? string.Empty).Trim(),
                Weight = Units.WeightToKilograms(weight, SelectedWeightUnit.Code),
                Height = Units.HeightToCentimeters(height, SelectedHeightUnit.Code),
                Age = age,
                Gender = SelectedGender,
                Sign = SelectedSign,
                Pal = SelectedPal.Value,
                Deviation = deviation,
            };

            return true;
        }

        private static UnitOption FindUnit(IReadOnlyList<UnitOption> units, string code)
        {
            return units.FirstOrDefault(unit => unit.Code == code) ?? units[0];
        }

        private void AddError(string propertyName, ICollection<string> invalidFields)
        {
            errors[propertyName] = localization["InvalidInputTitle"];

            if (FieldKeys.TryGetValue(propertyName, out string key))
            {
                invalidFields.Add(localization[key]);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors()
        {
            if (errors.Count == 0)
            {
                return;
            }

            var propertyNames = errors.Keys.ToList();
            errors.Clear();

            foreach (string propertyName in propertyNames)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private static bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                   || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
