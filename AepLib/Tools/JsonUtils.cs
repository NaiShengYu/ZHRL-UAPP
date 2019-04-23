using Newtonsoft.Json;
using System;

namespace AepApp.Tools
{
    public class JsonUtils
    {
        //数据解析
        public static T DeserializeObject<T>(string result)
        {
            T t = default(T);
            try
            {
                t = JsonConvert.DeserializeObject<T>(result);
            }
            catch (Exception)
            {
            }
            return t;

        }
    }
}
