namespace FatturazioneAPI.Models
{
    public class ArticoloModel
    {

        public string? cod_articolo { get; set; }
        public string desc_articolo { get; set; }
        public decimal prezzo { get { return Math.Round(prezzo_totale_articolo / quantita, 2); } } //we calculate the article price to be sure that it matches the total price
        public decimal quantita { get; set; }
        public decimal discount { get { return Math.Round(totalDiscount / quantita, 2); } } //we calculate the article discount to be sure that it matches the total discount
        public decimal totalDiscount { get; set; }
        public decimal prezzo_totale_articolo { get; set; }
        public IVAModel? ivaArticolo { get; set; } = null;

        public ArticoloModel() { }
        public ArticoloModel(string cod_articolo, string desc_articolo, decimal prezzo_totale_articolo, decimal totalDiscount, decimal quantita, IVAModel ivaArticolo)
        {
            this.cod_articolo = cod_articolo;
            this.desc_articolo = desc_articolo;
            this.prezzo_totale_articolo = prezzo_totale_articolo;
            this.totalDiscount = totalDiscount;
            this.quantita = quantita;
            this.ivaArticolo = ivaArticolo;
        }

        //public ArticoloModel(string desc_articolo, decimal prezzo, decimal quantita, bool flg_isDiscount)
        //{
        //    this.desc_articolo = desc_articolo;
        //    this.prezzo = prezzo;
        //    this.quantita = quantita;
        //    this.flg_isDiscount = flg_isDiscount;
        //}
    }
}
