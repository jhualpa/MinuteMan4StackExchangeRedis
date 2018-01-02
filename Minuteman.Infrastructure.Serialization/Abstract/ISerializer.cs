namespace Minuteman.Infrastructure.Serialization.Abstract
{
    public interface ISerializer
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string value);
    }
}
