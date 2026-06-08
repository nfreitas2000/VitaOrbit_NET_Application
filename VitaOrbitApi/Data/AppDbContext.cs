using Microsoft.EntityFrameworkCore;
using VitaOrbitApi.Models;

namespace VitaOrbitApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<SymptomRecord> SymptomRecords { get; set; }
        public DbSet<EnvironmentalCondition> EnvironmentalConditions { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.FullName).HasColumnName("FULL_NAME");
                entity.Property(e => e.Email).HasColumnName("EMAIL");
                entity.Property(e => e.Password).HasColumnName("PASSWORD");
                entity.Property(e => e.BirthDate).HasColumnName("BIRTH_DATE");
                entity.Property(e => e.Gender).HasColumnName("GENDER");
                entity.Property(e => e.UserDescription).HasColumnName("USER_DESCRIPTION");
                entity.Property(e => e.CurrentLocation).HasColumnName("CURRENT_LOCATION");
                entity.Property(e => e.PhoneNumber).HasColumnName("PHONE_NUMBER");
                entity.Property(e => e.EmergencyContact).HasColumnName("EMERGENCY_CONTACT");
                entity.Property(e => e.RegisteredAt).HasColumnName("REGISTERED_AT");

                entity.HasIndex(e => e.Email).IsUnique();

                entity.HasMany(e => e.HealthRecords)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.SymptomRecords)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasMany(e => e.Emergencies)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.EnvironmentalCondition)
                    .WithOne(e => e.User)
                    .HasForeignKey<EnvironmentalCondition>(e => e.UserId);
            });

            modelBuilder.Entity<HealthRecord>(entity =>
            {
                entity.ToTable("HEALTH_RECORDS");

                entity.HasKey(e => e.HealthRecordId);

                entity.Property(e => e.HealthRecordId).HasColumnName("HEALTH_RECORD_ID");
                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.HeartRate).HasColumnName("HEART_RATE");
                entity.Property(e => e.SystolicPressure).HasColumnName("SYSTOLIC_PRESSURE");
                entity.Property(e => e.DiastolicPressure).HasColumnName("DIASTOLIC_PRESSURE");
                entity.Property(e => e.BodyTemperature).HasColumnName("BODY_TEMPERATURE");
                entity.Property(e => e.OxygenSaturation).HasColumnName("OXYGEN_SATURATION");
                entity.Property(e => e.HydrationLevel).HasColumnName("HYDRATION_LEVEL");
                entity.Property(e => e.SleepHours).HasColumnName("SLEEP_HOURS");
                entity.Property(e => e.Notes).HasColumnName("NOTES");
                entity.Property(e => e.RiskClassification).HasColumnName("RISK_CLASSIFICATION");
                entity.Property(e => e.RegisteredAt).HasColumnName("REGISTERED_AT");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.HealthRecords)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<SymptomRecord>(entity =>
            {
                entity.ToTable("SYMPTOM_RECORDS");

                entity.HasKey(e => e.SymptomRecordId);

                entity.Property(e => e.SymptomRecordId).HasColumnName("SYMPTOM_RECORD_ID");
                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.SymptomName).HasColumnName("SYMPTOM_NAME");
                entity.Property(e => e.Intensity).HasColumnName("INTENSITY");
                entity.Property(e => e.Frequency).HasColumnName("FREQUENCY");
                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");
                entity.Property(e => e.StartedAt).HasColumnName("STARTED_AT");
                entity.Property(e => e.RiskClassification).HasColumnName("RISK_CLASSIFICATION");
                entity.Property(e => e.RegisteredAt).HasColumnName("REGISTERED_AT");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.SymptomRecords)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<EnvironmentalCondition>(entity =>
            {
                entity.ToTable("ENVIRONMENTAL_CONDITIONS");

                entity.HasKey(e => e.EnvironmentalConditionId);

                entity.Property(e => e.EnvironmentalConditionId).HasColumnName("ENVIRONMENTAL_CONDITION_ID");
                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.ExternalTemperature).HasColumnName("EXTERNAL_TEMPERATURE");
                entity.Property(e => e.Humidity).HasColumnName("HUMIDITY");
                entity.Property(e => e.Altitude).HasColumnName("ALTITUDE");
                entity.Property(e => e.AtmosphericPressure).HasColumnName("ATMOSPHERIC_PRESSURE");
                entity.Property(e => e.AirQuality).HasColumnName("AIR_QUALITY");
                entity.Property(e => e.RadiationLevel).HasColumnName("RADIATION_LEVEL");
                entity.Property(e => e.EnvironmentType).HasColumnName("ENVIRONMENT_TYPE");
                entity.Property(e => e.RegisteredAt).HasColumnName("REGISTERED_AT");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.EnvironmentalCondition)
                    .HasForeignKey<EnvironmentalCondition>(e => e.UserId);
            });

            modelBuilder.Entity<Emergency>(entity =>
            {
                entity.ToTable("EMERGENCIES");

                entity.HasKey(e => e.EmergencyId);

                entity.Property(e => e.EmergencyId).HasColumnName("EMERGENCY_ID");
                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.Location).HasColumnName("LOCATION");
                entity.Property(e => e.Message).HasColumnName("MESSAGE");
                entity.Property(e => e.Status).HasColumnName("STATUS");
                entity.Property(e => e.RequestDate).HasColumnName("REQUEST_DATE");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Emergencies)
                    .HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<Alert>(entity =>
            {
                entity.ToTable("ALERTS");

                entity.HasKey(e => e.AlertId);

                entity.Property(e => e.AlertId).HasColumnName("ALERT_ID");
                entity.Property(e => e.UserId).HasColumnName("USER_ID");
                entity.Property(e => e.HealthRecordId).HasColumnName("HEALTH_RECORD_ID");
                entity.Property(e => e.SymptomRecordId).HasColumnName("SYMPTOM_RECORD_ID");
                entity.Property(e => e.TypeAlert).HasColumnName("TYPE_ALERT");
                entity.Property(e => e.Message).HasColumnName("MESSAGE");
                entity.Property(e => e.RiskLevel).HasColumnName("RISK_LEVEL");
                entity.Property(e => e.RegisteredAt).HasColumnName("REGISTERED_AT");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.HealthRecord)
                    .WithMany(e => e.Alerts)
                    .HasForeignKey(e => e.HealthRecordId);

                entity.HasOne(e => e.SymptomRecord)
                    .WithMany(e => e.Alerts)
                    .HasForeignKey(e => e.SymptomRecordId);
            });
        }
    }
}