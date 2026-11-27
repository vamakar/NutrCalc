namespace NutrCalc.Services
{
    public enum Language
    {
        English,
        Russian,
    }

    public sealed class LanguageOption
    {
        public LanguageOption(Language language, string name)
        {
            Language = language;
            Name = name;
        }

        public Language Language { get; }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
