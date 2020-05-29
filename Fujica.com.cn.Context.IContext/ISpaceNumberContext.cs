using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 停车场车位管理
    /// </summary>
    public interface ISpaceNumberContext
    {
        /// <summary>
        /// 获取车场的剩余车位数
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        SpaceNumberModel GetRemainingSpace(string parkcode);


        /// <summary>
        /// 设置剩余车位数
        /// </summary>
        /// <returns></returns>
        bool SetRemainingSpace(SpaceNumberModel model);
    }
}
