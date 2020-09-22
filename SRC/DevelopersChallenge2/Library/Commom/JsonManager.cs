using System.Text.Json;

namespace Accountant.Library.Commom
{
    public static class JsonManager
    {
        public static string ObjectToJson(object objectValue)
        {
            var jsonString = JsonSerializer.Serialize(objectValue);
            return jsonString;
        }

        public static T JsonToObject<T>(string json)
        {
            var objectValue = JsonSerializer.Deserialize<T>(json);
            return objectValue;
        }
    }
}
