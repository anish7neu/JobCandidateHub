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
        //private readonly ICacheService _cacheService;
        private readonly ILogger<CandidateController> _logger;
        private const string CandidateCacheKey = "CandidateCacheKey";
        private readonly IDistributedCache _distributedCache;

        public CandidateController(
            //ICacheService cacheService, 
            ILogger<CandidateController> logger,
            IDistributedCache distributedCache
            )
        {
            _distributedCache = distributedCache;
            //_cacheService = cacheService;
            _logger = logger;
        }

        [HttpPost]
        [Route("addOrUpdate")]
        public async Task<ActionResult<Guid>> AddOrUpdateCandidate(AddorUpdateCandidateCommand command)
        {
            //_cacheService.RemoveData(CandidateCacheKey);
            await _distributedCache.RemoveAsync(CandidateCacheKey);
            Guid result = await Mediator.Send(command);

            return Ok(result);
        }

        [HttpGet]
        [Route("view")]
        public async Task<ActionResult<List<CandidatesVm>>> GetAllCandidate()
        {
            //var cacheData = _cacheService.GetCacheAsync<IEnumerable<CandidatesVm>>(CandidateCacheKey);
            var cacheData = await _distributedCache.GetStringAsync(CandidateCacheKey);
            if (cacheData == null)
            {
                _logger.LogInformation("Fetching data from database.");

                var query = new GetAllCandidatesQuery();
                var data = await Mediator.Send(query);
                var expirationTime = DateTimeOffset.Now.AddMinutes(10.0);
                cacheData = JsonConvert.SerializeObject(data);
                var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
                await  _distributedCache.SetStringAsync(CandidateCacheKey, cacheData, cacheOptions);
                return Ok(data);
            }
            return Ok(JsonConvert.DeserializeObject<IEnumerable<CandidatesVm>>(cacheData));
        }

    }
}
