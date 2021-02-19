using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

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

                Camp camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);

                if (await _repository.SaveChangesAsync())
                {
                    // uri para pegar o novo objeto
                    // Mapear do jeito $"/api/camps/{camp.Moniker}" funciona, porém a string está hardcoded
                    // Para isso existe a biblioteca do Asp.NET Core LinkGenerator
                    return Created($"/api/camps{camp.Moniker}", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
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
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) NotFound($"Could not find camp with moniker of {moniker}");

                // Usando mapper para aplicar os valores de um objeto para o outro
                // Assim não é necessário fazer manualmente oldcamp.Name = model.Name para todas as propriedades
                // source -> target
                _mapper.Map(model, oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            // FallThrough
            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null) return NotFound();

                _repository.Delete(oldCamp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest("Failed to delete camp");
        }
    }
}