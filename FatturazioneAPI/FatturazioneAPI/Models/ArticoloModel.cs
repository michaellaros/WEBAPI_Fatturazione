namespace FatturazioneAPI.Models
{
    public class ArticoloModel
    {
    
        public string? cod_articolo { get; set; }
        public string desc_articolo { get; set; }
        public decimal prezzo { get; set; }
        public decimal quantita { get; set; }
        public bool flg_isDiscount { get; set; }
        public decimal prezzo_totale_articolo { get { return Math.Round(prezzo * quantita,2); } }
        public IVAModel? ivaArticolo { get; set; } = null;

        public ArticoloModel() { }
        public ArticoloModel(string cod_articolo,string desc_articolo, decimal prezzo, decimal quantita, bool flg_isDiscount, IVAModel ivaArticolo)
        {
            this.cod_articolo= cod_articolo;
            this.desc_articolo = desc_articolo;
            this.prezzo = prezzo;
            this.quantita = quantita;
            this.flg_isDiscount = flg_isDiscount;
            this.ivaArticolo = ivaArticolo;
        }
    
    public ArticoloModel(string desc_articolo, decimal prezzo, decimal quantita, bool flg_isDiscount)
        {
            this.desc_articolo = desc_articolo;
            this.prezzo = prezzo;
            this.quantita = quantita;
            this.flg_isDiscount = flg_isDiscount;
        }
    }
}
