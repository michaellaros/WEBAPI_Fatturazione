namespace FatturazioneAPI.Models
{
    public class IVAModel
    {
        public string ivaGroup { get; set; }
        public decimal ivaPercent { get; set; }
        public decimal articlePrice { get; set; }

        public decimal ivaPrice { get; set; }

        public IVAModel(string ivaGroup, decimal ivaPercent, decimal articlePrice, decimal ivaPrice)
        {
            this.ivaGroup = ivaGroup;
            this.ivaPercent = ivaPercent;
            this.articlePrice = articlePrice;
            this.ivaPrice = ivaPrice;
        }
    }
}
