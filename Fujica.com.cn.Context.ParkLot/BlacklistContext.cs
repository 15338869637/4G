using Fujica.com.cn.Context.IContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.DataService.DataBase;

namespace Fujica.com.cn.Context.ParkLot
{
    /// <summary>
    /// 黑名单管理
    /// </summary>
    public class BlacklistContext : IBasicContext, IBlacklistContext
    {
        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<BlacklistModel> databaseoperate = null;

        public BlacklistContext(IBaseDataBaseOperate<BlacklistModel> _databaseoperate)
        {
            databaseoperate = _databaseoperate;
        }

        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddBlacklist(BlacklistModel model)
        {
            bool flag = databaseoperate.SaveToDataBase(model); 
            return flag;
        }

        /// <summary>
        /// 修改黑名单
        /// </summary>
        /// <returns></returns>
        public bool ModifyBlacklist(BlacklistModel model)
        {
            bool flag = databaseoperate.SaveToDataBase(model); 
            return flag;
        }

        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteBlacklist(BlacklistModel model)
        {
            bool flag = databaseoperate.DeleteInDataBase(model);
            return flag;
        }


        /// <summary>
        /// 获取某黑名单
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public BlacklistModel GetBlacklist(string parkcode)
        {

            BlacklistModel model = new BlacklistModel();
            try
            {  //只从数据库读，redis并不缓存此实体
                model = databaseoperate.GetFromDataBase(parkcode);
                model.List.OrderByDescending(a => a.StartDate).ToList();//降序
                return  model;
            }
            catch (Exception ex)
            {
                return null;
            }

           
        }

       


    }
}
