using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Infrastructure.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        public RedisCacheService(ILogger<RedisCacheService> logger, IConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger;
            _connectionMultiplexer = connectionMultiplexer;
            _database = connectionMultiplexer.GetDatabase(1); // Use database 1
        }
        public T GetCacheAsync<T>(string cacheKey)
        {
            var cachedData = _database.StringGet(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation($"Cache miss for key: {cacheKey}");
                return default;
            }
            _logger.LogInformation($"Cache hit for key: {cacheKey}");
            return JsonConvert.DeserializeObject<T>(cachedData);
        }
        public bool SetCacheAsync<T>(string cacheKey, T value, DateTimeOffset expirationTime)
        {
            var serializedData = JsonConvert.SerializeObject(value);
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _database.StringSet(cacheKey, serializedData, expiryTime);
            _logger.LogInformation($"Data cached for key: {cacheKey} with expiration: {expirationTime}");
            return isSet;
        }
        public object RemoveData(string key)
        {
            bool _isKeyExist = _database.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _database.KeyDelete(key);
            }
            return false;
        }
    }
}
