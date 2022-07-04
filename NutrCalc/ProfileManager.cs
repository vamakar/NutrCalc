using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NutrCalc
{
    internal class ProfileManager
    {
        internal Dictionary<string, Profile> ProfileDictionary { get; }
        private MainWindow MainWindow { get; }
        private string ProfilesLocation => $"{AppDomain.CurrentDomain.BaseDirectory}/profiles.xml";
        private string ProfilesScheme => "NutrCalc.Profiles.xsd";

        public ProfileManager(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            ProfileDictionary = GetProfiles();
        }

        internal void SaveInputData(string id)
        {
            MainWindow.CurrentProfile.id = MainWindow.IdTextBox.Text;

            if (ProfileDictionary.ContainsKey(id))
            {
                ProfileDictionary[id] = MainWindow.CurrentProfile;
            }
            else
            {
                ProfileDictionary.Add(id, MainWindow.CurrentProfile);
            }

            var writer = new XmlSerializer(typeof(Profiles));
            var xmlDoc = new XmlDocument();

            using (var memStr = new MemoryStream())
            {
                writer.Serialize(memStr, new Profiles { profile = ProfileDictionary.Values.ToArray() });
                memStr.Position = 0;
                xmlDoc.Load(memStr);
            }

            xmlDoc.Save(ProfilesLocation);
        }

        internal Dictionary<string, Profile> GetProfiles()
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
                var profiles = (Profiles)xmlSerializer.Deserialize(xmlReader);

                return profiles.profile.ToDictionary(x => x.id, x => x);
            }
        }

        internal void LoadProfile(string key)
        {
            Profile profile = ProfileDictionary[key];

            MainWindow.IdTextBox.Text = profile.id;

            MainWindow.TextBoxWeight.Text = profile.weight.ToString();
            MainWindow.TextBoxHeight.Text = (profile.height * 100).ToString();
            MainWindow.TextBoxAge.Text = profile.age.ToString();

            switch (profile.gender)
            {
                case Gender.Male:
                    MainWindow.GenderMale.IsChecked = true;
                    break;
                case Gender.Female:
                    MainWindow.GenderFemale.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (profile.sign)
            {
                case Sign.Gain:
                    MainWindow.DeviationGain.IsChecked = true;
                    break;
                case Sign.Keep:
                    MainWindow.DeviationKeep.IsChecked = true;
                    break;
                case Sign.Loss:
                    MainWindow.DeviationLoss.IsChecked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (profile.pal)
            {
                case 1.2:
                    MainWindow.ComboBoxPAL.SelectedIndex = 0;
                    break;
                case 1.3:
                    MainWindow.ComboBoxPAL.SelectedIndex = 1;
                    break;
                case 1.4:
                    MainWindow.ComboBoxPAL.SelectedIndex = 2;
                    break;
                case 1.5:
                    MainWindow.ComboBoxPAL.SelectedIndex = 3;
                    break;
                case 1.6:
                    MainWindow.ComboBoxPAL.SelectedIndex = 4;
                    break;
                case 1.7:
                    MainWindow.ComboBoxPAL.SelectedIndex = 5;
                    break;
                case 1.8:
                    MainWindow.ComboBoxPAL.SelectedIndex = 6;
                    break;
                case 1.9:
                    MainWindow.ComboBoxPAL.SelectedIndex = 7;
                    break;
                case 2.0:
                    MainWindow.ComboBoxPAL.SelectedIndex = 8;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            MainWindow.TextBoxDeviation.Text = profile.deviation.ToString();

            MainWindow.CurrentProfile = new Profile
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
