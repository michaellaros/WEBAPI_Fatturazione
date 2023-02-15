namespace RicevutaAPI.Models
{
    public class ArticoloModel
    {
    
        public string desc_articolo { get; set; }
        public decimal prezzo { get; set; }
        public int quantita { get; set; }
        public bool flg_isDiscount { get; set; }
        public decimal prezzo_totale_articolo { get { return prezzo * quantita; } }
        public IVAModel ivaArticolo { get; set; }


        public ArticoloModel(string desc_articolo, decimal prezzo, int quantita, bool flg_isDiscount, IVAModel ivaArticolo)
        {
            this.desc_articolo = desc_articolo;
            this.prezzo = prezzo;
            this.quantita = quantita;
            this.flg_isDiscount = flg_isDiscount;
            this.ivaArticolo = ivaArticolo;
        }
    
    public ArticoloModel(string desc_articolo, decimal prezzo, int quantita, bool flg_isDiscount)
        {
            this.desc_articolo = desc_articolo;
            this.prezzo = prezzo;
            this.quantita = quantita;
            this.flg_isDiscount = flg_isDiscount;
        }
    }
}
