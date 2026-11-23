using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NutrCalc
{
    public partial class MainWindow : Window
    {
        internal Profile CurrentProfile { get; set; }
        private ProfileManager ProfileManager { get; }

        public MainWindow()
        {
            ProfileManager = new ProfileManager();
            CurrentProfile = new Profile
            {
                Id = string.Empty
            };

            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            ProfileManager.Dispose();
            base.OnClosed(e);
        }

        private void BMI_Calc_Click(object sender, RoutedEventArgs e)
        {
            if (!TryBuildProfile(IdTextBox.Text.Trim(), out Profile profile))
            {
                return;
            }

            CurrentProfile = profile;
            DisplayResults(profile);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            string id = IdTextBox.Text.Trim();

            if (id.Length == 0)
            {
                MessageBox.Show(
                    "Enter a profile name in the ID field before saving.",
                    "Profile name required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                IdTextBox.Focus();
                return;
            }

            if (!TryBuildProfile(id, out Profile profile))
            {
                return;
            }

            CurrentProfile = profile;

            try
            {
                ProfileManager.Save(profile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Profile \"{id}\" was not saved.{Environment.NewLine}{exception.Message}",
                    "Save failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                $"Profile \"{id}\" saved.",
                "Saved",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var profilesWindow = new ProfilesWindow(ProfileManager)
            {
                Owner = this
            };

            if (profilesWindow.ShowDialog() == true && profilesWindow.SelectedProfile != null)
            {
                ApplyProfile(profilesWindow.SelectedProfile);
            }
        }

        private void ApplyProfile(Profile profile)
        {
            IdTextBox.Text = profile.Id;
            ComboBoxWeight.SelectedIndex = 0;
            TextBoxWeight.Text = profile.Weight.ToString(CultureInfo.CurrentCulture);
            ComboBoxHeight.SelectedIndex = 0;
            TextBoxHeight.Text = profile.Height.ToString(CultureInfo.CurrentCulture);
            TextBoxAge.Text = profile.Age.ToString(CultureInfo.CurrentCulture);
            TextBoxDeviation.Text = profile.Deviation.ToString(CultureInfo.CurrentCulture);

            ComboBoxPAL.SelectedIndex = GetPalIndex(profile.Pal);

            GenderMale.IsChecked = profile.Gender == Gender.Male;
            GenderFemale.IsChecked = profile.Gender == Gender.Female;
            DeviationGain.IsChecked = profile.Sign == Sign.Gain;
            DeviationKeep.IsChecked = profile.Sign == Sign.Keep;
            DeviationLoss.IsChecked = profile.Sign == Sign.Loss;

            CurrentProfile = profile;
        }

        private int GetPalIndex(double pal)
        {
            for (var index = 0; index < ComboBoxPAL.Items.Count; index++)
            {
                var item = (ComboBoxItem)ComboBoxPAL.Items[index];

                if (TryParseDouble(item.Content.ToString(), out double value) && Math.Abs(value - pal) < 0.0001)
                {
                    return index;
                }
            }

            return 0;
        }

        private bool TryBuildProfile(string id, out Profile profile)
        {
            profile = null;

            if (!TryReadInputs(out double weight, out double height, out short age, out double physActLevel, out double deviation))
            {
                return false;
            }

            profile = new Profile
            {
                Id = id,
                Weight = Units.WeightToKilograms(weight, GetUnit(ComboBoxWeight, "kg")),
                Height = Units.HeightToCentimeters(height, GetUnit(ComboBoxHeight, "cm")),
                Age = age,
                Gender = CurrentProfile.Gender,
                Sign = CurrentProfile.Sign,
                Pal = physActLevel,
                Deviation = deviation
            };

            return true;
        }

        private static string GetUnit(ComboBox comboBox, string fallback)
        {
            return (comboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? fallback;
        }

        private bool TryReadInputs(out double weight, out double height, out short age, out double physActLevel, out double deviation)
        {
            weight = 0;
            height = 0;
            age = 0;
            deviation = 0;
            physActLevel = 0;

            var invalidFields = new List<string>();
            TextBox firstInvalidField = null;

            if (!TryParseDouble(TextBoxWeight.Text, out weight))
            {
                invalidFields.Add("weight");
                firstInvalidField = firstInvalidField ?? TextBoxWeight;
            }

            if (!TryParseDouble(TextBoxHeight.Text, out height))
            {
                invalidFields.Add("height");
                firstInvalidField = firstInvalidField ?? TextBoxHeight;
            }

            if (!short.TryParse(TextBoxAge.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out age))
            {
                invalidFields.Add("age");
                firstInvalidField = firstInvalidField ?? TextBoxAge;
            }

            if (!TryParseDouble(TextBoxDeviation.Text, out deviation))
            {
                invalidFields.Add("deviation");
                firstInvalidField = firstInvalidField ?? TextBoxDeviation;
            }

            if (!(ComboBoxPAL.SelectedItem is ComboBoxItem palItem) || !TryParseDouble(palItem.Content.ToString(), out physActLevel))
            {
                invalidFields.Add("PAL");
            }

            if (invalidFields.Count == 0)
            {
                return true;
            }

            MessageBox.Show(
                $"Please enter numbers for: {string.Join(", ", invalidFields)}.{Environment.NewLine}" +
                "The decimal separator can be a comma or a dot, e.g. 70,5.",
                "Invalid input",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            firstInvalidField?.Focus();
            firstInvalidField?.SelectAll();

            return false;
        }

        private static bool TryParseDouble(string text, out double value)
        {
            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)
                   || double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static double GetGenderCoeff(Gender gender)
        {
            double genderCoeff;
            switch (gender)
            {
                case Gender.Male:
                    genderCoeff = 5;
                    break;
                case Gender.Female:
                    genderCoeff = -161;
                    break;
                default:
                    throw new Exception("Unknown case");
            }

            return genderCoeff;
        }

        private static double CalcBmr(double weight, double heightInMeters, short age, double genderCoeff)
        {
            return weight * 9.99 + heightInMeters * 625 - age * 4.92 + genderCoeff;
        }

        private static double GetDeviationCoeff(Sign sign)
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
                    throw new Exception("Unknown case");
            }
        }

        private static string GetDeviationText(Sign sign)
        {
            switch (sign)
            {
                case Sign.Gain:
                    return "for weight gain";
                case Sign.Keep:
                    return "to keep weight";
                case Sign.Loss:
                    return "for weight loss";
                default:
                    throw new Exception("Unknown case");
            }
        }

        private void DisplayResults(Profile profile)
        {
            double heightInMeters = profile.Height / 100;
            double genderCoeff = GetGenderCoeff(profile.Gender);
            double deviationCoeff = GetDeviationCoeff(profile.Sign);
            double basalMetabolicRate = CalcBmr(profile.Weight, heightInMeters, profile.Age, genderCoeff);
            double bodyMassIndex = profile.Weight / heightInMeters / heightInMeters;
            double dailyCalorieIndex = basalMetabolicRate * profile.Pal * (1 + deviationCoeff * profile.Deviation / 100);

            TextBlockBmrText.Text = "Basal metabolic rate:";
            TextBlockBmrNum.Text = basalMetabolicRate.ToString("####", CultureInfo.InvariantCulture);
            TextBlockBmiText.Text = "Body mass index:";
            TextBlockBmiNum.Text = bodyMassIndex.ToString("##.##", CultureInfo.InvariantCulture);
            TextBlockDciText.Text = $"Daily calorie index {GetDeviationText(profile.Sign)}:";
            TextBlockDciNum.Text = dailyCalorieIndex.ToString("####", CultureInfo.InvariantCulture);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton) sender;

            switch (radioButton.GroupName)
            {
                case "Deviation":
                    switch (radioButton.Name)
                    {
                        case "DeviationGain":
                            CurrentProfile.Sign = Sign.Gain;
                            break;
                        case "DeviationKeep":
                            CurrentProfile.Sign = Sign.Keep;
                            break;
                        case "DeviationLoss":
                            CurrentProfile.Sign = Sign.Loss;
                            break;
                    }
                    break;
                case "Gender":
                    switch (radioButton.Name)
                    {
                        case "GenderMale":
                            CurrentProfile.Gender = Gender.Male;
                            break;
                        case "GenderFemale":
                            CurrentProfile.Gender = Gender.Female;
                            break;
                    }
                    break;
            }
        }
    }
}
