namespace FatturazioneAPI.Models
{
    public class RicevutaStoricoModel
    {
        public string receipt_number { get; set; }
        public string? storno_number { get; set; }
        public string receipt_year { get; set; }

        public string store_id { get; set; }
        public DateTime date { get; set; }
        public string receipt_type { get; set; }

        public RicevutaStoricoModel(string receipt_number, string storno_number, string receipt_year, string store_id, DateTime date, string receipt_type)
        {
            this.receipt_number = receipt_number;
            this.storno_number = storno_number;
            this.receipt_year = receipt_year;
            this.store_id = store_id;
            this.date = date;
            this.receipt_type = receipt_type;
        }
    }
}
