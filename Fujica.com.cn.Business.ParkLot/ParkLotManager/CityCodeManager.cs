
/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 城市区号管理 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using System;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 城市区号管理.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    partial class ParkLotManager
    {
        /// <summary>
        /// 添加城市
        /// </summary>
        /// <returns></returns>
        public bool AddCityCode(string cityID)
        {
            CityCodeModel model = new CityCodeModel();
            model.CodeID = cityID;

            bool redisResult = _iCityCodeContext.AddCityCode(model);

            //redis执行不成功，直接返回false
            if (!redisResult) return false;
            
            AddCityCodeToMq(model);

            return redisResult;
        }

        /// <summary>
        /// 将新增城市区号信息发送至MQ
        /// </summary>
        /// <param name="cityCode"></param>
        /// <returns></returns>
        private bool AddCityCodeToMq(CityCodeModel model)
        {
            CommandEntity<CityCodeModel> entity = new CommandEntity<CityCodeModel>()
            {
                command = BussineCommand.NewCityCode,
                idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                message = model
            };
            return m_rabbitMQ.SendMessageForRabbitMQ("发送新增城市区号命令", m_serializer.Serialize(entity), entity.idMsg, "", "NewCityCode4007004008", "FuJiCaDynamicAddNewCityCode.direct");
        }

    }
}
