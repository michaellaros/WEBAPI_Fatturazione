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
                    totale += articolo.prezzo_totale_articolo;
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
                    bool flg_new = true;
                    foreach (IVAModel iva in ivaList.ToList())
                    {
                        if (iva.ivaGroup == articolo.ivaArticolo.ivaGroup)
                        {
                            ivaList.RemoveAll(x => x.ivaGroup == iva.ivaGroup);
                            iva.ivaPrice += articolo.ivaArticolo.ivaPrice;
                            ivaList.Add(iva);
                            flg_new = false;
                        }
                    }
                    if (flg_new)
                    {
                        ivaList.Add(articolo.ivaArticolo);
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
