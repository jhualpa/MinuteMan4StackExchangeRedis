using Minuteman.Infrastructure.Serialization.Abstract;
using ServiceStack.Text;

namespace Minuteman.Infrastructure.Serialization
{
    public class ServiceStackSerializer : ISerializer
    {
        public string Serialize<T>(T value)
        {
            return JsonSerializer.SerializeToString<T>(value);
        }

        public T Deserialize<T>(string value)
        {
            return JsonSerializer.DeserializeFromString<T>(value);
        }
    }
}
