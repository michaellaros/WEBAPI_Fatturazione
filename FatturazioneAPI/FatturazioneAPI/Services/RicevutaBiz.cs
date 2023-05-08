using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using System.Xml;

namespace FatturazioneAPI.Services
{
    public class RicevutaBiz
    {
        private readonly IConfiguration configuration;
        private string pathCartella { get { return configuration.GetSection("appsettings").GetValue<string>("folderstring"); } }
        public RicevutaBiz(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public RicevutaModel GetRicevuta(string fileName)
        {
            return RicevutaCostruction(GetRicevutaFromNomeFile(fileName));
        }

        public RicevutaModel RicevutaCostruction(string fileName)
        {

            try
            {
                RicevutaModel ricevuta = new RicevutaModel(fileName.Substring(fileName.LastIndexOf("\\") + 1).Replace(".xml", ""));
                if (File.Exists(fileName))
                {
                    XmlTextReader reader = new XmlTextReader(fileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(fileName);

                    ricevuta.szType = xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szTaType").InnerXml;
                    ricevuta.szWorkstationID = xml.SelectSingleNode("TAS/NEW_TA/HEADER/szWorkstationID")!.InnerXml;
                    ricevuta.lOperatorID = int.Parse(xml.SelectSingleNode("TAS/NEW_TA/HEADER/lOperatorID")!.InnerXml);
                    ricevuta.lFPtrReceiptNmbr = int.Parse(xml.SelectSingleNode("TAS/NEW_TA/FOOTER/lFPtrReceiptNmbr")!.InnerXml);
                    //verifico che sia di tipo sale
                    if (ricevuta.szType == "SA" || ricevuta.szType == "FI")
                    {

                        decimal totale = 0;


                        //ciclo articoli comprati
                        foreach (XmlNode node in xml.SelectNodes("TAS/NEW_TA/ART_SALE"))
                        {
                            XmlNode prezzoNode = node.SelectSingleNode("dTaTotal");
                            if (prezzoNode != null)
                            {
                                string codArticolo = node.SelectSingleNode("ARTICLE/szItemID").InnerXml;
                                string szPOSItemID = node.SelectSingleNode("ARTICLE/szPOSItemID").InnerXml;
                                string szItemLookupCode = node.SelectSingleNode("szItemLookupCode").InnerXml;
                                string szItemTaxGroupID = node.SelectSingleNode("ARTICLE/szItemTaxGroupID").InnerXml;
                                string nameArticolo = node.SelectSingleNode("ARTICLE/szDesc").InnerXml;
                                int lTaSeqNmbr = int.Parse(node.SelectSingleNode("Hdr/lTaSeqNmbr").InnerXml);
                                XmlNode discountNode = node.SelectSingleNode("dTaTotalDiscounted");
                                decimal discount = discountNode != null ? Math.Round(decimal.Parse(discountNode.InnerXml), 2) : 0; //get discount
                                decimal quantita = Math.Round(decimal.Parse(node.SelectSingleNode("dTaQty").InnerXml), 2);
                                decimal prezzo = Math.Round(decimal.Parse(prezzoNode.InnerXml), 2);
                                decimal dTaPrice = Math.Round(decimal.Parse(node.SelectSingleNode("dTaPrice").InnerXml), 2);

                                XmlNode ivaNode = GetIvaFromArticleId(xml, node.SelectSingleNode("Hdr/lTaCreateNmbr").InnerXml);
                                string ivaGroup = ivaNode.SelectSingleNode("TAX/szTaxGroupRuleName").InnerXml;

                                decimal ivaValue = ivaGroup == "Tax free" ? 0 : ivaNode.SelectSingleNode("dIncludedTaxValue") == null ? 0 : decimal.Parse(ivaNode.SelectSingleNode("dIncludedTaxValue").InnerXml);

                                decimal ivaPercent = ivaGroup == "Tax free" ? 0 : decimal.Parse(ivaNode.SelectSingleNode("TAX/dPercent").InnerXml);
                                int ivaGroupId = ivaGroup == "Tax free" ? -1 : int.Parse(ivaNode.SelectSingleNode("TAX/szTaxGroupID").InnerXml);

                                #region For Receipt

                                string szTaxAuthorityID = ivaNode.SelectSingleNode("TAX/szTaxAuthorityID").InnerXml;
                                string szTaxAuthorityName = ivaNode.SelectSingleNode("TAX/szTaxAuthorityName").InnerXml;
                                //string szReceiptPrintCode = ivaNode.SelectSingleNode("TAX/szReceiptPrintCode").InnerXml;
                                decimal dIncludedExactTaxValue = ivaGroup == "Tax free" ? 0 : ivaNode.SelectSingleNode("dIncludedExactTaxValue") == null ? 0 : decimal.Parse(ivaNode.SelectSingleNode("dIncludedExactTaxValue").InnerXml);
                                decimal dTotalSale = decimal.Parse(ivaNode.SelectSingleNode("dTotalSale").InnerXml);
                                decimal dUsedTotalSale = decimal.Parse(ivaNode.SelectSingleNode("dUsedTotalSale").InnerXml);



                                #endregion

                                IVAModel iva = new IVAModel(ivaGroup, ivaPercent, prezzo + discount - ivaValue, ivaValue, ivaGroupId, szTaxAuthorityID, szTaxAuthorityName, dIncludedExactTaxValue, dTotalSale, dUsedTotalSale);



                                ricevuta.articoli.Add(new ArticoloModel(codArticolo, nameArticolo, prezzo, discount, quantita, iva, lTaSeqNmbr, dTaPrice, szPOSItemID, szItemLookupCode, szItemTaxGroupID));

                            }
                        }

                        return ricevuta;


                    }
                    else
                    {
                        Console.WriteLine("{0} is not a valid file or directory.", fileName);
                    }

                }
                return null;
            }

            catch (Exception e)
            {
                throw e;
            }



        }


        public string GetRicevutaFromNomeFile(string? nomeFile, string? pathIn = null)
        {
            string path = string.IsNullOrEmpty(pathIn) ? pathCartella : pathIn;
            string[] fileNames = Directory.GetFiles(path);

            foreach (string pathElemento in fileNames)
            {
                if (pathElemento.Contains(nomeFile))
                {
                    return pathElemento;
                }
            }
            string[] cartelle = Directory.GetDirectories(path);
            foreach (string pathElemento in cartelle)
            {
                if (Directory.Exists(pathElemento))
                {
                    // This path is a directory
                    string pathTrovato = GetRicevutaFromNomeFile(nomeFile, pathElemento);
                    if (!string.IsNullOrEmpty(pathTrovato))
                    {
                        return pathTrovato;
                    }
                }

                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", pathElemento);
                }
            }
            return null;
        }


        public List<RicevutaSelectModel> RicercaRicevuta(RicercaRicevutaRequest request)
        {
            List<RicevutaSelectModel> result = new List<RicevutaSelectModel>();
            try
            {
                List<string> possibleFolders = new List<string>();
                if (request.data != null)
                {
                    if (Directory.Exists($@"{pathCartella}\{request.dataString}"))
                    {
                        possibleFolders.Add($@"{pathCartella}\{request.dataString}");
                    }
                    else
                    {
                        return result;
                    }
                }
                else
                {
                    possibleFolders.AddRange(Directory.GetDirectories(pathCartella));
                }
                foreach (string folder in possibleFolders)
                {
                    foreach (string nomeFile in Directory.GetFiles(folder))
                    {
                        RicevutaSelectModel model = CheckRicevutaMatchFilter(nomeFile, request);
                        if (model != null) { result.Add(model); }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }


        public RicevutaSelectModel CheckRicevutaMatchFilter(string fileName, RicercaRicevutaRequest request)
        {
            string[] fileNameSplit = fileName.Substring(fileName.LastIndexOf(@"\") + 1).Split('_');
            if (!string.IsNullOrEmpty(request.negozio) && fileNameSplit[0] != request.negozio)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.cassa) && fileNameSplit[1] != request.cassa)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.transazione) && fileNameSplit[2] != request.transazione)
            {
                return null;
            }

            if (request.data != null && fileNameSplit[3].Substring(0, 8) != request.dataString.Substring(0, 8))
            {
                return null;
            }

            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            if (xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szTaType").InnerXml != "SA" && xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szTaType").InnerXml != "FI")
            {
                return null;
            }
            XmlNode totalSaleNode = xml.SelectSingleNode("TAS/NEW_TA/TOTAL/dTotalSale");
            if (totalSaleNode == null) { return null; }
            decimal prezzoTotale = decimal.Parse(totalSaleNode.InnerXml);
            return new RicevutaSelectModel()
            {
                nomeFile = fileName,
                prezzoTotale = prezzoTotale,
            };
        }

        public XmlNode GetIvaFromArticleId(XmlDocument xml, string articleId)
        {
            foreach (XmlNode iva in xml.SelectNodes("TAS/NEW_TA/TAX_ART"))
            {
                if (iva.SelectSingleNode("Hdr/lTaRefToCreateNmbr").InnerXml == articleId)
                {
                    return iva;
                }
            }
            return null;
        }





    }
}
