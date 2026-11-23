using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NutrCalc
{
    public partial class ProfilesWindow : Window
    {
        private ProfileManager ProfileManager { get; }

        internal Profile SelectedProfile { get; private set; }

        internal ProfilesWindow(ProfileManager profileManager)
        {
            ProfileManager = profileManager;

            InitializeComponent();

            DisplayProfiles();
        }

        private void DisplayProfiles()
        {
            foreach (var profile in ProfileManager.GetProfiles())
            {
                var newItem = new ListViewItem { Tag = profile.Id, Content = profile.Id };
                newItem.MouseDoubleClick += ProfilesPanel_OnMouseDoubleClick;

                ProfilesPanel.Items.Add(newItem);
            }
        }

        private bool SelectProfile(ListViewItem item)
        {
            if (item == null)
            {
                return false;
            }

            SelectedProfile = ProfileManager.GetProfile((string) item.Tag);

            if (SelectedProfile == null)
            {
                return false;
            }

            DialogResult = true;
            return true;
        }

        private void ProfilesPanel_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectProfile(e.Source as ListViewItem);
        }

        private void SelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ListViewItem item in ProfilesPanel.Items)
            {
                if (!item.IsSelected)
                {
                    continue;
                }

                if (SelectProfile(item))
                {
                    return;
                }

                break;
            }
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (ListViewItem item in ProfilesPanel.Items)
            {
                if (!item.IsSelected)
                {
                    continue;
                }

                ProfileManager.Delete((string) item.Tag);
                ProfilesPanel.Items.Remove(item);

                break;
            }
        }
    }
}
