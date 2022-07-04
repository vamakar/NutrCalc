using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NutrCalc
{
    public partial class ProfilesWindow : Window
    {
        private ProfileManager ProfileManager { get; }

        internal ProfilesWindow(ProfileManager profileManager)
        {
            ProfileManager = profileManager;

            InitializeComponent();

            DisplayProfiles();
        }

        private void DisplayProfiles()
        {
            foreach (string profile in ProfileManager.ProfileDictionary.Keys)
            {
                var newItem = new ListViewItem() {Name = profile, Content = profile};
                newItem.MouseDoubleClick += ProfilesPanel_OnMouseDoubleClick;

                ProfilesPanel.Items.Add(newItem);
            }
        }

        private void LoadProfileById(IFrameworkInputElement listViewItem)
        {
            ProfileManager.LoadProfile(listViewItem?.Name);

            Close();
        }

        private void ProfilesPanel_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = e.Source as ListViewItem;
            LoadProfileById(item);
        }

        private void SelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ListViewItem item in ProfilesPanel.Items)
            {
                if (!item.IsSelected) continue;

                LoadProfileById(item);
                break;
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ListViewItem item in ProfilesPanel.Items)
            {
                if (!item.IsSelected) continue;

                ProfileManager.ProfileDictionary.Remove(item.Name);
                ProfilesPanel.Items.Remove(item);

                break;
            }
        }
    }
}
