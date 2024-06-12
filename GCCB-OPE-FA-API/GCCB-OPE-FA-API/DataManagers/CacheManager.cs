using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace GCCB_OPE_FA_API.DataManagers
{
    public class CacheManager 
    {
        private readonly Lazy<IConnectionMultiplexer> _connection = null;

        public CacheManager(string redisConnectionString)
        {
            try
            {
                _connection = new Lazy<IConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(redisConnectionString);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IConnectionMultiplexer Connection { get { return _connection.Value; } }
        public IDatabase Database => Connection.GetDatabase();
        public T Get<T>(string key)
        {
            try
            {
                var result = Database.StringGet(key);
                if (!result.HasValue || result.IsNullOrEmpty) return default;
                T t = JsonConvert.DeserializeObject<T>(result);
                return t;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool Save<T>(string key, T value, TimeSpan? dt = null)
        {
            try
            {
                var settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                return Database.StringSet(key, JsonConvert.SerializeObject(value, settings), dt);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool KeyExists(string key)
        {
            try
            {
                return Database.KeyExists(key);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
