using Application.CandidateInfos.Commands;
using Application.CandidateInfos.Queries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class CandidateController : ApiControllerBase
    {
        
        [HttpPost]
        [Route("addOrUpdate")]
        public async Task<ActionResult<Guid>> AddOrUpdateCandidate(AddorUpdateCandidateCommand command)
        {
            Guid result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult<List<CandidatesVm>>> GetAllCandidate()
        {
            var query = new GetAllCandidatesQuery();
            return Ok(await Mediator.Send(query));
        }

    }
}
