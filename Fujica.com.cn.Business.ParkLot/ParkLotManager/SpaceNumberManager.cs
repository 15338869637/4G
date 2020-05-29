/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 收费设置(模板)管理 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
 using Fujica.com.cn.Context.Model;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    ///  停车场管理器
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 新增注释信息. llp <br/> 
    /// </remarks>   
    partial class ParkLotManager
    {
        /// <summary>
        /// 获取车场的剩余车位数
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public SpaceNumberModel GetRemainingSpace(string parkcode)
        {
            SpaceNumberModel model = _iSpaceNumberContext.GetRemainingSpace(parkcode);
            return model;
        }


        /// <summary>
        /// 设置剩余车位数
        /// </summary>
        /// <returns></returns>
        public bool SetRemainingSpace(SpaceNumberModel model)
        {
            bool redisResult = _iSpaceNumberContext.SetRemainingSpace(model);
            return redisResult;
        }
    }
}
