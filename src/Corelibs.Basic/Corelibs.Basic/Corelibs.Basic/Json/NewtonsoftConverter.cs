using Newtonsoft.Json;

namespace Corelibs.Basic.Json
{
    public class NewtonsoftJsonConverter : IJsonConverter
    {
        T IJsonConverter.Deserialize<T>(string json)
        {
            try
            {
                var jsonSettings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };

                return JsonConvert.DeserializeObject<T>(json, jsonSettings);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return default;
            }
        }

        string IJsonConverter.Serialize<T>(T obj)
        {
            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            try
            {
                return JsonConvert.SerializeObject(obj, jsonSettings);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return default;
            }
        }
    }
}
