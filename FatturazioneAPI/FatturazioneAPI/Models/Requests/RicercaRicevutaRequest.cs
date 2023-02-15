using System.Text.Json.Serialization;

namespace RicevutaAPI.Models.Requests
{
    public class RicercaRicevutaRequest
    {
        public string? negozio { get; set; }
        public string? cassa { get;set; }
        public string? transazione { get; set; }
        public DateTime? data { get; set; }
        [JsonIgnore]
        public string? dataString { get { return data != null ? data?.ToString("yyyyMMdd") : null; } }
        public bool IsEmpty()
        {
            return negozio == null && cassa == null && transazione == null && data == null;
        }
    }
}
