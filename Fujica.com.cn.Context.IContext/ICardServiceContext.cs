using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 卡务管理
    /// </summary>
    public interface ICardServiceContext
    {
        /// <summary>
        /// 开卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddCard(CardServiceModel model, CarTypeEnum carType);

        /// <summary>
        /// 修改卡信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyCard(CardServiceModel model, CarTypeEnum carType);

        /// <summary>
        /// 锁定 禁止出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool LockedCard(CardServiceModel model, CarTypeEnum carType);

        /// <summary>
        /// 解锁 可以出场、入场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UnLockedCard(CardServiceModel model, CarTypeEnum carType);

        /// <summary>
        /// 注销 从数据库删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteCard(CardServiceModel model, CarTypeEnum carType);

        /// <summary>
        ///  某车场的所有卡 调用时根据业务筛选月卡，储值卡
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        List<CardServiceModel> AllTypeCard(string parkcode, CarTypeEnum carType);

        /// <summary>
        /// 获取分页的数据
        /// </summary>
        /// <param name="model">查询条件实体</param>
        /// <param name="pageIndex">当前页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="totalCount">总条数</param>
        /// <returns></returns>
        List<CardServiceModel> GetCardPage(CardServiceSearchModel model, CarTypeEnum carType);

        /// <summary>
        /// 获取某卡
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        CardServiceModel GetCard(string carNo, string parkCode, CarTypeEnum carType); 
    }
}
