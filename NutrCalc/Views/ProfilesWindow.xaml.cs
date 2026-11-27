using System;
using System.Windows;
using NutrCalc.ViewModels;

namespace NutrCalc.Views
{
    public partial class ProfilesWindow : Window
    {
        public ProfilesWindow(ProfilesWindowViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            InitializeComponent();

            DataContext = viewModel;
            viewModel.CloseRequested += OnCloseRequested;
        }

        private void OnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
