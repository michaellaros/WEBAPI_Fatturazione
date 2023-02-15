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
            RicevutaModel result = _ricevuta.RicevutaCostruction(fileName);
            if(result==null) return NotFound();
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

        [HttpGet]
        public IActionResult GetTest()
        {
            return Ok("{\r\n    \"glossary\": {\r\n        \"title\": \"example glossary\",\r\n\t\t\"GlossDiv\": {\r\n            \"title\": \"S\",\r\n\t\t\t\"GlossList\": {\r\n                \"GlossEntry\": {\r\n                    \"ID\": \"SGML\",\r\n\t\t\t\t\t\"SortAs\": \"SGML\",\r\n\t\t\t\t\t\"GlossTerm\": \"Standard Generalized Markup Language\",\r\n\t\t\t\t\t\"Acronym\": \"SGML\",\r\n\t\t\t\t\t\"Abbrev\": \"ISO 8879:1986\",\r\n\t\t\t\t\t\"GlossDef\": {\r\n                        \"para\": \"A meta-markup language, used to create markup languages such as DocBook.\",\r\n\t\t\t\t\t\t\"GlossSeeAlso\": [\"GML\", \"XML\"]\r\n                    },\r\n\t\t\t\t\t\"GlossSee\": \"markup\"\r\n                }\r\n            }\r\n        }\r\n    }\r\n}");
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
