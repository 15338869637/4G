using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.RedisCache
{
    public interface IBaseRedisOperate<IBaseModel> //where IBaseModel : new()
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        IBaseModel model { get; set; }

        /// <summary>
        /// 将对象保存到redis
        /// </summary>
        /// <returns></returns>
        bool SaveToRedis();

        /// <summary>
        /// 从redis读出对象
        /// </summary>
        /// <returns></returns>
        IBaseModel GetFromRedis();

        /// <summary>
        /// 在redis中删除对象
        /// </summary>
        /// <returns></returns>
        bool DeleteInRedis();

        /// <summary>
        /// 从redis读出所有数据
        /// </summary>
        /// <returns></returns>
        IList<IBaseModel> GetAllFromRedis();
    }
}
