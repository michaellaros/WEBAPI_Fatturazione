namespace FatturazioneAPI.Models
{
    public class IVAModel
    {
        public string ivaGroup { get; set; }
        public decimal ivaPrice { get; set; }

        public IVAModel(string ivaGroup, decimal ivaPrice)
        {
            this.ivaGroup = ivaGroup;
            this.ivaPrice = ivaPrice;
        }
    }
}
