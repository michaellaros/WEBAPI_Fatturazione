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
        private string baseFolderPDF { get { return configuration.GetSection("directories").GetValue<string>("folderPDF"); } }

        public PDFBiz(IConfiguration configuration)
        {
            this.configuration = configuration;

        }

        public string GeneraPDFFromRicevuta(SendPDFRequest request)
        {
            #region config

            //abilito lettura caratteri speciali
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Create a font
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            int nArticoliPagina = 15;

            #endregion

            #region creazione documento
            // Open template
            PdfPage template = PdfReader.Open(@"C:\Temp\template\modello-fattura.pdf", PdfDocumentOpenMode.Import).Pages[0];

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Fattura";


            //creo pagine

            int nPagine = request.Ricevuta.articoli.Count % nArticoliPagina == 0 ? request.Ricevuta.articoli.Count / nArticoliPagina : (request.Ricevuta.articoli.Count / nArticoliPagina) + 1;

            for (int i = 0; i < nPagine; i++)
            {
                document.AddPage(template);
            }

            #endregion

            #region Inserimento intestazione

            // creo textFormatter
            XGraphics gfx = XGraphics.FromPdfPage(document.Pages[0]);
            XTextFormatter tf = new XTextFormatter(gfx);
            tf.DrawString(request.Cliente.surname, font, XBrushes.Black, new XRect(300, 150, 100, 20));
            tf.DrawString(request.Cliente.name, font, XBrushes.Black, new XRect(400, 150, 100, 20));
            tf.DrawString(request.Cliente.address, font, XBrushes.Black, new XRect(300, 180, 100, 20));
            tf.DrawString(request.Cliente.birthday, font, XBrushes.Black, new XRect(400, 180, 100, 20));
            gfx.Dispose();
            #endregion

            #region inserimento articoli

            for (int i = 0; i < nPagine; i++)
            {
                XGraphics gfxArticoli = XGraphics.FromPdfPage(document.Pages[i]);
                XTextFormatter tfArticoli = new XTextFormatter(gfxArticoli);

                XRect codice = new XRect(20, 275, 100, 20);
                XRect descrizione = new XRect(120, 275, 255, 20);
                XRect quantita = new XRect(375, 275, 100, 20);
                XRect prezzoTot = new XRect(475, 275, 100, 20);

                XVector vettArticoli = new XVector(0, 25);

                for (int j = i * nArticoliPagina; j < (i + 1) * nArticoliPagina && j < request.Ricevuta.articoli.Count; j++)
                {
                    tfArticoli.DrawString(request.Ricevuta.articoli[j].cod_articolo != null ? request.Ricevuta.articoli[j].cod_articolo : "-", font, XBrushes.Black, codice);
                    tfArticoli.DrawString(request.Ricevuta.articoli[j].desc_articolo, font, XBrushes.Black, descrizione);
                    tfArticoli.DrawString(request.Ricevuta.articoli[j].quantita.ToString(), font, XBrushes.Black, quantita);
                    tfArticoli.DrawString(request.Ricevuta.articoli[j].prezzo_totale_articolo.ToString(), font, XBrushes.Black, prezzoTot);

                    codice.Offset(vettArticoli);
                    descrizione.Offset(vettArticoli);
                    quantita.Offset(vettArticoli);
                    prezzoTot.Offset(vettArticoli);
                }
                gfxArticoli.Dispose();
            }

            #endregion

            #region inserimento riepilogo iva
            XGraphics gfxIva = XGraphics.FromPdfPage(document.Pages[document.PageCount - 1]);


            XTextFormatter tfIva = new XTextFormatter(gfxIva);

            XRect group = new XRect(20, 650, 100, 20);
            XRect price = new XRect(120, 650, 255, 20);

            XVector vettIva = new XVector(0, 20);

            foreach (IVAModel iva in request.Ricevuta.riepilogoIva)
            {
                tfIva.DrawString(iva.ivaGroup, font, XBrushes.Black, group);
                tfIva.DrawString(iva.ivaPrice.ToString(), font, XBrushes.Black, price);

                group.Offset(vettIva);
                price.Offset(vettIva);
            }

            gfx.Dispose();
            #endregion

            // Save the document...
            string folderPDF = baseFolderPDF + DateTime.Now.ToString("yyyyMMdd") + @"\";
            if (!Directory.Exists(folderPDF))
            {
                Directory.CreateDirectory(folderPDF);
            }
            string filename = $"{folderPDF}{request.Ricevuta.nome_ricevuta}_{request.Cliente.surname}{request.Cliente.name}";
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
