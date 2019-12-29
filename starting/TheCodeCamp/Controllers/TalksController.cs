using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [RoutePrefix("api/camps/{moniker}/talks")]
    public class TalksController : ApiController
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        public TalksController(ICampRepository repository, IMapper mapper) {
            _repository = repository;
            _mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> Get(string moniker, bool includeSpeakers = false) {
            try
            {
                var results = await _repository.GetTalksByMonikerAsync(moniker, includeSpeakers);
                return Ok(_mapper.Map<IEnumerable<TalkModel>>(results));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


        [Route("{id:int}", Name="GetTalk")]
        public async Task<IHttpActionResult> Get(string moniker,int id, bool includeSpeakers = false)
        {
            try
            {
                var result = await _repository.GetTalkByMonikerAsync(moniker, id, includeSpeakers);
                if (result == null) return NotFound();
                
                return Ok(_mapper.Map<TalkModel>(result));
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(string moniker, TalkModel talkModel) {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp != null) {
                    var talk = _mapper.Map<Talk>(talkModel);
                    talk.Camp = camp;
                    _repository.AddTalk(talk);

                    if (await _repository.SaveChangesAsync()) {
                        return CreatedAtRoute("GetTalk", new { moniker = moniker, id = talk.TalkId }, _mapper.Map<TalkModel>(talk));
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return BadRequest();
        }

        [Route("id:int")]
        public async Task<IHttpActionResult> Put(string moniker,int id, TalkModel talkModel)
        {
            try
            {
                if (ModelState.IsValid)
                {                
                    var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                    if (talk == null) return NotFound();

                    _mapper.Map(talkModel, talk);

                    if (await _repository.SaveChangesAsync()) {
                        return Ok(_mapper.Map<TalkModel>(talk));
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            return BadRequest();
        }

    }
}