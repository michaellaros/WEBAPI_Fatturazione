using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models.Requests
{
    public class RicercaClienteRequest
    {
        public string? clientSurname { get; set; }
        public string? clientName { get; set; }
        public string? cf_piva { get; set; }
        public string? email { get; set; }
        public DateTime? birthDate { get; set; }
        [JsonIgnore]
        public string? birthDateString { get { return birthDate != null ? birthDate?.ToString("yyyy-MM-dd") : null; } }
        public string? clientAddress { get; set; }
        public bool IsEmpty()
        {
            return clientSurname == null && clientName == null && birthDate == null && clientAddress == null && birthDate == null;
        }
    }
}
