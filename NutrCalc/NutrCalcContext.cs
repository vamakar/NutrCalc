using System.Data.Entity;
using System.Data.SQLite;

namespace NutrCalc
{
    public class NutrCalcContext : DbContext
    {
        private const string CreateProfilesTable =
            "CREATE TABLE IF NOT EXISTS Profiles (" +
            "Id TEXT NOT NULL CONSTRAINT PK_Profiles PRIMARY KEY, " +
            "Weight REAL NOT NULL, " +
            "Height REAL NOT NULL, " +
            "Age INTEGER NOT NULL, " +
            "Gender INTEGER NOT NULL, " +
            "Sign INTEGER NOT NULL, " +
            "Pal REAL NOT NULL, " +
            "Deviation REAL NOT NULL)";

        public NutrCalcContext(string databasePath)
            : base(new SQLiteConnection($"Data Source={databasePath};Version=3;"), true)
        {
            // Схема создаётся вручную в EnsureCreated: SQLite-провайдер EF6
            // не умеет создавать таблицы через CreateIfNotExists.
            System.Data.Entity.Database.SetInitializer<NutrCalcContext>(null);
        }

        public DbSet<Profile> Profiles { get; set; }

        public void EnsureCreated()
        {
            Database.ExecuteSqlCommand(CreateProfilesTable);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .ToTable("Profiles")
                .HasKey(profile => profile.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
