namespace VitaOrbitApi.Models
{
    public class SymptomRecord
    {
        public int SymptomRecordId { get; private set; }
        public int UserId { get; private set; }
        public User? User { get; private set; }
        public string SymptomName { get; private set; }
        public int Intensity { get; private set; }
        public string Frequency { get; private set; }
        public string Description { get; private set; }
        public DateTime StartedAt { get; private set; }
        public string RiskClassification { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        public ICollection<Alert> Alerts { get; private set; }

        protected SymptomRecord() {

            Alerts = new List<Alert>();

        }

        public SymptomRecord(int userId, string symptomName, int intensity, string frequency, string description, DateTime startedAt)
        {
            UserId = userId;
            SymptomName = symptomName;
            Intensity = intensity;
            Frequency = frequency;
            Description = description;
            StartedAt = startedAt;
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
