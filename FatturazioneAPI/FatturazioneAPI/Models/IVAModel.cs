using System.Text.Json.Serialization;

namespace FatturazioneAPI.Models
{
    public class IVAModel
    {
        public string ivaGroup { get; set; }
        public decimal ivaPercent { get; set; }
        public decimal articlePrice { get; set; }
        public decimal ivaPrice { get; set; }
        public int groupId { get; set; }

        [JsonIgnore]
        public string szTaxAuthorityID { get; set; }
        [JsonIgnore]
        public string szTaxAuthorityName { get; set; }
        [JsonIgnore]
        public string szReceiptPrintCode { get; set; }
        [JsonIgnore]
        public decimal dIncludedExactTaxValue { get; set; }
        [JsonIgnore]
        public decimal dTotalSale { get; set; }
        [JsonIgnore]
        public decimal dUsedTotalSale { get; set; }

        public IVAModel(string ivaGroup, decimal ivaPercent, decimal articlePrice, decimal ivaPrice, int groupId, string szTaxAuthorityID, string szTaxAuthorityName, string szReceiptPrintCode, decimal dIncludedExactTaxValue, decimal dTotalSale, decimal dUsedTotalSale)
        {
            this.ivaGroup = ivaGroup;
            this.ivaPercent = ivaPercent;
            this.articlePrice = articlePrice;
            this.ivaPrice = ivaPrice;
            this.groupId = groupId;
            this.szTaxAuthorityID = szTaxAuthorityID;
            this.szTaxAuthorityName = szTaxAuthorityName;
            this.szReceiptPrintCode = szReceiptPrintCode;
            this.dIncludedExactTaxValue = dIncludedExactTaxValue;
            this.dTotalSale = dTotalSale;
            this.dUsedTotalSale = dUsedTotalSale;
        }
    }
}
