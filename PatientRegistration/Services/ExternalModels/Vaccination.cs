using System.Text.Json.Serialization;

namespace PatientRegistration.Services.ExternalModels
{
    public class Vaccination
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("SSN")]
        public string SSN { get; set; }

        [JsonPropertyName("vaccine_name")]
        public string VaccineName { get; set; }

        [JsonPropertyName("vaccine_date")]
        public DateTime VaccineDate { get; set; }

        [JsonPropertyName("vaccine_validity")]
        public string VaccineValidity { get; set; }

        [JsonPropertyName("administered_by")]
        public string AdministeredBy { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
