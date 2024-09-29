using Application.CandidateInfos.Commands;
using Application.CandidateInfos.Queries;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    public class CandidateController : ApiControllerBase
    {
        private readonly ILogger<CandidateController> _logger;
        private const string CandidateCacheKey = "CandidateCacheKey";
        private readonly IMemoryCache _memoryCache;

        public CandidateController(
            ILogger<CandidateController> logger,
            IMemoryCache memoryCache
            )
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        [HttpPost]
        [Route("addOrUpdate")]
        public async Task<ActionResult<Guid>> AddOrUpdateCandidate(AddorUpdateCandidateCommand command)
        {
            _memoryCache.Remove(CandidateCacheKey);
            Guid result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult<List<CandidatesVm>>> GetAllCandidate()
        {
            // Attempt to retrieve data from cache
            if (!_memoryCache.TryGetValue(CandidateCacheKey, out List<CandidatesVm> cacheData))
            {
                _logger.LogInformation("Fetching data from database.");

                // Query to get the candidate data
                var query = new GetAllCandidatesQuery();
                var data = await Mediator.Send(query);

                // Cache the data with a defined expiration time
                var expirationTime = DateTimeOffset.Now.AddMinutes(10.0);
                _memoryCache.Set(CandidateCacheKey, data, expirationTime);

                return Ok(data);
            }

            // If cache data exists, return it
            return Ok(cacheData);
        }

    }
}
