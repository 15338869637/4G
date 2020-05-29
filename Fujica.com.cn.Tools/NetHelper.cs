using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Tools
{
    public class NetHelper
    {
        /// <summary>
        /// 获取IPv4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string hostName = System.Net.Dns.GetHostName();//本机名  
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostAddresses(hostName);
            string ip = addressList.First(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            return ip;
        }
    }
}
