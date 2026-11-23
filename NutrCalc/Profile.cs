using System.ComponentModel.DataAnnotations;

namespace NutrCalc
{
    public class Profile
    {
        [Key]
        public string Id { get; set; }

        /// <summary>Вес в килограммах.</summary>
        public double Weight { get; set; }

        /// <summary>Рост в сантиметрах.</summary>
        public double Height { get; set; }

        public short Age { get; set; }

        public Gender Gender { get; set; }

        public Sign Sign { get; set; }

        public double Pal { get; set; }

        public double Deviation { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
    }

    public enum Sign
    {
        Gain,
        Keep,
        Loss,
    }
}
