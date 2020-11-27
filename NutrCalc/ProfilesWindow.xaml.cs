using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NutrCalc
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ProfilesWindow : Window
    {
        public ProfilesWindow()
        {
            InitializeComponent();

            DisplayProfiles();
        }

        private void DisplayProfiles()
        {
            foreach (string profile in MainWindow.ProfileDictionary.Keys)
            {
                var newItem = new ListViewItem() {Name = profile, Content = profile};
                newItem.MouseDoubleClick += ProfilesPanel_OnMouseDoubleClick;

                ProfilesPanel.Items.Add(newItem);
            }
        }

        private void LoadProfileById(IFrameworkInputElement listViewItem)
        {
            MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mainWindow?.LoadProfile(listViewItem?.Name);

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

                MainWindow.ProfileDictionary.Remove(item.Name);
                ProfilesPanel.Items.Remove(item);

                break;
            }
        }
    }
}
