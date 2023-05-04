namespace FatturazioneAPI.Models.Responses
{
    public class GetInfoTransazioneResponse
    {
        public string store_id { get; set; }
        public string workstation_id { get; set; }
        public string ta { get; set; }
        public string date { get; set; }
        public int client_id { get; set; }
        public GetInfoTransazioneResponse(string store_id, string workstation_id, string ta, string date, int client_id)
        {
            this.store_id = store_id;
            this.workstation_id = workstation_id;
            this.ta = ta;
            this.date = date;
            this.client_id = client_id;
        }
    }
}
