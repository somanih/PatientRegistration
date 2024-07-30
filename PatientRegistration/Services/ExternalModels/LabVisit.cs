using System.Text.Json.Serialization;

namespace PatientRegistration.Services.ExternalModels
{
    public class LabVisit
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("SSN")]
        public string SSN { get; set; }

        [JsonPropertyName("lab_name")]
        public string LabName { get; set; }

        [JsonPropertyName("lab_test_request")]
        public string LabTestRequest { get; set; }

        [JsonPropertyName("collection_date")]
        public DateTime CollectionDate { get; set; }

        [JsonPropertyName("result_date")]
        public DateTime ResultDate { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
