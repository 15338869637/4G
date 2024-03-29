﻿using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
/***************************************************************************************
 * *
 * *        File Name        : RequestFujicaMvcApi.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 通用请求后台mvcApi接口
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Tools
{

    /// <summary>
    /// 请求后台mvcApi接口.
    /// </summary>
    /// <remarks>
    /// 2019.09.21: 修改: RequestInterfaceMvc 日志参数完善. llp <br/> 
    /// 2019.10.17: 修改: RequestInterfaceMvc,车编,车牌参数提取,写入日志. llp <br/>  
    /// </remarks>  
    public class RequestFujicaMvcApi
    {


        #region 基础参数
        /// <summary>
        /// 后台mvc接口通用appid
        /// </summary>
        private string Fujica_appid = FujicaMvcApiHelper.GetConfig().Fujica_appid;//fujica_dasdetds3f15det2

        /// <summary>
        /// 后台mvc接口通用secret
        /// </summary>
        private string Fujica_secret = FujicaMvcApiHelper.GetConfig().Fujica_secret;//2f463f6f5e264b8abacfad39efc7d782

        /// <summary>
        /// 后台mvc接口通用privatekey
        /// </summary>
        private string Fujica_privatekey =FujicaMvcApiHelper.GetConfig().Fujica_privatekey;

        /// <summary>
        /// 后台mvc接口请求地址
        /// </summary>
        private string Fujica_url = FujicaMvcApiHelper.GetConfig().Fujica_url;//"http://192.168.15.207:8020/Api/";// 

        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        /// <summary>
        /// 请求富士接口返回的result字符串
        /// </summary>
        public string FujicaResult { get; private set; }

        private HttpHelper HttpHelp;
        #endregion

        public RequestFujicaMvcApi()
        {
            m_logger = new Logger.Logger();
            m_serializer = new JsonSerializer(m_logger);
            HttpHelp = new HttpHelper();
        }

      
        /// <summary>
        /// 请求后台mvc接口
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool RequestInterfaceMvc(string servername, Dictionary<string, object> arguments)
        {
            string parkingCode = ""; //车编
            string carNo = "";//车牌 
            try
            {
                if (arguments.ContainsKey("ParkingCode") && arguments["ParkingCode"] != null)
                {
                    parkingCode = arguments["ParkingCode"].ToString();
                }
                if (arguments.ContainsKey("CarNo") && arguments["CarNo"] != null)
                {
                    carNo = arguments["CarNo"].ToString();
                }
                string jsonstr = m_serializer.Serialize(arguments);
                string timestamp = GenerateTimeStamp();
                string sign = GetSignContentV2(string.Format("param={0}&secret={1}&timestamp={2}", jsonstr, Fujica_secret, timestamp), Fujica_privatekey);

                HttpHelp._headers.Clear();
                HttpHelp.Headers.Add("sign", sign);
                HttpHelp.Headers.Add("appid", Fujica_appid);
                HttpHelp.Headers.Add("timestamp", timestamp);
                string result = HttpHelp.HttpPost(Fujica_url + servername, jsonstr);
                Dictionary<string, object> resultdic = m_serializer.Deserialize<Dictionary<string, object>>(result);
                if ((bool)resultdic["IsSuccess"])
                {
                    this.FujicaResult = result;
                    m_logger.LogInfo(LoggerLogicEnum.Communication, "", parkingCode, carNo, "Fujica.com.cn.Tools.RequestFujicaMvcApi.RequestInterfaceMvc", string.Format("接口执行成功,请求接口:{0},请求字符串:{1},原始返回结果:{2}", servername, m_serializer.Serialize(arguments), result));
                    return true;
                }
                else
                {
                    this.FujicaResult = result;
                    m_logger.LogWarn(LoggerLogicEnum.Communication, "", parkingCode, carNo, "Fujica.com.cn.Tools.RequestFujicaMvcApi.RequestInterfaceMvc", string.Format("接口执行不成功,请求接口:{0},请求字符串:{1},原始返回结果:{2}", servername, m_serializer.Serialize(arguments), result));
                    return false;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Communication, "", parkingCode, carNo, "Fujica.com.cn.Tools.RequestFujicaMvcApi.RequestInterfaceMvc", string.Format("请求业务抛出异常，请求接口:{0},异常信息:{1}", servername, ex.ToString()));
                return false;
            }
        }

        #region  基础方法

        /// <summary>
        /// 生成时间戳，标准北京时间，时区为东八区，自1970年1月1日 0点0分0秒以来的秒数
        /// </summary>
        /// <returns>时间戳</returns>
        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private string GetSignContentV2(string content, string privateKey, string input_charset = "utf-8")
        {
            string str = privateKey.Replace("\n", "").Replace("\\n","");
            byte[] res = Convert.FromBase64String(str);
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

        private RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
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
            catch (Exception)
            {
                return null;
            }
            finally { binr.Close(); }
        }

        private int GetIntegerSize(BinaryReader binr)
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

        #endregion
    }
}
