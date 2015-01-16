﻿using Newtonsoft.Json;

namespace LibInfluxDB.Net
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        public static T ReadAs<T>(this InfluxDbApiResponse response)
        {
            var @object = JsonConvert.DeserializeObject<T>(response.Body);
            return @object;
        }
    }
}