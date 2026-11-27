using System;
using System.Windows;
using NutrCalc.Services;
using NutrCalc.ViewModels;
using NutrCalc.Views;

namespace NutrCalc
{
    public partial class App : Application
    {
        private ProfileManager profileManager;
        private DialogService dialogService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var localization = new LocalizationService();

            try
            {
                profileManager = new ProfileManager();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    localization.Format(
                        "DatabaseErrorMessage",
                        Environment.NewLine,
                        exception.Message),
                    localization["DatabaseErrorTitle"],
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Shutdown(1);
                return;
            }

            dialogService = new DialogService(localization);

            var viewModel = new MainWindowViewModel(profileManager, dialogService, localization);
            var window = new MainWindow { DataContext = viewModel };

            dialogService.Owner = window;
            MainWindow = window;

            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            profileManager?.Dispose();

            base.OnExit(e);
        }
    }
}
