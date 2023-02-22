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
        [HttpGet]
        [Route("{fileName}")]
        public IActionResult Get(string fileName)
        {
            RicevutaModel result = _ricevuta.RicevutaCostruction(fileName.Replace("%2F","/")); //ripristino gli / che tramite url vengono sostiuiti
            if(result==null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRicevuta")]
        public IActionResult GetRicevuta(GetRicevutaRequest request)
        {
            RicevutaModel result = _ricevuta.RicevutaCostruction(request.fileName);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //[HttpGet]
        //[Route("DB/{fileName}")]
        //public IActionResult GetDB(string fileName)
        //{
        //    GetDBResponse response = new GetDBResponse();
        //    try 
        //    {
        //        response.ricevute.Add(_DataBase.GetRicevuta_DB(fileName));

        //        if (response.ricevute.Count == 0)
        //            return NotFound();
        //        return Ok(response);
        //    }
        //    catch(Exception ex) 
        //    { 
        //        return StatusCode(500, ex); 
        //    }
            
        //}

        //[HttpGet]
        //[Route("DB")]
        //public IActionResult GetDB()
        //{

        //    GetDBResponse response = new GetDBResponse();
        //    try
        //    {
        //        response.ricevute = _DataBase.GetRicevute();

        //        if (response.ricevute.Count == 0)
        //            return NotFound();
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex);
        //    }
        //}


        [HttpPost]
        public IActionResult RicercaRicevuta(RicercaRicevutaRequest request) 
        {
            try
            {
                if (request.IsEmpty()) return BadRequest();

                RicercaRicevutaResponse response = new RicercaRicevutaResponse
                {
                    ricevute = _ricevuta.RicercaRicevuta(request)
                };
                if (response.ricevute == null) return StatusCode(500,"Errore nel recupero ricevuta!");
                if (response.ricevute.Count == 0) return NotFound("Nessuna ricevuta trovata!");
                return Ok(response);
            }catch(Exception ex)
            {
                return StatusCode(500,ex);
            }
            
        }


    }
}
