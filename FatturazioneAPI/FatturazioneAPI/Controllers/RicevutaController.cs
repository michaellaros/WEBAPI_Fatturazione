﻿using Microsoft.AspNetCore.Mvc;
using FatturazioneAPI.Models;
using FatturazioneAPI.Models.Requests;
using FatturazioneAPI.Models.Responses;
using FatturazioneAPI.Services;

namespace FatturazioneAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RicevutaController : Controller
    {
        private readonly RicevutaBiz _ricevuta;
        private readonly PDFBiz _PDF;
        private readonly DataBase _dataBase;

        public RicevutaController(IConfiguration configuration, PDFBiz pdf, RicevutaBiz ricevuta, DataBase dataBase)
        {
            _dataBase = dataBase; //possibile miglioria? interfaccia?
            _ricevuta = ricevuta;
            _PDF = pdf;

        }

        [HttpPost]
        [Route("GetRicevuta")]
        public IActionResult GetRicevuta(GetRicevutaRequest request)
        {
            try
            {
                RicevutaModel result = _ricevuta.RicevutaCostruction(request.fileName);
                if (result == null)
                {
                    return NotFound();
                }

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
                if (request.IsEmpty())
                {
                    return BadRequest();
                }

                RicercaRicevutaResponse result = new RicercaRicevutaResponse
                {
                    ricevute = _ricevuta.RicercaRicevuta(request)
                };
                if (result.ricevute.Count == 0)
                {
                    return NotFound("Nessuna ricevuta trovata!");
                }
                result.ricevute = _dataBase.RemoveReceiptWithInvoice(result.ricevute);
                if (result.ricevute == null)
                {
                    return StatusCode(500, "Errore nel recupero ricevuta!");
                }

                if (result.ricevute.Count == 0)
                {
                    return NotFound("Nessuna ricevuta trovata!");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Route("Storico")]
        public IActionResult RicercaStoricoRicevuta(RicercaRicevutaStoricoRequest request)
        {
            try
            {

                List<RicevutaStoricoModel> result = _dataBase.RicercaRicevutaStorico(request);

                if (result == null)
                {
                    return StatusCode(500, "Errore nel recupero storico ricevuta!");
                }

                if (result.Count == 0)
                {
                    return NotFound("Nessuna ricevuta trovata!");
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
