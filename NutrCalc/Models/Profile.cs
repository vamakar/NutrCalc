using System.ComponentModel.DataAnnotations;

namespace NutrCalc.Models
{
    public class Profile
    {
        [Key]
        public string Id { get; set; }

        public double Weight { get; set; }

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
