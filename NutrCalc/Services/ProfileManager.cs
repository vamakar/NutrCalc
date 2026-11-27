using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NutrCalc.Models;

namespace NutrCalc.Services
{
    public class ProfileManager : IProfileStore, IDisposable
    {
        private readonly NutrCalcContext context;

        public ProfileManager()
            : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NutrCalc.db"))
        {
        }

        public ProfileManager(string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentException("Database path must not be empty.", nameof(databasePath));
            }

            DatabasePath = databasePath;
            context = new NutrCalcContext(databasePath);
            context.EnsureCreated();
        }

        public string DatabasePath { get; }

        public IReadOnlyList<Profile> GetProfiles()
        {
            return context.Profiles
                .OrderBy(profile => profile.Id)
                .ToList();
        }

        public Profile GetProfile(string id)
        {
            return context.Profiles
                .AsNoTracking()
                .FirstOrDefault(profile => profile.Id == id);
        }

        public void Save(Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (string.IsNullOrWhiteSpace(profile.Id))
            {
                throw new ArgumentException("Profile id must not be empty.", nameof(profile));
            }

            var stored = context.Profiles.Find(profile.Id);

            if (stored == null)
            {
                context.Profiles.Add(profile);
            }
            else
            {
                context.Entry(stored).CurrentValues.SetValues(profile);
            }

            context.SaveChanges();
        }

        public void Delete(string id)
        {
            var stored = context.Profiles.Find(id);

            if (stored == null)
            {
                return;
            }

            context.Profiles.Remove(stored);
            context.SaveChanges();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
