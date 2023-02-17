using Drawing = PdfSharp.Drawing;

using PdfSharp.Drawing;
using PdfSharp.Pdf;
using FatturazioneAPI.Models.Requests;
using Microsoft.Data.SqlClient;
using PdfSharp.Drawing.Layout;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using FatturazioneAPI.Models;
using System.Drawing.Printing;
using PdfSharp.Charting;
using PdfSharp.Pdf.IO;

namespace FatturazioneAPI.Services
{
    public class PDFBiz
    {
        readonly IConfiguration configuration;
        private string baseFolderPDF { get { return configuration.GetSection("directories").GetValue<string>("folderPDF"); } }

        public PDFBiz(IConfiguration configuration)
        {
            this.configuration = configuration;

        }

        public void GeneraPDF()
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";

            // Create an empty page
            PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            XPen pen = new XPen(XColors.Black, 0.5);
            // Draw the text
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(20, 400, 100, 220);
            tf.Alignment = XParagraphAlignment.Left;

            

            for(int i = 270;i < 320; i+=20)
            {
                XRect codice = new XRect(20, i, 100, 20);
                XRect descrizione = new XRect(120, i, 255, 20);
                XRect quantita = new XRect(375, i, 100, 20);
                XRect prezzoTot = new XRect(475, i, 100, 20);

                codice.Offset(2, 3);
                descrizione.Offset(2, 3);
                quantita.Offset(2, 3);
                prezzoTot.Offset(2, 3);

                tf.DrawString($" articolo {i}", font, XBrushes.Black, codice);
                tf.DrawString($" Descrizione {i}", font, XBrushes.Black, descrizione);
                tf.DrawString($" Quantita {i}", font, XBrushes.Black, quantita);
                tf.DrawString($" PrezzoTot {i}", font, XBrushes.Black, prezzoTot);
            }


            tf.DrawString($"{page.Width} x {page.Height}", font, XBrushes.Black, rect);

            // Save the document...
            const string filename = "C:\\Temp\\HelloWorld1.pdf";
            document.Save(filename);
            // ...and start a viewer.
            //Process.Start(filename);
        }

        public void GeneraFattura() {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            PdfDocument document = PdfReader.Open("C:\\Temp\\template\\modello-fattura.pdf", PdfDocumentOpenMode.Modify);

            // Create an empty page
            PdfPage page = document.Pages[0];

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            XPen pen = new XPen(XColors.Black, 0.5);
            // Draw the text
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect = new XRect(20, 400, 100, 220);
            tf.Alignment = XParagraphAlignment.Left;

            int y = 275;

            XRect codice = new XRect(28, y, 100, 25);
            XRect descrizione = new XRect(83, y, 255, 25);
            XRect quantita = new XRect(280, y, 100, 25);
            XRect prezzoTot = new XRect(330, y, 100, 25);

            XVector vett = new XVector(0, 24.4);

            for (int i = 0; i < 14; i++)
            {
                tf.DrawString($"Cod{i}", font, XBrushes.Black, codice);
                tf.DrawString($"Descrizione{i}", font, XBrushes.Black, descrizione);
                tf.DrawString($"{i}", font, XBrushes.Black, quantita);
                tf.DrawString($"PrezzoTot{i}", font, XBrushes.Black, prezzoTot);

                codice.Offset(vett);
                descrizione.Offset(vett);
                quantita.Offset(vett);
                prezzoTot.Offset(vett);
            }

            // Save the document...
            const string filename = "C:\\Temp\\HelloWorld1.pdf";
            document.Save(filename);
        }

        public void GeneraPDFFromRicevuta(SendPDFRequest request)
        {
            #region config

            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Fattura";

            // Create an empty page
            PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

            

            XPen pen = new XPen(XColors.Black, Math.PI);
            #endregion

            #region table articoli


            #endregion


            // Save the document...
            string folderPDF = baseFolderPDF+DateTime.Now.ToString("yyyyMMdd")+"\\";
            if (!Directory.Exists(folderPDF))
            {
                Directory.CreateDirectory(folderPDF);
            }
            string filename = $"{folderPDF}{request.Ricevuta.nome_ricevuta}_{request.Cliente.clientSurname}{request.Cliente.clientName}";
            int nameUsed = 0;
            while(!File.Exists(filename + (nameUsed == 0 ? "" : $"_{nameUsed}")))
            {
                nameUsed++;
            }
            document.Save(filename + (nameUsed == 0 ? "" : $"_{nameUsed}"));
        }

        private void DrawTableArticoli(PdfPage page, int y)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XPen pen = new XPen(XColors.Black, 0.5);


            XRect codice = new XRect(20, y, 100, 20);
            XRect descrizione = new XRect(120, y, 255, 20);
            XRect quantita = new XRect(375, y, 100, 20);
            XRect prezzoTot = new XRect(475, y, 100, 20);

            XVector vett = new XVector(0, 20);

            for (int i = 0; i < 20; i++)
            {
                gfx.DrawRectangle(pen, codice);
                gfx.DrawRectangle(pen, descrizione);
                gfx.DrawRectangle(pen, quantita);
                gfx.DrawRectangle(pen, prezzoTot);

                codice.Offset(vett);
                descrizione.Offset(vett);
                quantita.Offset(vett);
                prezzoTot.Offset(vett);
            }

        }

        private void ValorizzaTableArticoli(PdfPage page, int y, List<ArticoloModel> articoli)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XPen pen = new XPen(XColors.Black, 0.5);
            XTextFormatter tf = new XTextFormatter(gfx);
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
            int margine = 3;

            XRect codice = new XRect(20+ margine, y+ margine, 100, 20);
            XRect descrizione = new XRect(120 + margine, y+margine, 255, 20);
            XRect quantita = new XRect(375 + margine, y + margine, 100, 20);
            XRect prezzoTot = new XRect(475 + margine, y + margine, 100, 20);

            tf.DrawString("Codice articolo", font, XBrushes.Black, codice);
            tf.DrawString("Descrizione", font, XBrushes.Black, descrizione);
            tf.DrawString("Quantita", font, XBrushes.Black, quantita);
            tf.DrawString("PrezzoTot", font, XBrushes.Black, prezzoTot);

            XVector vett = new XVector(0, 20);



        }
    }
}
