using RicevutaAPI.Models;
using RicevutaAPI.Models.Requests;
using System.Text;
using System.Xml;

namespace RicevutaAPI.Services
{
    public class RicevutaBiz
    {
        private string pathCartella { get { return @"C:\Temp\Normal"; } }
        public RicevutaModel RicevutaCostruction(string fileName)
        {

            try
            {
                RicevutaModel recipe = new RicevutaModel(fileName);
                if (File.Exists(fileName))
                {
                    XmlTextReader reader = new XmlTextReader(fileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(fileName);
                    //verifico che sia di tipo sale
                    if (xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szTaType").InnerXml == "SA")
                    {
                        decimal totale = 0;


                        //ciclo articoli comprati
                        foreach (XmlNode node in xml.SelectNodes("TAS/NEW_TA/ART_SALE"))
                        {
                            XmlNode prezzoNode = node.SelectSingleNode("dTaPrice");
                            if (prezzoNode != null)
                            {
                                string nameArticolo = node.SelectSingleNode("ARTICLE/szDesc").InnerXml;
                                decimal ivaValue = decimal.Parse(node.SelectSingleNode("TAX_ART/dPercent").InnerXml);
                                string ivaGroup = node.SelectSingleNode("TAX_ART/TAX/szTaxGroupRuleName").InnerXml;
                                IVAModel iva = new IVAModel(ivaGroup, ivaValue);
                                int quantita = int.Parse(node.SelectSingleNode("dTaQty").InnerXml);
                                decimal prezzo = Math.Round(decimal.Parse(prezzoNode.InnerXml.Replace(".", ",")), 2);
                                string discount = node.SelectSingleNode("szDiscDesc").InnerXml;
                                ArticoloModel articolo1 = new ArticoloModel(nameArticolo, prezzo, quantita, false, iva);
                                recipe.articoli.Add(articolo1);

                            }
                        }
                        return recipe;


                    }
                    else
                    {
                        Console.WriteLine("{0} is not a valid file or directory.", fileName);
                    }

                }
                return null;
            }

            catch
            {
                return null;
            }



        }
        public string? GetRicevuta(string id, string? pathIn = null)
        {
            string path = string.IsNullOrEmpty(pathIn) ? pathCartella : pathIn;
            string[] fileNames = Directory.GetFiles(path);

            foreach (string pathElemento in fileNames)
            {
                if (File.Exists(pathElemento))
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(pathElemento);
                    string idFile = xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szUniqueID").InnerXml;
                    if (idFile == id)
                        return pathElemento;
                }

                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", pathElemento);
                }
            }
            string[] cartelle = Directory.GetDirectories(path);
            foreach (string pathElemento in cartelle)
            {
                if (Directory.Exists(pathElemento))
                {
                    // This path is a directory
                    string? pathTrovato = GetRicevuta(id, pathElemento);
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

        public string GetRicevutaFromNomeFile(string? nomeFile, string? pathIn = null)
        {
            string path = string.IsNullOrEmpty(pathIn) ? pathCartella : pathIn;
            string[] fileNames = Directory.GetFiles(path);

            foreach (string pathElemento in fileNames)
            {
                if (pathElemento.Contains(nomeFile))
                    return pathElemento;
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
        public static void stampaPrintBuffer(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    XmlTextReader reader = new XmlTextReader(path);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(path);
                    string printBuffer = xml.SelectSingleNode("TAS/PrintBuffer").InnerXml;
                    byte[] converted = Convert.FromBase64String(printBuffer);
                    Console.WriteLine(Encoding.UTF8.GetString(converted));
                }

                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} is not a valid file or directory with error {1}.", path, e.Message);
            }


        }
        // errore in questo file "C:\\TPDotnet\\Pos\\Transactions\\Normal\\20210315\\18_1_1816_20210315165011.xml"
        public string StampaCheckTotale(string path)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                if (File.Exists(path))
                {
                    XmlTextReader reader = new XmlTextReader(path);
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    XmlDocument xml = new XmlDocument();
                    xml.Load(path);
                    //verifico che sia di tipo sale
                    if (xml.SelectSingleNode("TAS/NEW_TA/TA_CONTROL/szTaType").InnerXml == "SA")
                    {
                        decimal totale = 0;
                        string[] pathSplit = path.Split("\\");

                        result.Append(string.Format("=====================================\nfile {0} nella cartella {1}\n", pathSplit[pathSplit.Length - 1], pathSplit[pathSplit.Length - 2]));

                        //ciclo articoli comprati
                        foreach (XmlNode node in xml.SelectNodes("TAS/NEW_TA/ART_SALE"))
                        {
                            XmlNode prezzoNode = node.SelectSingleNode("dTaPrice");
                            if (prezzoNode != null)
                            {
                                string articolo = node.SelectSingleNode("ARTICLE/szDesc").InnerXml;
                                int quantita = int.Parse(node.SelectSingleNode("dTaQty").InnerXml);
                                decimal prezzo = Math.Round(decimal.Parse(prezzoNode.InnerXml.Replace(".", ",")), 2);
                                decimal totArticolo = prezzo * quantita;
                                result.Append(string.Format("{0}   {1}   {2}   {3}\n", articolo, quantita, prezzo, totArticolo));
                                totale += totArticolo;
                            }
                        }

                        //cicle discounts
                        foreach (XmlNode node in xml.SelectNodes("TAS/NEW_TA/DISC_INFO"))
                        {
                            XmlNode prezzoNode = node.SelectSingleNode("dDiscValue");
                            if (prezzoNode != null)
                            {
                                string discount = node.SelectSingleNode("szDiscDesc").InnerXml;
                                int quantita = int.Parse(node.SelectSingleNode("dDiscQty") != null ? node.SelectSingleNode("dDiscQty").InnerXml : "1");
                                decimal prezzo = -decimal.Parse(prezzoNode.InnerXml.Replace(".", ","));
                                decimal totDiscount = prezzo * quantita;
                                result.Append(string.Format("{0}   {1}   {2}   {3}\n", discount, quantita, prezzo, totDiscount));
                                totale += totDiscount;
                            }

                        }
                        totale = Math.Round(totale, 2);
                        result.Append(string.Format("Totale calcolato: {0}\n", totale));
                        XmlNode totaleNode = xml.SelectSingleNode("TAS/NEW_TA/TOTAL/dTotalSale");
                        if (totaleNode != null)
                        {
                            result.Append(string.Format("Totale scontrino: {0}\n", decimal.Parse(totaleNode.InnerXml.Replace(".", ","))));
                            if (decimal.Parse(totaleNode.InnerXml.Replace(".", ",")) == totale)
                                result.Append("Totale calcolato uguale a totale su scontrino");
                            else
                                result.Append("Totale calcolato diverso a totale su scontrino");
                        }


                    }

                }

                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }
                return result.ToString();
            }
            catch (Exception e)
            {
                return string.Format("{0} is not a valid file or directory with error {1}.", path, e.Message);
            }



        }

        public void CicleCartella(string path)
        {
            string[] cartelle = Directory.GetDirectories(path);
            foreach (string pathElemento in cartelle)
            {

                if (pathElemento.StartsWith("-"))
                {
                    Console.WriteLine(pathElemento);
                }
                else if (Directory.Exists(pathElemento))
                {
                    // This path is a directory
                    CicleCartella(pathElemento);
                }
                else if (File.Exists(pathElemento))
                    StampaCheckTotale(pathElemento);
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", pathElemento);
                }
            }


            string[] file = Directory.GetFiles(path);
            foreach (string pathElemento in file)
            {

                if (Directory.Exists(pathElemento))
                {
                    // This path is a directory
                    CicleCartella(pathElemento);
                }
                else if (File.Exists(pathElemento))
                    StampaCheckTotale(pathElemento);
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", pathElemento);
                }
            }
        }

        public List<string> RicercaRicevuta(RicercaRicevutaRequest request)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> possibleFolders= new List<string>();
                if(request.data != null)
                {
                    if (Directory.Exists($"{pathCartella}\\{request.dataString}"))
                    {
                        possibleFolders.Add($"{pathCartella}\\{request.dataString}");
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
                    result.AddRange(Directory.GetFiles(folder)
                        .Where(x =>
                        CheckRicevutaMatchFilter(x.Substring(x.LastIndexOf(@"\")+1), request)));
                }
            }catch(Exception e)
            {
                return null;
            }
            return result;
        }

        public bool CheckRicevutaMatchFilter(string fileName, RicercaRicevutaRequest request)
        {
            string[] fileNameSplit = fileName.Split('_');
            if(!string.IsNullOrEmpty( request.negozio) && fileNameSplit[0] != request.negozio)
                return false;
            if(!string.IsNullOrEmpty(request.cassa) && fileNameSplit[1] != request.cassa)
                return false;
            if(!string.IsNullOrEmpty(request.transazione) && fileNameSplit[2] != request.transazione)
                return false;
            if(request.data != null && fileNameSplit[3] != request.dataString.Substring(0,8))
                return false;
            return true;
        }
    }
}
