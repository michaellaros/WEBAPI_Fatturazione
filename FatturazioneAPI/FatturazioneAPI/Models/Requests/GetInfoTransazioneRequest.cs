namespace FatturazioneAPI.Models.Requests
{
    public class GetInfoTransazioneRequest
    {

        public string store_id { get; set; }
        public string receipt_year { get; set; }
        public string receipt_number { get; set; }
        public string date { get; set; }

    }
}
