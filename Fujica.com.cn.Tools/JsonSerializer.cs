using Fujica.com.cn.Logger;
using Newtonsoft.Json;
using System;

namespace Fujica.com.cn.Tools
{
    public interface ISerializer
    {
        /// <summary>
        /// 将字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        T Deserialize<T>(string text);
        /// <summary>
        /// 将对象序列化为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);
    }
    public class JsonSerializer: ISerializer
    {
        ILogger m_logger = null;

        public JsonSerializer(ILogger logger)
        {
            JsonSerializerSettings setting =
                new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-dd HH:mm:ss",
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
            JsonConvert.DefaultSettings = () => { return setting; };

            m_logger = logger;
        }

        /// <summary>
        /// 将对象序列化为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            if (obj == null) return null;

            try
            {
                string jsonString = JsonConvert.SerializeObject(obj);
                return jsonString;
            }
            catch (Exception ex)
            {
                var stack = new System.Diagnostics.StackTrace();
                string actionName = stack.GetFrame(1).GetMethod().Name;
                string className = stack.GetFrame(1).GetMethod().ReflectedType.Name;
                m_logger.LogUtils("Fujica.com.cn.Tools.JsonSerializer",
                    string.Format("Json序列化出现异常，className：{0}；actionName：{1};参数:{2}",
                    className, actionName, obj.GetType()), ex);
                throw ex;
            }
        }

        /// <summary>
        /// 将字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <summary>
        /// Json反序列化
        /// </summary>
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(json);
                return obj;
            }
            catch (Exception ex)
            {
                var stack = new System.Diagnostics.StackTrace();
                string actionName = stack.GetFrame(1).GetMethod().Name;
                string className = stack.GetFrame(1).GetMethod().ReflectedType.Name;
                m_logger.LogUtils("Fujica.com.cn.Tools.JsonSerializer",
                    string.Format("Json反序列化出现异常，className：{0}；actionName：{1};参数:{2}",
                    className, actionName, json), ex);
                return default(T);
            }
        }
    }
}
