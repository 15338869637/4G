using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fujica.com.cn.Tools
{
    public class HttpHelper
    {
        public WebHeaderCollection _headers = new WebHeaderCollection();

        /// <summary>
        /// 请求的头
        /// </summary>
        public WebHeaderCollection Headers
        {
            get
            {
                return _headers;
            }
        }
        
        /// <summary>
        /// POST 默认utf8格式提交
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public string HttpPost(string Url, string postDataStr, string contentType = "application/json;charset=UTF-8")
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string strValue = "";
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                string strURL = Url;
                request = (HttpWebRequest)WebRequest.Create(strURL);
                if (Headers != null) request.Headers = Headers;
                request.Timeout = 10000;
                request.ReadWriteTimeout = 10000;
                request.Method = "POST";
                request.ContentType = contentType;
                request.AllowAutoRedirect = false;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                request.Proxy = null;
                string paraUrlCoded = postDataStr;
                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paraUrlCoded);
                request.ContentLength = payload.Length;
               
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();
                response = (System.Net.HttpWebResponse)request.GetResponse();
                string StrDate = "";
                StreamReader Reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }
                //strValue = Reader.ReadToEnd().Trim();
                Reader.Close();
                Reader.Dispose();
            }
            catch (ThreadAbortException e)
            {
                Thread.ResetAbort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return strValue;
        }

    }
}
