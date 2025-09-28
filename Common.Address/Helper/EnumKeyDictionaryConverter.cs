using Newtonsoft.Json;

namespace Common.Address.Helper
{
    /// <summary>
    /// This attribute helps map an expected Dictionary KVP Enun
    /// Dictionary<TEnum, T>,
    /// </summary>
    public class EnumKeyDictionaryConverter<TEnum,T> : JsonConverter where TEnum : struct, Enum
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<TEnum, T>);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var temp = serializer.Deserialize<Dictionary<string, string>>(reader);

            var result = new Dictionary<TEnum, T>();

            foreach (var kvp in temp)
            {
                if(Enum.TryParse<TEnum>(kvp.Key,true,out var enumKey))
                {
                    result[enumKey] = (T)Convert.ChangeType(kvp.Value, typeof(T));
                }
            }
            
            return result;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var dict = (Dictionary<TEnum, T>?)value;
            var temp = dict?.ToDictionary(k => k.Key.ToString(), v => v.Value);
            
            serializer.Serialize(writer, temp);
        }
    }
}
