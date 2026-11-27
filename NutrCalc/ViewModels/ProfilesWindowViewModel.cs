using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NutrCalc.Infrastructure;
using NutrCalc.Models;
using NutrCalc.Services;

namespace NutrCalc.ViewModels
{
    public sealed class ProfilesWindowViewModel : ViewModelBase
    {
        private readonly IProfileStore store;
        private Profile selectedProfile;

        public ProfilesWindowViewModel(IProfileStore store, ILocalizationService localization)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            Localization = localization ?? throw new ArgumentNullException(nameof(localization));

            Profiles = new ObservableCollection<Profile>(store.GetProfiles());

            SelectCommand = new RelayCommand(Select, () => SelectedProfile != null);
            DeleteCommand = new RelayCommand(Delete, () => SelectedProfile != null);
        }

        public event EventHandler CloseRequested;

        public ILocalizationService Localization { get; }

        public ObservableCollection<Profile> Profiles { get; }

        /// <summary>Выбранный профиль после закрытия окна; null означает отмену.</summary>
        public Profile Result { get; private set; }

        public ICommand SelectCommand { get; }

        public ICommand DeleteCommand { get; }

        public Profile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                if (Set(ref selectedProfile, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private void Select()
        {
            if (SelectedProfile == null)
            {
                return;
            }

            Result = SelectedProfile;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Delete()
        {
            Profile profile = SelectedProfile;

            if (profile == null)
            {
                return;
            }

            store.Delete(profile.Id);
            Profiles.Remove(profile);
            SelectedProfile = null;
        }
    }
}
