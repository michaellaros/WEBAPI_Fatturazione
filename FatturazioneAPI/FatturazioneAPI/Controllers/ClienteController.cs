using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;
using Newtonsoft.Json;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly DataBase _dataBase;
        private ILogger<ClienteController> _logger;

        public ClienteController(DataBase dataBase, ILogger<ClienteController> logger)
        {
            _dataBase = dataBase;
            _logger = logger;
        }



        [HttpPost]
        [Route("Search")]
        public IActionResult RicercaCliente(RicercaClienteRequest request)
        {
            try
            {
                if (request.IsEmpty())
                {
                    return BadRequest();
                }

                RicercaClientiResponse response = new RicercaClientiResponse
                {
                    clienti = _dataBase.GetClienti(request)
                };
                if (response.clienti == null)
                {
                    return StatusCode(500, "Errore nel recupero del cliente");
                }

                if (response.clienti.Count == 0)
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"RicercaCliente error: {JsonConvert.SerializeObject(ex)}");
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertCliente(ClientiModel client)
        {
            try
            {
                if (string.IsNullOrEmpty(client.passport_number) && !_dataBase.CheckValidProvince(client.zipcode, client.city, client.country_code, client.district_code))
                {
                    return Conflict("Dati comune non validi!");
                }
                int client_id = _dataBase.InsertClient(client);
                ClientiModel result = _dataBase.GetCliente(client_id);
                if (result == null)
                {
                    return StatusCode(500, "Errore nel recupero del cliente creato!");
                }


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"InsertCliente error: {JsonConvert.SerializeObject(ex)}");
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("Update")]
        public IActionResult UpdateCliente(ClientiModel client)
        {
            try
            {
                if (!client.id.HasValue)
                {
                    return BadRequest();
                }
                if (string.IsNullOrEmpty(client.passport_number) && !_dataBase.CheckValidProvince(client.zipcode, client.city, client.country_code, client.district_code))
                {
                    return BadRequest("Dati città non validi!");
                }

                _dataBase.UpdateClient(client);
                ClientiModel result = _dataBase.GetCliente(client.id.Value);
                if (result == null)
                {
                    return StatusCode(500, "Errore nel recupero del cliente creato!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdateCliente error: {JsonConvert.SerializeObject(ex)}");
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Route("GetProvince")]
        public IActionResult GetProvince(string filter)
        {
            try
            {
                List<ProvinciaModel> result = _dataBase.GetProvince(filter);


                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetProvince error: {JsonConvert.SerializeObject(ex)}");
                return StatusCode(500, ex.Message);
            }

        }
    }
}
