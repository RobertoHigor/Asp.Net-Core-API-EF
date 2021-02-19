using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using CoreCodeCamp.Models;
using AutoMapper;
using System.Linq;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
    }
}