using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly DataBase _dataBase;

        public ClienteController(DataBase dataBase)
        {
            _dataBase = dataBase;

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
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertCliente(ClientiModel client)
        {
            try
            {
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
                return StatusCode(500, ex.Message);
            }

        }
    }
}
