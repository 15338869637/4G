using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.RedisCache
{
    /// <summary>
    /// 仅在IBaseRedisOperate接口无法满足数据库操作的情况下添加方法与实现
    /// 该接口不需要注册到AutoFac容器中
    /// </summary>
    public interface IExtenRedisOperate<IBaseModel>
    {
        /// <summary>
        /// Key与GUID关联
        /// </summary>
        /// <returns></returns>
        bool KeyLinkGuid();

        /// <summary>
        /// 通过Key获取对象模型
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        IBaseModel GetModelByKey();

        bool KeyUnLinkGuid();
    }
}
