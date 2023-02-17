using FictiveShop.Core.Interfeces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace FictiveShop.Infrastructure.DataAccess
{
    public class RedisContext : IInMemoryRedis
    {
        private Dictionary<string, (string Value, DateTime Expiry)> data;
        private readonly ILogger<RedisContext> _logger;

        public RedisContext(ILogger<RedisContext> logger = null)
        {
            data = new Dictionary<string, (string Value, DateTime Expiry)>();
            _logger = logger;
        }

        public string Get(string key)
        {
            if (data.TryGetValue(key, out (string Value, DateTime Expiry) value))
            {
                if (value.Item2 >= DateTime.UtcNow)
                {
                    return value.Item1;
                }
                data.Remove(key);
            }
            _logger?.LogError($"The provied key: {key} doesn't exist in redis db");
            return null;
        }

        public bool Set(string key, string value, TimeSpan expiry)
        {
            var expiryTime = DateTime.UtcNow.Add(expiry);
            try
            {
                if (!data.ContainsKey(key))
                {
                    data.Add(key, new(value, expiryTime));
                }
                else
                {
                    data[key] = new(value, expiryTime);
                }

                return true;
            }
            catch (Exception)
            {
                _logger?.LogError($"The provied key: {key} doesn't exist in redis db");
                return false;
            }
        }

        public bool Delete(string key)
        {
            if (!data.ContainsKey(key))
            {
                _logger?.LogError($"The provied key: {key} doesn't exist in redis db");
                return false;
            }
            return data.Remove(key);
        }
    }
}