using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ICacheService
    {
        T GetCacheAsync<T>(string cacheKey);
        bool SetCacheAsync<T>(string cacheKey, T value, DateTimeOffset expirationTime);
        object RemoveData(string cacheKey); 
    }
}
