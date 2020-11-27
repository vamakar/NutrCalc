using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NutrCalc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string ProfilesLocation => $"{AppDomain.CurrentDomain.BaseDirectory}/profiles.xml";
        private static string ProfilesScheme => "NutrCalc.Profiles.xsd";
        internal static Dictionary<string, Profile> ProfileDictionary { get; set; } = GetProfiles();

        private static Profile CurrentProfile { get; set; }

        private static Sign _sign;
        private static Gender _gender;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BMI_Calc_Click(object sender, RoutedEventArgs e)
        {
            var weight = Convert.ToDouble(TextBoxWeight.Text);
            var height = Convert.ToDouble(TextBoxHeight.Text);
            var age = Convert.ToInt16(TextBoxAge.Text);
            var pal = Convert.ToDouble(((ComboBoxItem)ComboBoxPAL.SelectedItem).Content.ToString().Replace('.',','));
            var deviation = Convert.ToDouble(TextBoxDeviation.Text);

            if (ComboBoxWeight.SelectedIndex == 1)
            {
                weight *= 2.20462;
            }

            switch (ComboBoxHeight.SelectedIndex)
            {
                case 0:
                    height /= 100;
                    break;
                case 2:
                    height *= 0.00393701;
                    break;
            }

            double mr;
            switch (_gender)
            {
                case Gender.Male:
                    mr = weight * 9.99 + height * 625 - age * 4.92 + 5;
                    break;
                case Gender.Female:
                    mr = weight * 9.99 + height * 625 - age * 4.92 - 161;
                    break;
                default:
                    return;
            }

            double dcn = mr * pal;

            string forWeight;
            double dci;
            switch (_sign)
            {
                case Sign.Gain:
                    forWeight = "for weight gain";
                    dci = dcn * (1 + deviation / 100);
                    break;
                case Sign.Keep:
                    forWeight = "to keep weight";
                    dci = dcn;
                    break;
                case Sign.Loss:
                    forWeight = "for weight loss";
                    dci = dcn * (1 - deviation / 100);
                    break;
                default:
                    return;
            }

            TextBlockBMI.Text = $"BMI: {weight / height / height}";
            TextBlockMR.Text = $"MR: {mr}";
            TextBlockDCN.Text = $"DCN: {dcn}";
            TextBlockDCI.Text = $"DCI {forWeight}: {dci}";

            CurrentProfile = new Profile
            {
                id = string.Empty,
                weight = weight,
                height = height,
                age = age,
                gender = _gender,
                sign = _sign,
                pal = pal,
                deviation = deviation
            };
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var profilesWindow = new ProfilesWindow();
            profilesWindow.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveInputData(IdTextBox.Text);
        }

        private void SaveInputData(string id)
        {
            CurrentProfile.id = IdTextBox.Text;

            if (ProfileDictionary.ContainsKey(id))
            {
                ProfileDictionary[id] = CurrentProfile;
            }
            else
            {
                ProfileDictionary.Add(id, CurrentProfile);
            }

            var writer = new XmlSerializer(typeof(Profiles));
            var xmlDoc = new XmlDocument();

            using (var memStr = new MemoryStream())
            {
                writer.Serialize(memStr, new Profiles{profile = ProfileDictionary.Values.ToArray()});
                memStr.Position = 0;
                xmlDoc.Load(memStr);
            }

            xmlDoc.Save(ProfilesLocation);
        }

        private static Dictionary<string, Profile> GetProfiles()
        {
            var xmlDoc = new XmlDocument();

            if (!File.Exists(ProfilesLocation)) return new Dictionary<string, Profile>();

            xmlDoc.Load(ProfilesLocation);
            var eventHandler = new ValidationEventHandler(ValidationEventHandler);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ProfilesScheme))
            {
                XmlSchema xmlSchema = XmlSchema.Read(stream, eventHandler);
                xmlDoc.Schemas.Add(xmlSchema);
                xmlDoc.Validate(eventHandler);
            }

            using (var xmlReader = new XmlTextReader(ProfilesLocation))
            {
                var xmlSerializer = new XmlSerializer(typeof(Profiles));
                var profiles = (Profiles) xmlSerializer.Deserialize(xmlReader);

                return profiles.profile.ToDictionary(x => x.id, x=> x);
            }
        }

        internal void LoadProfile(string key)
        {
            Profile profile = ProfileDictionary[key];

            IdTextBox.Text = profile.id;

            TextBoxWeight.Text = profile.weight.ToString();
            TextBoxHeight.Text = (profile.height * 100).ToString();
            TextBoxAge.Text = profile.age.ToString();

            switch (profile.gender)
            {
                case Gender.Male:
                    GenderMale.IsChecked = true;
                    break;
                case Gender.Female:
                    GenderFemale.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (profile.sign)
            {
                case Sign.Gain:
                    DeviationGain.IsChecked = true;
                    break;
                case Sign.Keep:
                    DeviationKeep.IsChecked = true;
                    break;
                case Sign.Loss:
                    DeviationLoss.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (profile.pal)
            {
                case 1.2:
                    ComboBoxPAL.SelectedIndex = 0;
                    break;
                case 1.3:
                    ComboBoxPAL.SelectedIndex = 1;
                    break;
                case 1.4:
                    ComboBoxPAL.SelectedIndex = 2;
                    break;
                case 1.5:
                    ComboBoxPAL.SelectedIndex = 3;
                    break;
                case 1.6:
                    ComboBoxPAL.SelectedIndex = 4;
                    break;
                case 1.7:
                    ComboBoxPAL.SelectedIndex = 5;
                    break;
                case 1.8:
                    ComboBoxPAL.SelectedIndex = 6;
                    break;
                case 1.9:
                    ComboBoxPAL.SelectedIndex = 7;
                    break;
                case 2.0:
                    ComboBoxPAL.SelectedIndex = 8;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            TextBoxDeviation.Text = profile.deviation.ToString();

            CurrentProfile = new Profile
            {
                id = profile.id,
                weight = profile.weight,
                height = profile.height,
                age = profile.age,
                gender = profile.gender,
                sign = profile.sign,
                pal = profile.pal,
                deviation = profile.deviation
            };
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
                            _sign = Sign.Gain;
                            break;
                        case "DeviationKeep":
                            _sign = Sign.Keep;
                            break;
                        case "DeviationLoss":
                            _sign = Sign.Loss;
                            break;
                    }
                    break;
                case "Gender":
                    switch (radioButton.Name)
                    {
                        case "GenderMale":
                            _gender = Gender.Male;
                            break;
                        case "GenderFemale":
                            _gender = Gender.Female;
                            break;
                    }
                    break;
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    MessageBox.Show(e.Message, "Error");
                    break;
                case XmlSeverityType.Warning:
                    MessageBox.Show(e.Message, "Warning");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
