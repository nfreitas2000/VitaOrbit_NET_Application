namespace VitaOrbitApi.Models
{
    public class HealthRecord
    {
        public int HealthRecordId { get; private set; }
        public int UserId { get; private set; }
        public User? User { get; private set; }
        public int HeartRate { get; private set; }
        public int SystolicPressure { get; private set; }
        public int DiastolicPressure { get; private set; }
        public decimal BodyTemperature { get; private set; }
        public int OxygenSaturation { get; private set; }
        public decimal HydrationLevel { get; private set; }
        public decimal SleepHours { get; private set; }
        public string Notes { get; private set; }
        public string RiskClassification { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        public ICollection<Alert> Alerts { get; private set; }


        protected HealthRecord() {

            Alerts = new List<Alert>();

        }

        public HealthRecord(int userId, int heartRate, int systolicPressure, int diastolicPressure, decimal bodyTemperature, int oxygenSaturation, decimal hydrationLevel, decimal sleepHours, string notes)
        {
            UserId = userId;
            HeartRate = heartRate;
            SystolicPressure = systolicPressure;
            DiastolicPressure = diastolicPressure;
            BodyTemperature = bodyTemperature;
            OxygenSaturation = oxygenSaturation;
            HydrationLevel = hydrationLevel;
            SleepHours = sleepHours;
            Notes = notes;
            RiskClassification = "Baixo";
            RegisteredAt = DateTime.UtcNow;
            Alerts = new List<Alert>();

        }

        public void UpdateRiskClassification(string riskClassification)
        {
            RiskClassification = riskClassification;
        }

    }
}
