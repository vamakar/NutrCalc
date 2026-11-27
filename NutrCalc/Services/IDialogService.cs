using NutrCalc.Models;

namespace NutrCalc.Services
{
    public enum MessageKind
    {
        Information,
        Warning,
        Error,
    }

    public interface IDialogService
    {
        void ShowMessage(string message, string title, MessageKind kind);

        Profile SelectProfile(IProfileStore store);
    }
}
