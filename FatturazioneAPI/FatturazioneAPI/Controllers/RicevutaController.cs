using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;
using System.Linq.Expressions;

namespace Ricevuta.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RicevutaController : Controller
    {
        private readonly RicevutaBiz _ricevuta;
        private readonly PDFBiz _PDF;

        public RicevutaController(IConfiguration configuration)
        {
            //_DataBase = new DataBase(configuration); //possibile miglioria? interfaccia?
            _ricevuta = new RicevutaBiz(configuration);
            _PDF = new PDFBiz(configuration);

        }

        [HttpPost]
        [Route("GetRicevuta")]
        public IActionResult GetRicevuta(GetRicevutaRequest request)
        {
            try
            {
                RicevutaModel result = _ricevuta.RicevutaCostruction(request.fileName);
                if (result == null) return NotFound();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult RicercaRicevuta(RicercaRicevutaRequest request) 
        {
            try
            {
                if (request.IsEmpty()) return BadRequest();

                RicercaRicevutaResponse result = new RicercaRicevutaResponse
                {
                    ricevute = _ricevuta.RicercaRicevuta(request)
                };
                if (result.ricevute == null) return StatusCode(500,"Errore nel recupero ricevuta!");
                if (result.ricevute.Count == 0) return NotFound("Nessuna ricevuta trovata!");
                return Ok(result);
            }catch(Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
            
        }


    }
}
