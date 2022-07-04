using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace NutrCalc
{
    public partial class MainWindow : Window
    {
        internal Profile CurrentProfile { get; set; }
        private ProfileManager ProfileManager { get; }
        private string WeightDevText { get; set; }

        public MainWindow()
        {
            ProfileManager = new ProfileManager(this);
            CurrentProfile = new Profile
            {
                id = string.Empty
            };

            InitializeComponent();
        }

        private void BMI_Calc_Click(object sender, RoutedEventArgs e)
        {
            double weight;
            double height;
            short age;
            double physActLevel;
            double deviation;
            try
            {
                weight = Convert.ToDouble(TextBoxWeight.Text);
                height = Convert.ToDouble(TextBoxHeight.Text);
                age = Convert.ToInt16(TextBoxAge.Text);
                physActLevel = Convert.ToDouble(((ComboBoxItem)ComboBoxPAL.SelectedItem).Content.ToString().Replace('.', ','));
                deviation = Convert.ToDouble(TextBoxDeviation.Text);
            }
            catch (FormatException)
            {
                return;
            }

            if (ComboBoxWeight.SelectedIndex == 1)
            {
                weight *= 2.20462;
            }

            double measSysCoeff = GetMeasSysCoeff();
            height *= measSysCoeff;

            double genderCoeff = GetGenderCoeff(CurrentProfile.gender);
            double deviationCoeff = GetDeviationCoeff(CurrentProfile.sign);
            double metabolicRate = CalcMr(weight, height, age, genderCoeff);
            double dailyCalorieIndex = metabolicRate * physActLevel * (1 + deviationCoeff * deviation / 100);

            DisplayResults(weight, height, metabolicRate, dailyCalorieIndex);

            CurrentProfile.weight = weight;
            CurrentProfile.height = height;
            CurrentProfile.age = age;
            CurrentProfile.pal = physActLevel;
            CurrentProfile.deviation = deviation;
        }

        private double GetMeasSysCoeff()
        {
            double measSysCoeff;
            switch (ComboBoxHeight.SelectedIndex)
            {
                case 0:
                    measSysCoeff = 0.01;
                    break;
                case 2:
                    measSysCoeff = 0.00393701;
                    break;
                default:
                    measSysCoeff = 1;
                    break;
            }

            return measSysCoeff;
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

        private static double CalcMr(double weight, double height, short age, double genderCoeff)
        {
            return weight * 9.99 + height * 625 - age * 4.92 + genderCoeff;
        }

        private double GetDeviationCoeff(Sign sign)
        {
            double deviationCoeff;
            switch (sign)
            {
                case Sign.Gain:
                    WeightDevText = "for weight gain";
                    deviationCoeff = 1;
                    break;
                case Sign.Keep:
                    WeightDevText = "to keep weight";
                    deviationCoeff = 0;
                    break;
                case Sign.Loss:
                    WeightDevText = "for weight loss";
                    deviationCoeff = -1;
                    break;
                default:
                    throw new Exception("Unknown case");
            }

            return deviationCoeff;
        }

        private void DisplayResults(double weight, double height, double mr, double dci)
        {
            TextBlockBmiText.Text = "Basal metabolic rate:";
            TextBlockBmiNum.Text = (weight / height / height).ToString("##.##", CultureInfo.InvariantCulture);
            TextBlockMrText.Text = "Metabolic rate:";
            TextBlockMrNum.Text = mr.ToString("####", CultureInfo.InvariantCulture);
            TextBlockDciText.Text = $"Daily calorie index {WeightDevText}:";
            TextBlockDciNum.Text = dci.ToString("####", CultureInfo.InvariantCulture);
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var profilesWindow = new ProfilesWindow(ProfileManager);
            profilesWindow.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            ProfileManager.SaveInputData(IdTextBox.Text);
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
                            CurrentProfile.sign = Sign.Gain;
                            break;
                        case "DeviationKeep":
                            CurrentProfile.sign = Sign.Keep;
                            break;
                        case "DeviationLoss":
                            CurrentProfile.sign = Sign.Loss;
                            break;
                    }
                    break;
                case "Gender":
                    switch (radioButton.Name)
                    {
                        case "GenderMale":
                            CurrentProfile.gender = Gender.Male;
                            break;
                        case "GenderFemale":
                            CurrentProfile.gender = Gender.Female;
                            break;
                    }
                    break;
            }
        }
    }
}
