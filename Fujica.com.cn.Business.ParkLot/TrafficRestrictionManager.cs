/***************************************************************************************
 * *
 * *        File Name        : TrafficRestrictionManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 通行设置管理器 逻辑类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 通行设置管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    public class TrafficRestrictionManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        private readonly RabbitMQSender m_rabbitMQ;

        /// <summary>
        /// 通行设置信息操作上下文
        /// </summary>
        private ITrafficRestrictionContext _iTrafficRestrictionContext = null;

        public string LastErrorDescribe
        {
            get;
            set;
        }

        public TrafficRestrictionManager(ILogger logger, ISerializer serializer,
            RabbitMQSender rabbitMQ,
            ITrafficRestrictionContext iTrafficRestrictionContext)
        {
            m_logger = logger;
            m_serializer = serializer;
            m_rabbitMQ = rabbitMQ;
            _iTrafficRestrictionContext = iTrafficRestrictionContext;
        }

        /// <summary>
        /// 保存通行设置
        /// 并下发mq数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="drivewayList">该停车场所有的车道</param>
        /// <returns></returns>
        public bool SaveTrafficRestriction(TrafficRestrictionModel model, List<DrivewayModel> drivewayList)
        {
            bool flag = _iTrafficRestrictionContext.SaveTrafficRestriction(model);
            if (!flag) return false;

            //下发通行限制
            return SendTrafficRestriction(model, drivewayList);
        }

        /// <summary>
        /// 读取通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public TrafficRestrictionModel GetTrafficRestriction(string guid)
        {
            return _iTrafficRestrictionContext.GetTrafficRestriction(guid);
        }

        /// <summary>
        /// 删除通行设置
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool DeleteTrafficRestriction(TrafficRestrictionModel model, List<DrivewayModel> drivewayList)
        {
            bool flag = _iTrafficRestrictionContext.DeleteTrafficRestriction(model);
            if (!flag) return false;
            model.DrivewayGuid.Clear();
            //下发通行限制
            return SendTrafficRestriction(model, drivewayList);
        }

        /// <summary>
        /// 读取通行设置列表
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public List<TrafficRestrictionModel> GetTrafficRestrictionList(string projectGuid)
        {
            List<TrafficRestrictionModel> list = _iTrafficRestrictionContext.GetTrafficRestrictionList(projectGuid);
            return list;
        }

        /// <summary>
        /// 下发通行限制
        /// </summary>
        /// <param name="model">通行限制模型</param>
        /// <param name="drivewayList">停车场车道集合</param>
        /// <returns></returns>
        public bool SendTrafficRestriction(TrafficRestrictionModel model, List<DrivewayModel> drivewayList)
        {
            bool result = false;
            LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
            try
            {
                //根据车道集合一条一条发送给相机，减少相机计算
                foreach (var drivewayModel in drivewayList)
                {
                    TrafficRestrictionDetialModel sendmodel = new TrafficRestrictionDetialModel()
                    {
                        Guid = model.Guid,
                        CarTypeGuid = model.CarTypeGuid,
                        DeviceIdentify = drivewayModel.DeviceMacAddress,
                        AssignDays = model.AssignDays,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        IsDelete = model.DrivewayGuid.Count(m => m == drivewayModel.Guid) > 0 ? false : true
                    };

                    CommandEntity<TrafficRestrictionDetialModel> entity = new CommandEntity<TrafficRestrictionDetialModel>()
                    {
                        command = BussineCommand.Restrict,
                        idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                        message = sendmodel
                    };
                    result = m_rabbitMQ.SendMessageForRabbitMQ("发送通行限制指令", m_serializer.Serialize(entity), entity.idMsg, model.ParkCode);
                    if (!result)
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, model.Guid, model.ParkCode, "", "Fujica.com.cn.Business.ParkLot.TrafficRestrictionManager.SendTrafficRestriction", "下发通行限制时发生异常", ex.ToString());
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
        }
    }
}
