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
                foreach (ArticoloModel articolo in articoli)
                {
                    totale += articolo.prezzo_totale_articolo + articolo.totalDiscount;
                }
                return totale;
            }
        }
        public List<IVAModel> riepilogoIva
        {
            get
            {
                List<IVAModel> ivaList = new List<IVAModel>();
                foreach (ArticoloModel articolo in articoli)
                {

                    IVAModel? iva = ivaList.Find(x => x.ivaGroup == articolo.ivaArticolo!.ivaGroup);
                    if (iva != null)
                    {

                        iva.ivaPrice += articolo.ivaArticolo.ivaPrice;
                        iva.articlePrice += articolo.ivaArticolo.articlePrice;

                    }
                    else
                    {
                        IVAModel ivaArticolo = articolo.ivaArticolo;
                        ivaList.Add(new IVAModel(ivaArticolo.ivaGroup, ivaArticolo.ivaPercent, ivaArticolo.articlePrice, ivaArticolo.ivaPrice));
                    }
                }
                return ivaList;

            }
        }
        public RicevutaModel(string nome_ricevuta)
        {
            this.nome_ricevuta = nome_ricevuta;
            articoli = new List<ArticoloModel>();
        }

        public RicevutaModel()
        {
        }
    }

}
