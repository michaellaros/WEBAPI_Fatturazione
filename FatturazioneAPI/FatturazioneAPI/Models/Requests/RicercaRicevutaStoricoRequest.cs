namespace FatturazioneAPI.Models.Requests
{
    public class RicercaRicevutaStoricoRequest
    {
        public string? receipt_number { get; set; }
        public string? date_start { get; set; }
        public string? date_end { get; set; }
    }
}
