using PdfSharp.Drawing;
using PdfSharp.Pdf;
using FatturazioneAPI.Models.Requests;
using PdfSharp.Drawing.Layout;
using FatturazioneAPI.Models;
using PdfSharp.Pdf.IO;

namespace FatturazioneAPI.Services
{
    public class PDFBiz
    {
        private readonly IConfiguration configuration;
        private readonly RicevutaBiz _ricevuta;
        private readonly DataBase _dataBase;
        private string baseFolderPDF { get { return configuration.GetSection("directories").GetValue<string>("folderPDF"); } }

        public PDFBiz(IConfiguration configuration, RicevutaBiz ricevuta, DataBase dataBase)
        {
            this.configuration = configuration;
            this._ricevuta = ricevuta;
            this._dataBase = dataBase;
        }

        public string GeneraPDFFromRicevuta(SendPDFRequest request)
        {

            RicevutaModel receipt = _ricevuta.GetRicevuta(request.receiptName);
            ClientiModel client = _dataBase.GetCliente(request.client_id);
            int shopNumber = int.Parse(receipt.nome_ricevuta.Split("_")[0]);
            string receiptNumber = _dataBase.GetReceiptNumber(shopNumber).ToString("D8");
            #region config

            //abilito lettura caratteri speciali
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // Create a font
            XFont font = new XFont("Vezus Serif Regular", 7, XFontStyle.Regular);
            int nArticoliPagina = 25;

            #endregion

            #region creazione documento
            // Open template
            PdfPage template = PdfReader.Open(@"C:\Temp\template\modello-fattura.pdf", PdfDocumentOpenMode.Import).Pages[0];

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Fattura";


            //creo pagine

            int nPagine = receipt.articoli.Count % nArticoliPagina == 0 ? receipt.articoli.Count / nArticoliPagina : (receipt.articoli.Count / nArticoliPagina) + 1;

            for (int i = 0; i < nPagine; i++)
            {
                document.AddPage(template);
            }

            #endregion

            #region Inserimento intestazione

            // creo textFormatter
            XGraphics gfx = XGraphics.FromPdfPage(document.Pages[0]);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString(client.business_name, font, XBrushes.Black, new XRect(329, 165, 250, 15));
            tf.DrawString($"{client.surname} {client.name}", font, XBrushes.Black, new XRect(329, 180, 250, 15));
            tf.DrawString(client.address, font, XBrushes.Black, new XRect(329, 195, 250, 15));

            tf.DrawString(client.zipcode, font, XBrushes.Black, new XRect(329, 210, 50, 15));
            tf.DrawString(client.city, font, XBrushes.Black, new XRect(379, 210, 150, 15));
            tf.DrawString(client.district_code, font, XBrushes.Black, new XRect(529, 210, 50, 15));

            tf.DrawString(client.country_code, font, XBrushes.Black, new XRect(329, 225, 250, 15));
            if (!string.IsNullOrEmpty(client.piva))
            {
                tf.DrawString(client.piva, font, XBrushes.Black, new XRect(111.6, 260, 100, 20)); //partita iva
            }
            else if (!string.IsNullOrEmpty(client.cf))
            {
                tf.DrawString(client.cf, font, XBrushes.Black, new XRect(224.5, 260, 100, 20)); //cf
            }
            else if (!string.IsNullOrEmpty(client.passport_number))
            {
                tf.DrawString(client.passport_number, font, XBrushes.Black, new XRect(111.6, 260, 100, 20)); //passport number in box partita iva
            }
            tf.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), font, XBrushes.Black, new XRect(437.5, 260, 100, 20));



            tf.DrawString($"{receiptNumber}/{shopNumber}", font, XBrushes.Black, new XRect(26, 260, 100, 20));

            gfx.Dispose();
            #endregion

            #region inserimento articoli

            for (int i = 0; i < nPagine; i++)
            {
                XGraphics gfxArticoli = XGraphics.FromPdfPage(document.Pages[i]);
                XTextFormatter tfArticoli = new XTextFormatter(gfxArticoli);
                int hArticle = 300;
                int articleDistanceY = 14;

                tfArticoli.DrawString((i + 1).ToString(), font, XBrushes.Black, new XRect(526, 260, 100, 20)); //page number
                if (i == 0) //print receipt number on first page
                {
                    XFont receiptNumberFont = new XFont("Vezus Serif Regular", 7, XFontStyle.Bold);
                    string[] ricevutaNameSplit = receipt.nome_ricevuta.Split("_");
                    tfArticoli.DrawString($"Scontrino fiscale {ricevutaNameSplit[2]} emesso dalla cassa {ricevutaNameSplit[1]}", receiptNumberFont, XBrushes.Black, new XRect(31.5, hArticle, 500, 14));
                    hArticle += articleDistanceY;
                }

                XRect codice = new XRect(31.5, hArticle, 100, articleDistanceY);
                XRect descrizione = new XRect(102, hArticle, 255, articleDistanceY);
                XRect quantita = new XRect(274, hArticle, 60, articleDistanceY);
                XRect unitPrice = new XRect(344, hArticle, 70, articleDistanceY);
                XRect totalDiscount = new XRect(416, hArticle, 55, articleDistanceY);
                XRect iva = new XRect(470, hArticle, 60, articleDistanceY);
                XRect codIva = new XRect(531, hArticle, 40, articleDistanceY);

                XVector vettArticoli = new XVector(0, articleDistanceY);

                for (int j = i * nArticoliPagina; j < (i + 1) * (nArticoliPagina - (i == 0 ? 1 : 0)) && j < receipt.articoli.Count; j++)
                {
                    ArticoloModel article = receipt.articoli[j];
                    tfArticoli.DrawString(article.cod_articolo != null ? receipt.articoli[j].cod_articolo : "-", font, XBrushes.Black, codice);
                    tfArticoli.DrawString(article.desc_articolo, font, XBrushes.Black, descrizione);

                    tfArticoli.Alignment = XParagraphAlignment.Right;

                    tfArticoli.DrawString(article.quantita.ToString("0.000"), font, XBrushes.Black, quantita);
                    tfArticoli.DrawString(article.prezzo.ToString("0.00"), font, XBrushes.Black, unitPrice);
                    tfArticoli.DrawString(article.totalDiscount.ToString("0.00"), font, XBrushes.Black, totalDiscount);
                    tfArticoli.DrawString((article.prezzo_totale_articolo + article.totalDiscount).ToString("0.00"), font, XBrushes.Black, iva);
                    tfArticoli.DrawString(article.ivaArticolo.groupId.ToString(), font, XBrushes.Black, codIva);

                    tfArticoli.Alignment = XParagraphAlignment.Left;

                    codice.Offset(vettArticoli);
                    descrizione.Offset(vettArticoli);
                    quantita.Offset(vettArticoli);
                    unitPrice.Offset(vettArticoli);
                    totalDiscount.Offset(vettArticoli);
                    iva.Offset(vettArticoli);
                    codIva.Offset(vettArticoli);
                }
                gfxArticoli.Dispose();
            }

            #endregion

            #region inserimento riepilogo iva e totale
            XGraphics gfxIva = XGraphics.FromPdfPage(document.Pages[document.PageCount - 1]);


            XTextFormatter tfIva = new XTextFormatter(gfxIva);

            XRect index = new XRect(31.5, 685, 50, 20);
            XRect percent = new XRect(90, 685, 60.5, 20);
            XRect articlePrice = new XRect(150, 685, 75, 20);
            XRect price = new XRect(225, 685, 74, 20);
            XRect group = new XRect(316, 685, 250, 20);

            XVector vettIva = new XVector(0, 14);

            //foreach (IVAModel iva in receipt.riepilogoIva)
            for (int i = 0; i < receipt.riepilogoIva.Count; i++)
            {
                IVAModel iva = receipt.riepilogoIva[i];

                tfIva.DrawString(iva.groupId.ToString(), font, XBrushes.Black, index);

                tfIva.Alignment = XParagraphAlignment.Right;

                tfIva.DrawString(iva.ivaPercent.ToString("0.000"), font, XBrushes.Black, percent);
                tfIva.DrawString(iva.articlePrice.ToString("0.00"), font, XBrushes.Black, articlePrice);
                tfIva.DrawString(iva.ivaPrice.ToString("0.00"), font, XBrushes.Black, price);
                tfIva.Alignment = XParagraphAlignment.Left;

                tfIva.DrawString(iva.ivaGroup, font, XBrushes.Black, group);

                index.Offset(vettIva);
                percent.Offset(vettIva);
                articlePrice.Offset(vettIva);
                price.Offset(vettIva);
                group.Offset(vettIva);
            }

            XFont fontTotal = new XFont("Vezus Serif Regular", 10, XFontStyle.Regular);

            decimal articleTotal = 0, ivaTotal = 0;
            receipt.riepilogoIva.ForEach(iva => { articleTotal += iva.articlePrice; ivaTotal += iva.ivaPrice; });

            tfIva.Alignment = XParagraphAlignment.Right;
            tfIva.DrawString(articleTotal.ToString("0.00"), fontTotal, XBrushes.Black, new XRect(520, 767, 50, 20));
            tfIva.DrawString(ivaTotal.ToString("0.00"), fontTotal, XBrushes.Black, new XRect(520, 779, 50, 20));
            tfIva.DrawString(receipt.prezzo_totale.ToString("0.00"), fontTotal, XBrushes.Black, new XRect(520, 791, 50, 20));



            gfx.Dispose();
            #endregion

            _dataBase.InsertFattura(receipt, client.id.Value, receiptNumber);

            // Save the document...
            string folderPDF = baseFolderPDF + DateTime.Now.ToString("yyyyMMdd") + @"\";
            if (!Directory.Exists(folderPDF))
            {
                Directory.CreateDirectory(folderPDF);
            }
            string filename = $"{folderPDF}{receipt.nome_ricevuta}_{client.business_name}";
            int nameUsed = 0;
            while (File.Exists(filename + (nameUsed == 0 ? "" : $"_{nameUsed}") + ".pdf"))
            {
                nameUsed++;
            }
            document.Save(filename + (nameUsed == 0 ? "" : $"_{nameUsed}") + ".pdf");

            return filename + (nameUsed == 0 ? "" : $"_{nameUsed}") + ".pdf";
        }


    }
}
