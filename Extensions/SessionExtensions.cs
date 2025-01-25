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
        public static void SetDecimal(this ISession session, string key, decimal value)
        {
            session.SetString(key, value.ToString());
        }

        public static decimal GetDecimal(this ISession session, string key)
        {
            var value = session.GetString(key);
            return !string.IsNullOrEmpty(value) ? decimal.Parse(value) : 0;
        }
    }
}
