namespace Minuteman.Infrastructure.Configuration.Settings
{
    public enum RedisKey
    {
        Login,
        ClientApiKey
    }

    public enum RedisChannel
    {
    }

    public interface IRedisSettings
    {
        string Configuration { get; }
        string Key(RedisKey key);
        string Channel(RedisChannel channel);
    }
}
