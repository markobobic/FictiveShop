using FictiveShop.Core.Interfeces;
using System;
using System.Collections.Generic;

namespace FictiveShop.Infrastructure.DataAccess
{
    public class RedisContext : IInMemoryRedis
    {
        private Dictionary<string, (string Value, DateTime Expiry)> data;

        public RedisContext()
        {
            data = new Dictionary<string, (string Value, DateTime Expiry)>();
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
                return false;
            }
        }

        public bool Delete(string key)
        {
            return data.Remove(key);
        }
    }
}