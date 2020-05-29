using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    ///  城市区号模型
    /// </summary>
    public class CityCodeModel : IBaseModel
    {
        /// <summary>
        /// 城市区号（四位数存储） 
        /// 如0755 0020
        /// </summary>
        public string CodeID { get; set; }
    }
}
