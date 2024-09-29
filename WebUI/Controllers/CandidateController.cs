using Application.CandidateInfos.Commands;
using Application.CandidateInfos.Queries;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class CandidateController : ApiControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CandidateController> _logger;
        private const string CandidateCacheKey = "CandidateCacheKey";

        public CandidateController(ICacheService cacheService, ILogger<CandidateController> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpPost]
        [Route("addOrUpdate")]
        public async Task<ActionResult<Guid>> AddOrUpdateCandidate(AddorUpdateCandidateCommand command)
        {
            _cacheService.RemoveData(CandidateCacheKey);

            Guid result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult<List<CandidatesVm>>> GetAllCandidate()
        {
            var cacheData = _cacheService.GetCacheAsync<IEnumerable<Candidate>>(CandidateCacheKey);
            if (cacheData == null)
            {
                _logger.LogInformation("Fetching data from database.");

                var query = new GetAllCandidatesQuery();
                var data = await Mediator.Send(query);
                var expirationTime = DateTimeOffset.Now.AddMinutes(10.0);
                _cacheService.SetCacheAsync(CandidateCacheKey, data, expirationTime);
                return Ok(data);
            }
            return Ok(cacheData);
        }

    }
}
