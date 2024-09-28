using Application.CandidateInfos.Commands;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using W;

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
        public async Task<ActionResult<Guid>> GetAllCandidate(AddorUpdateCandidateCommand command)
        {
            Guid result = await Mediator.Send(command);

            return Ok(result);
        }

    }
}
