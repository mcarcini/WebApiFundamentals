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
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var result = await _repository.GetAllCampsAsync();
                var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

                //return NotFound();
                return Ok(mappedResult);
            }
            catch (Exception e) {
                return InternalServerError(e);
            }            
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Get(String moniker) {
            try {
                var result = await _repository.GetCampAsync(moniker);
                if (result == null) return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception e) {
                return InternalServerError(e);
            }
        }
    }
}