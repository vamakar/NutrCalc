using System;
using System.Windows;
using NutrCalc.Models;
using NutrCalc.Services;
using NutrCalc.ViewModels;

namespace NutrCalc.Views
{
    public sealed class DialogService : IDialogService
    {
        private readonly ILocalizationService localization;

        public DialogService(ILocalizationService localization)
        {
            this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        }

        public Window Owner { get; set; }

        public void ShowMessage(string message, string title, MessageKind kind)
        {
            MessageBoxImage image;

            switch (kind)
            {
                case MessageKind.Error:
                    image = MessageBoxImage.Error;
                    break;
                case MessageKind.Warning:
                    image = MessageBoxImage.Warning;
                    break;
                default:
                    image = MessageBoxImage.Information;
                    break;
            }

            if (Owner != null)
            {
                MessageBox.Show(Owner, message, title, MessageBoxButton.OK, image);
            }
            else
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, image);
            }
        }

        public Profile SelectProfile(IProfileStore store)
        {
            var viewModel = new ProfilesWindowViewModel(store, localization);
            var window = new ProfilesWindow(viewModel) { Owner = Owner };

            window.ShowDialog();

            return viewModel.Result;
        }
    }
}
