using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AppleStore.Extensions
{
    public static class SessionExtensions
    {
        // Phương thức để lưu trữ đối tượng dưới dạng JSON trong Session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Phương thức để lấy đối tượng JSON từ Session
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
