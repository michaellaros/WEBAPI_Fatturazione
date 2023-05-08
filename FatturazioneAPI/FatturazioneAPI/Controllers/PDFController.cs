using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class PDFController : Controller
    {
        private readonly PDFBiz _PDF;
        private readonly DataBase _database;

        public PDFController(PDFBiz pdf, DataBase dataBase)
        {
            _PDF = pdf;
            _database = dataBase;
        }

        //[HttpGet]
        //public IActionResult Get()
        //{

        //    string fileName = _PDF.GeneraFattura();
        //    var stream = new FileStream(fileName, FileMode.Open);
        //    return File(stream, "application/pdf");
        //}

        [HttpPost]
        [Route("SendPDF")]
        public IActionResult SendPDF(SendPDFRequest request)
        {
            try
            {
                if (_database.CheckInvoiceAlreadyDone(new CheckInvoiceAlreadyDoneRequest(request.receiptName)))
                {
                    return Conflict("Fattura già esistente!");
                }
                GetInfoTransazioneRequest receipt_number = _PDF.GeneraPDFFromRicevuta(request);
                //string fileName = _PDF.GeneraPDFFromRicevuta(request);
                //var stream = new FileStream(fileName, FileMode.Open);
                return Ok(receipt_number);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("GetExistingPDF")]
        public IActionResult GetExistingPDF(GetInfoTransazioneRequest request)
        {
            try
            {
                GetInfoTransazioneResponse response = _database.GetInfoTransazione(request);
                string? fileName = _PDF.GetPDFnamefromTransaction(response.date, $@"{request.receipt_number}_{response.store_id}");
                if (string.IsNullOrEmpty(fileName)) { return NotFound(); }
                var stream = new FileStream(fileName, FileMode.Open);

                return File(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("GetCreditNotes")]
        public IActionResult GetCreditNotes(GetInfoTransazioneRequest request)
        {
            try
            {

                GetInfoTransazioneResponse response = _database.GetInfoTransazione(request);
                SendPDFRequest requestPDF = new SendPDFRequest
                {
                    client_id = response.client_id,
                    receiptName = $@"{response.store_id}_{response.workstation_id}_{response.ta}"
                };
                if (!_database.CheckInvoiceAlreadyDone(new CheckInvoiceAlreadyDoneRequest(requestPDF.receiptName)))
                {
                    return Conflict("Fattura non esistente o già stornata!");
                }



                GetInfoTransazioneRequest receipt_number = _PDF.GeneraPDFFromRicevuta(requestPDF, request);
                //string fileName = _PDF.GeneraPDFFromRicevuta(request);
                //var stream = new FileStream(fileName, FileMode.Open);
                return Ok(receipt_number);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
