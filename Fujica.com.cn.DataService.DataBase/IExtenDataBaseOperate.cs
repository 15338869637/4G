using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.DataBase
{
    /// <summary>
    /// 仅在IBaseDataBaseOperate接口无法满足数据库操作的情况下添加方法与实现
    /// 该接口不需要注册到AutoFac容器中
    /// </summary>
    public interface IExtenDataBaseOperate<IBaseModel>
    {
        /// <summary>
        /// 切换值
        /// </summary>
        /// <returns></returns>
        bool ToggleValue(IBaseModel model);

        /// <summary>
        /// 通过key获取guid
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetGuidByKey(string key);

        List<IBaseModel> GetListByKeys(string key);
    }
}
