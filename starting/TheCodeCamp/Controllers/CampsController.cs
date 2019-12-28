using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps")]
    public class CampsController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [Route()]
        public async Task<IHttpActionResult> Get(bool includeTalks =false)
        {
            try
            {
                var result = await _repository.GetAllCampsAsync(includeTalks);
                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

                //return NotFound();
                return Ok(mappedResult);
            }
            catch (Exception e) {
                return InternalServerError(e);
            }            
        }

        [Route("{moniker}",Name = "GetCamp")]
        public async Task<IHttpActionResult> Get(String moniker, bool includeTalks = false) {
            try {
                var result = await _repository.GetCampAsync(moniker, includeTalks);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception e) {
                return InternalServerError(e);
            }
        }

        [Route("searchByEventDate/{eventDate:datetime}")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks=false)
        {
            try
            {
                var result = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);
                if (result == null) return NotFound();
                return Ok(_mapper.Map<CampModel[]>(result));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(CampModel model) {
            try {
                if (await _repository.GetCampAsync(model.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "The moniker is already un use");
                }

                if (ModelState.IsValid) {                   
                    var camp = _mapper.Map<Camp>(model);
                    _repository.AddCamp(camp);

                    if (await _repository.SaveChangesAsync()) {
                        var newModel = _mapper.Map<CampModel>(camp);                        

                        return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker}, newModel);
                    }
                }                
            }
            catch (Exception e) {
                return InternalServerError(e);
            }
            return BadRequest(ModelState);
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Put(String moniker,CampModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _mapper.Map(model, camp);

                if (await _repository.SaveChangesAsync()) {                
                    return Ok(_mapper.Map<CampModel>(camp));
                }                
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return BadRequest(ModelState);
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Delete(String moniker)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return NotFound();

                _repository.DeleteCamp(camp);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else {
                    return InternalServerError();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }            
        }
    }
}