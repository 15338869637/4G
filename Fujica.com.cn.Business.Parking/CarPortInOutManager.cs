using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Parking
{
    /// <summary>
    /// 车位进出类
    /// </summary>
    public class CarPortInOutManager
    {
        /// <summary>
        /// 入位
        /// </summary>
        /// <returns></returns>
        public bool Enter(DateTime inTime, string parkCode, string carNo, string cardNo = "")
        {
            return false;
        }

        /// <summary>
        /// 出位
        /// </summary>
        /// <returns></returns>
        public bool Exit(DateTime outTime, string parkCode, string carNo, string cardNo = "")
        {
            return false;
        }
    }
}
