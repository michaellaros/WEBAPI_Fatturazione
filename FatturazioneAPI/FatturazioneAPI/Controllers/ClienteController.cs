using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;
using System.Linq.Expressions;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClienteController : Controller
    {
        private readonly ClientiBiz _cliente;
        
        public ClienteController(IConfiguration configuration)
        {
            _cliente = new ClientiBiz(configuration);
            
        }

        

        [HttpPost]
        public IActionResult RicercaCliente(RicercaClienteRequest request)
        {
            try
            {
                if (request.IsEmpty()) return BadRequest();

                RicercaClientiResponse response = new RicercaClientiResponse
                {
                    clienti = _cliente.GetClienti(request)
                };
                if (response.clienti == null) return StatusCode(500, "Errore nel recupero del cliente");
                if (response.clienti.Count == 0) return NotFound();
                return Ok(response);
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
