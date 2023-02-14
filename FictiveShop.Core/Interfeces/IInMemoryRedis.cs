namespace FictiveShop.Core.Interfeces
{
    public interface IInMemoryRedis
    {
        string Get(string key);

        bool Set(string key, string value, TimeSpan expiry);

        bool Delete(string key);
    }
}