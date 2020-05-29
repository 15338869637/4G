using System;
using System.Security.Cryptography;
using System.Text;
/***************************************************************************************
 * *
 * *        File Name        : Signature.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 公钥验证签名
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Tools
{
    public class Signature
    {
        /// <summary>
        /// 公钥验证签名.
        /// </summary>
        /// <remarks>
        /// 2019.09.21: 修改:日志参数完善. llp <br/> 
        /// </remarks>   
        public static bool verify(string content, string signedString, string publicKey, string input_charset)
        {
            bool result = false;

            Encoding code = Encoding.GetEncoding(input_charset);
            byte[] Data = code.GetBytes(content);
            byte[] data = Convert.FromBase64String(signedString);
            RSAParameters paraPub = ConvertFromPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.ImportParameters(paraPub);
            SHA1 sh = new SHA1CryptoServiceProvider();
            result = rsaPub.VerifyData(Data, sh, data);
            return result;
        }

        /// <summary>
        /// 转公钥参数
        /// </summary>
        /// <param name="pemFileConent"></param>
        /// <returns></returns>
        private static RSAParameters ConvertFromPublicKey(string pemFileConent)
        {

            byte[] keyData = Convert.FromBase64String(pemFileConent);
            if (keyData.Length < 162)
            {
                throw new ArgumentException("pem file content is incorrect.");
            }
            byte[] pemModulus = new byte[128];
            byte[] pemPublicExponent = new byte[3];
            Array.Copy(keyData, 29, pemModulus, 0, 128);
            Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
            RSAParameters para = new RSAParameters();
            para.Modulus = pemModulus;
            para.Exponent = pemPublicExponent;
            return para;
        }
    }
}
