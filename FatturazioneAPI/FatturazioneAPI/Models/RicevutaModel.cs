namespace FatturazioneAPI.Models
{
    public class RicevutaModel
    {
        public string nome_ricevuta { get; set; }
        public List<ArticoloModel> articoli { get; set; }
        public decimal prezzo_totale 
        { 
            get 
            {
                decimal totale = 0;
                foreach(ArticoloModel articolo in articoli)
                {
                    totale += articolo.prezzo_totale_articolo;
                }
                return totale;
            } 
        }
        public int numero_articoli { get { return articoli.FindAll(x => x.flg_isDiscount == false).Count(); } }
        public int numero_sconti { get { return articoli.FindAll(x => x.flg_isDiscount == true).Count(); } }
        public RicevutaModel(string nome_ricevuta) 
        { 
            this.nome_ricevuta= nome_ricevuta;
            articoli= new List<ArticoloModel>();
        }
    }

}
