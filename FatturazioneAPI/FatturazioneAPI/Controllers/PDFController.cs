using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class PDFController : Controller
    {
        private readonly PDFBiz _PDF;

        public PDFController(IConfiguration configuration)
        {
            _PDF = new PDFBiz(configuration);

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
                string fileName = _PDF.GeneraPDFFromRicevuta(request);
                var stream = new FileStream(fileName, FileMode.Open);
                return File(stream, "application/pdf");
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }
    }
}
