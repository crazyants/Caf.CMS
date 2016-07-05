using Newtonsoft.Json;

namespace CAF.Mvc.JQuery.Datatables.Core
{
    public static class SerializerHelper
    {
        public static string ToSerializerIgnoreNullValue(this object result)
        {
            return JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static string ToSerializer(this object result)
        {
            return JsonConvert.SerializeObject(result);
        }
    }

    
}