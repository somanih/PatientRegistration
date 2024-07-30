using System.Text.Json.Serialization;

namespace PatientRegistration.Services.ExternalModels
{
    public class Medication
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("visit_id")]
        public int VisitId { get; set; }

        [JsonPropertyName("SSN")]
        public string SSN { get; set; }

        [JsonPropertyName("medicine_name")]
        public string MedicineName { get; set; }

        [JsonPropertyName("dosage")]
        public string Dosage { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("prescribed_by")]
        public string PrescribedBy { get; set; }

        [JsonPropertyName("prescription_period")]
        public string PrescriptionPeriod { get; set; }

        [JsonPropertyName("prescription_date")]
        public DateTime PrescriptionDate { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
