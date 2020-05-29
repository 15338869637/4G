using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;

namespace Fujica.com.cn.Web.Manager
{
    public class RequestFujicaInterface
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="methodname">请求方法名</param>
        /// <param name="servername">服务名</param>
        /// <param name="argument">参数</param>
        /// <returns></returns>
        public bool GetRequest(string methodname, string servername, string arguments)
        {
            try
            {
                string timestamp = Convert.ToInt64(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString(); //时间戳
                string sign = Sign.GetSignContent(string.Format("param={0}&secret={1}&timestamp={2}",
                    arguments, ConfigurationManager.AppSettings["secret"], timestamp),
                    ConfigurationManager.AppSettings["privateKey"]);
                HttpHelper.Headers.Clear();
                HttpHelper.Headers.Add("appid", ConfigurationManager.AppSettings["appid"]);
                HttpHelper.Headers.Add("sign", sign);
                HttpHelper.Headers.Add("timestamp", timestamp);

                string result = "{\"IsSuccess\":\"false\",\"result\":\"未进入任何post,get,put请求\"}";

                if (methodname.Equals("post", StringComparison.OrdinalIgnoreCase))
                {
                    result = HttpHelper.HttpPost(ConfigurationManager.AppSettings["apiurl"] + servername, arguments);
                }else if (methodname.Equals("get", StringComparison.OrdinalIgnoreCase))
                {
                    result = HttpHelper.HttpGet(ConfigurationManager.AppSettings["apiurl"] + servername, arguments);
                }
                else if (methodname.Equals("put", StringComparison.OrdinalIgnoreCase))
                {
                    result = HttpHelper.HttpPut(ConfigurationManager.AppSettings["apiurl"] + servername, arguments);
                }

                Dictionary<string, object> resultdic = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result);
                if ((bool)resultdic["IsSuccess"])
                {
                    this.LastResult = result;
                }
                else
                {
                    //不需要记录日志，API已经记录了
                    //throw new Exception(string.Format("接口执行不成功,请求接口:{0},请求字符串:{1},原始返回结果:{2}", servername, new JavaScriptSerializer().Serialize(arguments), result));
                    this.LastResult = result;
                }
                return true;
            }
            catch (Exception ex)
            {
                //不需要记录日志，API已经记录了
                //throw new Exception(string.Format("请求业务抛出异常，请求接口:{0}", servername), ex);
            }
            return false;
        }

        /// <summary>
        /// 接口请求后返回的原始字符串
        /// </summary>
        public string LastResult { get; private set; }
    }

    internal static class Sign
    {
        public static string GetSignContent(string content, string privateKey, string input_charset = "UTF-8")
        {
            byte[] res = Convert.FromBase64String(privateKey);
            RSACryptoServiceProvider rsaCsp = DecodeRSAPrivateKey(res);
            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(input_charset))
            {
                dataBytes = Encoding.UTF8.GetBytes(content);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(input_charset).GetBytes(content);
            }
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }

        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // ---------  Set up stream to decode the asn.1 encoded RSA private key  ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)	//version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------  all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)		//expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();	// data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }



            while (binr.ReadByte() == 0x00)
            {	//remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);		//last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }
    }

    public class HttpHelper
    {
        private static WebHeaderCollection _headers = new WebHeaderCollection();

        /// <summary>
        /// 请求的头
        /// </summary>
        public static WebHeaderCollection Headers
        {
            get
            {
                return _headers;
            }
        }

        /// <summary>
        /// 设置http请求头
        /// </summary>
        private static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }

        /// <summary>
        /// POST 默认utf8格式提交
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, string postDataStr, string contentType = "application/json;charset=UTF-8", bool isUseCert = false, string sslcertpath = "", string sslcertpassword = "")
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string strValue = "";
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                string strURL = Url;
                request = (HttpWebRequest)WebRequest.Create(strURL);
                if (Headers != null) request.Headers = Headers;
                request.Method = "POST";
                request.ContentType = contentType;
                request.AllowAutoRedirect = false;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                request.Proxy = null;
                string paraUrlCoded = postDataStr;
                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paraUrlCoded);
                request.ContentLength = payload.Length;
                //是否使用证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + sslcertpath, sslcertpassword);
                    request.ClientCertificates.Add(cert);
                }
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

        /// <summary>
        /// PUT 默认utf8格式提交
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postDataStr"></param>
        /// <returns></returns>
        public static string HttpPut(string Url, string postDataStr, string contentType = "application/json;charset=UTF-8", bool isUseCert = false, string sslcertpath = "", string sslcertpassword = "")
        {
            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string strValue = "";
            try
            {
                //设置最大连接数
                ServicePointManager.DefaultConnectionLimit = 200;
                //设置https验证方式
                if (Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                            new RemoteCertificateValidationCallback(CheckValidationResult);
                }

                string strURL = Url;
                request = (HttpWebRequest)WebRequest.Create(strURL);
                if (Headers != null) request.Headers = Headers;
                request.Method = "PUT";
                request.ContentType = contentType;
                request.AllowAutoRedirect = false;
                request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
                request.Proxy = null;
                string paraUrlCoded = postDataStr;
                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paraUrlCoded);
                request.ContentLength = payload.Length;
                //是否使用证书
                if (isUseCert)
                {
                    string path = HttpContext.Current.Request.PhysicalApplicationPath;
                    X509Certificate2 cert = new X509Certificate2(path + sslcertpath, sslcertpassword);
                    request.ClientCertificates.Add(cert);
                }
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

        /// <summary>
        /// GET 返回utf8格式
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="urlParamStr"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string urlParamStr = "")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (urlParamStr == "" ? "" : "?") + urlParamStr);
            if (Headers != null) request.Headers = Headers;
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        #region 取客户端真实IP
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            var ipAddress = string.Empty;
            try
            {
                var list = HttpContext.Current.Request.Headers.GetValues("X-Real-IP");
                ipAddress = ((string[])list)[0].Split(',')[0];
            }
            catch
            {
                ipAddress = GetIPAddress;
            }
            return ipAddress;
        }
        ///  <summary>    
        ///  取得客户端真实IP。如果有代理则取第一个非内网地址    
        ///  </summary>    
        public static string GetIPAddress
        {
            get
            {
                var result = string.Empty;

                try
                {
                    result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (!string.IsNullOrEmpty(result))
                    {
                        //可能有代理    
                        if (result.IndexOf(".") == -1)        //没有“.”肯定是非IPv4格式    
                            result = null;
                        else
                        {
                            if (result.IndexOf(",") != -1)
                            {
                                //有“,”，估计多个代理。取第一个不是内网的IP。    
                                result = result.Replace("  ", "").Replace("'", "");
                                string[] temparyip = result.Split(",;".ToCharArray());
                                for (int i = 0; i < temparyip.Length; i++)
                                {
                                    if (IsIPAddress(temparyip[i])
                                            && temparyip[i].Substring(0, 3) != "10."
                                            && temparyip[i].Substring(0, 7) != "192.168"
                                            && temparyip[i].Substring(0, 7) != "172.16.")
                                    {
                                        return temparyip[i];        //找到不是内网的地址    
                                    }
                                }
                            }
                            else if (IsIPAddress(result))  //代理即是IP格式    
                                return result;
                            else
                                result = null;        //代理中的内容  非IP，取IP    
                        }

                    }

                    string IpAddress = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                    if (string.IsNullOrEmpty(result))
                        result = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];

                    if (string.IsNullOrEmpty(result))
                        result = HttpContext.Current.Request.UserHostAddress;
                }
                catch (Exception ex)
                {
                    throw new Exception("获取终端IP抛出异常", ex);
                }

                return result;
            }
        }
        ///  <summary>  
        ///  判断是否是IP地址格式  0.0.0.0  
        ///  </summary>  
        ///  <param  name="str1">待判断的IP地址</param>  
        ///  <returns>true  or  false</returns>  
        public static bool IsIPAddress(string str1)
        {
            if (string.IsNullOrEmpty(str1) || str1.Length < 7 || str1.Length > 15) return false;

            const string regFormat = @"^d{1,3}[.]d{1,3}[.]d{1,3}[.]d{1,3}$";

            var regex = new Regex(regFormat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str1);
        }
        #endregion
    }

    public class HttpCacheHelper
    {
        /// <summary>
        /// 线程安全锁
        /// </summary>
        private static object asyncLock = new object();

        /// <summary>
        /// 删除缓存。
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="isDeleteSrcCache">是否删除源值缓存（False=仅删除状态缓存）</param>
        public static void DelCache(string cacheName, bool isDeleteSrcCache)
        {
            HttpRuntime.Cache.Remove(GetStateCacheKey(cacheName));
            if (isDeleteSrcCache)
            {
                HttpRuntime.Cache.Remove(cacheName);
            }
        }

        /// <summary>
        /// 获取缓存对象。
        /// <para>如果返回的缓存对象不为 Null，则表示缓存可用，</para>
        /// <para>否则，请调用者自数据库或其它位置获取对象，并设置该缓存。</para>
        /// <para></para>
        /// <para>应用实例：</para>
        /// <para>string cacheName = "UserList";</para>
        /// <para>DataTable dtUserList = CacheHelper.GetCache(cacheName) as DataTable;</para>
        /// <para>if (dtUserList == null)</para>
        /// <para>{</para>
        /// <para>    dtUserList = GetUserListByDB();</para>
        /// <para>    // 不会造成循环失效，因为 SetCache 方法仅允许第一个 GetCache 时返回空的进程设置缓存。</para>
        /// <para>    CacheHelper.SetCache(cacheName, dtUserList);</para>
        /// <para>}</para>
        /// <para>return dtUserList;</para>
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <returns></returns>
        public static object GetCache(string cacheName)
        {
            if (HttpRuntime.Cache[GetStateCacheKey(cacheName)] == null)
            {
                lock (asyncLock)
                {
                    if (HttpRuntime.Cache[GetStateCacheKey(cacheName)] == null)
                    {
                        SetCacheLoading(cacheName);
                        return null;
                    }
                }
            }
            return HttpRuntime.Cache[cacheName];
        }

        public static T GetCache<T>(string cacheName)
        {
            if (HttpRuntime.Cache[GetStateCacheKey(cacheName)] == null)
            {
                lock (asyncLock)
                {
                    if (HttpRuntime.Cache[GetStateCacheKey(cacheName)] == null)
                    {
                        SetCacheLoading(cacheName);
                        return default(T);
                    }
                }
            }
            return (T)HttpRuntime.Cache[cacheName];
        }

        /// <summary>
        /// 获取状态缓存的键名
        /// </summary>
        /// <param name="cacheName">缓存名称</param>
        /// <returns></returns>
        private static string GetStateCacheKey(string cacheName)
        {
            return string.Format("{0}{1}", cacheName, "_Dep");
        }

        /// <summary>
        /// 是否忽略缓存设置操作。
        /// </summary>
        /// <param name="cacheName">缓存名称</param>
        /// <returns>状态缓存为空或状态缓存值为当前线程ID时，返回 False，代表不忽略缓存设置操作。</returns>
        private static bool IsIgnoreCacheSet(string cacheName)
        {
            return ((HttpRuntime.Cache[GetStateCacheKey(cacheName)] != null) && (((int)HttpRuntime.Cache[GetStateCacheKey(cacheName)]) != Thread.CurrentThread.ManagedThreadId));
        }

        /// <summary>
        /// 设置无过期缓存。
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        public static void SetCache(string cacheName, object val)
        {
            if (val != null)
            {
                if (GetCache(cacheName) != null)
                {
                    DelCache(cacheName, true);
                }
                HttpRuntime.Cache.Insert(cacheName, val);
                CacheDependency dependencies = new CacheDependency(null, new string[] { cacheName });
                HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies);
            }
        }

        /// <summary>
        /// 设置文件依赖缓存。
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="file">缓存依赖的文件</param>
        public static void SetCache(string cacheName, object val, string file)
        {
            if (GetCache(cacheName) != null)
            {
                DelCache(cacheName, true);
            }
            SetCache(cacheName, val, new string[] { file });
        }

        /// <summary>
        /// 设置文件依赖缓存。
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="files">缓存依赖的文件组</param>
        public static void SetCache(string cacheName, object val, string[] files)
        {
            if (((val != null) && (files.Length != 0)) && !IsIgnoreCacheSet(cacheName))
            {
                if (GetCache(cacheName) != null)
                {
                    DelCache(cacheName, true);
                }
                HttpRuntime.Cache.Insert(cacheName, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(12, 0, 0), CacheItemPriority.High, null);
                CacheDependency dependencies = new CacheDependency(files, new string[] { cacheName });
                HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies);
            }
        }

        /// <summary>
        /// 设置时间过期缓存（滑动过期或绝对过期）。
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="cacheTime">要缓存的时长（分钟）</param>
        /// <param name="isSlidingTime">是否为滑动过期</param>
        public static void SetCache(string cacheName, object val, int cacheTime, bool isSlidingTime)
        {
            if (((val != null) && (cacheTime >= 1)) && !IsIgnoreCacheSet(cacheName))
            {
                if (isSlidingTime)
                {
                    HttpRuntime.Cache.Insert(cacheName, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime * 2, 0));
                }
                else
                {
                    HttpRuntime.Cache.Insert(cacheName, val, null, DateTime.Now.AddMinutes((double)(cacheTime * 2)), TimeSpan.Zero);
                }
                CacheDependency dependencies = new CacheDependency(null, new string[] { cacheName });
                if (isSlidingTime)
                {
                    HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime, 0));
                }
                else
                {
                    HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies, DateTime.Now.AddMinutes((double)cacheTime), TimeSpan.Zero);
                }
            }
        }

        /// <summary>
        /// 设置无过期缓存。（当前线程并非最初检测到缓存过期的线程）
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        public static void SetCacheAsync(string cacheName, object val)
        {
            if (val != null)
            {
                HttpRuntime.Cache.Insert(cacheName, val);
                CacheDependency dependencies = new CacheDependency(null, new string[] { cacheName });
                HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies);
            }
        }

        /// <summary>
        /// 设置文件依赖缓存。（当前线程并非最初检测到缓存过期的线程）
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="files">缓存依赖的文件</param>
        public static void SetCacheAsync(string cacheName, object val, string[] files)
        {
            if ((val != null) && (files.Length != 0))
            {
                foreach (string str in files)
                {
                    if (!File.Exists(str))
                    {
                        if (!Directory.Exists(Directory.GetParent(str).ToString()))
                        {
                            Directory.CreateDirectory(Directory.GetParent(str).ToString());
                        }
                        File.Create(str).Close();
                    }
                }
                HttpRuntime.Cache.Insert(cacheName, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(12, 0, 0), CacheItemPriority.High, null);
                CacheDependency dependencies = new CacheDependency(files, new string[] { cacheName });
                HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies);
            }
        }

        /// <summary>
        /// 设置文件依赖缓存。（当前线程并非最初检测到缓存过期的线程）
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="file">缓存依赖的文件组</param>
        public static void SetCacheAsync(string cacheName, object val, string file)
        {
            SetCacheAsync(cacheName, val, new string[] { file });
        }

        /// <summary>
        /// 设置时间过期缓存（滑动过期或绝对过期）。（当前线程并非最初检测到缓存过期的线程）
        /// </summary>
        /// <param name="cacheName">缓存的名称</param>
        /// <param name="val">要缓存的对象</param>
        /// <param name="cacheTime">要缓存的时长（分钟）</param>
        /// <param name="isSlidingTime">是否为滑动过期</param>
        public static void SetCacheAsync(string cacheName, object val, int cacheTime, bool isSlidingTime)
        {
            if ((val != null) && (cacheTime >= 1))
            {
                if (isSlidingTime)
                {
                    HttpRuntime.Cache.Insert(cacheName, val, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime * 2, 0));
                }
                else
                {
                    HttpRuntime.Cache.Insert(cacheName, val, null, DateTime.Now.AddMinutes((double)(cacheTime * 2)), TimeSpan.Zero);
                }
                CacheDependency dependencies = new CacheDependency(null, new string[] { cacheName });
                if (isSlidingTime)
                {
                    HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies, Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime, 0));
                }
                else
                {
                    HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), 0, dependencies, DateTime.Now.AddMinutes((double)cacheTime), TimeSpan.Zero);
                }
            }
        }

        /// <summary>
        /// 设置一个缓存的状态为 Loading 状态，并记录当前进程ID。
        /// <para>一个缓存的 Loading 状态最长可以存在10分钟。</para>
        /// </summary>
        /// <param name="cacheName">缓存名称</param>
        private static void SetCacheLoading(string cacheName)
        {
            HttpRuntime.Cache.Insert(GetStateCacheKey(cacheName), Thread.CurrentThread.ManagedThreadId, null, DateTime.Now.AddMinutes(10.0), TimeSpan.Zero);
        }

    }
}