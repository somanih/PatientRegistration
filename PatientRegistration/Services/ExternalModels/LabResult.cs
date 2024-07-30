using System.Text.Json.Serialization;

namespace PatientRegistration.Services.ExternalModels
{
    public class LabResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("lab_visit_id")]
        public string LabVisitId { get; set; }

        [JsonPropertyName("test_name")]
        public string TestName { get; set; }

        [JsonPropertyName("test_result")]
        public string TestResult { get; set; }

        [JsonPropertyName("test_observation")]
        public string TestObservation { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("attachments")]
        public List<object> Attachments { get; set; }
    }
}
