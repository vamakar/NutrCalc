using System.Collections.Generic;
using NutrCalc.Models;

namespace NutrCalc.Services
{
    public interface IProfileStore
    {
        IReadOnlyList<Profile> GetProfiles();

        Profile GetProfile(string id);

        void Save(Profile profile);

        void Delete(string id);
    }
}
