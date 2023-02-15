using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;
using System.Linq.Expressions;

namespace Ricevuta.Controllers
{
    [EnableCors("cors")]
    [Route("api/[controller]")]
    [ApiController]
    public class RicevutaController : Controller
    {
        private readonly RicevutaBiz _ricevuta;
        private readonly DataBase _DataBase;
        
        public RicevutaController(IConfiguration configuration)
        {
            _DataBase = new DataBase(configuration); //possibile miglioria? interfaccia?
            _ricevuta = new RicevutaBiz();
            
        }
        [HttpGet]
        [Route("{fileName}")]
        public IActionResult Get(string fileName)
        {
            RicevutaModel result = _ricevuta.RicevutaCostruction(fileName.Replace("%2F","/"));
            if(result==null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRicevuta")]
        public IActionResult GetRicevuta(GetRicevutaRequest request)
        {
            RicevutaModel result = _ricevuta.RicevutaCostruction(request.fileName.Replace("%2F", "/"));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [Route("DB/{fileName}")]
        public IActionResult GetDB(string fileName)
        {
            GetDBResponse response = new GetDBResponse();
            try 
            {
                response.ricevute.Add(_DataBase.GetRicevuta_DB(fileName));

                if (response.ricevute.Count == 0)
                    return NotFound();
                return Ok(response);
            }
            catch(Exception ex) 
            { 
                return StatusCode(500, ex); 
            }
            
        }

        [HttpGet]
        [Route("DB")]
        public IActionResult GetDB()
        {

            GetDBResponse response = new GetDBResponse();
            try
            {
                response.ricevute = _DataBase.GetRicevute();

                if (response.ricevute.Count == 0)
                    return NotFound();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


        [HttpPost]
        public IActionResult RicercaRicevuta(RicercaRicevutaRequest request) 
        {
            if (request.IsEmpty()) return BadRequest();

            RicercaRicevutaResponse response = new RicercaRicevutaResponse
            {
                ricevute = _ricevuta.RicercaRicevuta(request)
            };
            if(response.ricevute == null) return StatusCode(500);
            if(response.ricevute.Count == 0) return NotFound();
            return Ok(response);
        }

        
    }
}
