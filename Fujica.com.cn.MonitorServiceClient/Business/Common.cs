using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.MonitorServiceClient.Business
{
    public class Common
    {

        /// <summary>
        /// 散列车辆在场数据到不同db(10-15)
        /// </summary>
        /// <returns></returns>
        public static int GetDatabaseNumber(string carNo)
        {
            byte[] byts = Encoding.UTF8.GetBytes(carNo);
            int length = byts.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += byts[i];
            }
            return (sum % 6) + 10; //6代表分配到6个db里,10代表从第10个db开始
        }
    }
}
