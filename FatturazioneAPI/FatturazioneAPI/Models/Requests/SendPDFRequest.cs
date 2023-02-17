namespace FatturazioneAPI.Models.Requests
{
    public class SendPDFRequest
    {
        public RicevutaModel Ricevuta { get; set;}
        public ClientiModel Cliente { get; set;}
    }
}
