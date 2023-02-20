using System.Text.Json;

namespace FatturazioneAPI.Models.Requests
{
    public class SendPDFRequest
    {
        public RicevutaModel Ricevuta { get; set;}
        public ClientiModel Cliente { get; set;}

        private SendPDFRequest? DeserializeUsingGenericSystemTextJson(string json)
        {
            var request = JsonSerializer.Deserialize<SendPDFRequest>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return request;
        }
    }
}
