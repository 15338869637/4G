using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 城市区号管理
    /// </summary>
    public interface ICityCodeContext
    {
        /// <summary>
        /// 添加城市
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddCityCode(CityCodeModel model);

    }
}
