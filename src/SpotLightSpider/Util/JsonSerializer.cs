using Newtonsoft.Json;

namespace SpotLightSpider.Util
{
    /// <summary>
    /// Json序列化和反序列化工具
    /// </summary>
    public static class JsonSerializer
    {
        private const string ERROR_SERIALIZE = "序列化失败";               //错误-序列化
        private const string ERROR_DESERIALIZE = "反序列化失败";           //错误-反序列化

        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="json">序列化的 JSON 字符串。</param>
        /// <param name="error">错误信息。</param>
        /// <returns>成功返回true,否则返回false。</returns>
        public static bool Serialize(object obj, out string json, out string error)
        {
            //序列化
            try
            {
                json = obj == null ? null : JsonConvert.SerializeObject(obj);
                error = null;
                return true;
            }
            catch
            {
            }
            //序列化失败
            json = null;
            error = ERROR_SERIALIZE;
            return false;
        }

        /// <summary>
        /// 将指定的 JSON 字符串转换为 T 类型的对象。
        /// </summary>
        /// <typeparam name="T">所生成对象的类型。</typeparam>
        /// <param name="json">要进行反序列化的 JSON 字符串。</param>
        /// <returns>反序列化的对象。</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 将指定的 JSON 字符串转换为 T 类型的对象。
        /// </summary>
        /// <typeparam name="T">所生成对象的类型。</typeparam>
        /// <param name="json">要进行反序列化的 JSON 字符串。</param>
        /// <param name="obj">反序列化的对象。</param>
        /// <param name="error">错误信息。</param>
        /// <returns>成功返回true,否则返回false。</returns>
        public static bool Deserialize<T>(string json, out T obj, out string error)
        {
            //反序列化
            try
            {
                obj = JsonConvert.DeserializeObject<T>(json);
                error = null;
                return true;
            }
            catch
            {
            }
            //反序列化失败
            obj = default(T);
            error = ERROR_DESERIALIZE;
            return false;
        }
    }
}
