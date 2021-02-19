using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CoreCodeCamp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        // Ao utilizar ActionResult<Tipo>, é dado como Ok
        // Caso o retorno seja desse tipo. 
        // É possível utilizar IActionResult caso não queira explicitar
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                // Por ser async, é necessario await
                // Caso contrário, será retornado o OK mesmo sem atribuir o results.
                var results = await _repository.GetAllCampsAsync(includeTalks);

                // Suporta outros tipos de collections como IEnumerable etc   
                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                // Não existe um método para retoranr 500 Internal Server ERror
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetCampById(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                if (result == null) return NotFound();

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }


        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                // Validar no controller campo único para que não se repita
                var campExist = await _repository.GetCampAsync(model.Moniker);
                if (campExist != null)
                {
                    return BadRequest("Moniker is in use");
                }

                // Gerar o Link antes de criar o objeto, caso haja algum problema
                var location = _linkGenerator.GetPathByAction("GetCampById",
                    "Camps",
                    new { moniker = model.Moniker });

                /*
                 * if (ModelState.IsValid)
                 * Serve para validar a Model, usando as annotations.
                 * Somente é utilizado caso queira algo customizado ou não esteja usando [ApiController]
                 */

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);
                if (await _repository.SaveChangesAsync())
                {
                    // uri para pegar o novo objeto
                    // Mapear do jeito $"/api/camps/{camp.Moniker}" funciona, porém a string está hardcoded
                    // Para isso existe a biblioteca do Asp.NET Core LinkGenerator
                    return Created("", _mapper.Map<CampModel>(camp));
                }
                
        // Por utilizar Query Strings, não está sendo passado a data pela rota, e sim como parâmetro
        // Sendo assim, a rota não precias ser "search/{theDate}"
        [HttpGet("search/")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return NotFound();

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
            return BadRequest();
        }         
    }
}