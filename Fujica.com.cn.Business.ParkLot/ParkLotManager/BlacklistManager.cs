using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 黑名单管理
    /// </summary>
    partial class ParkLotManager
    {
        /// <summary>
        /// 添加黑名单
        /// </summary>
        /// <returns></returns>
        public bool AddNewBlacklist(BlacklistModel model)
        {
            bool flag = _iBlacklistContext.AddBlacklist(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BLACK.GetDesc();
                return false;
            }
            return SendTrafficRestriction(model);
        }

        /// <summary>
        /// 修改黑名单
        /// </summary>
        /// <returns></returns>
        public bool ModifyBlacklist(BlacklistModel model)
        {
            bool flag = _iBlacklistContext.ModifyBlacklist(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_BLACK.GetDesc();
                return false;
            }
            return SendTrafficRestriction(model);
        }

        /// <summary>
        /// 删除黑名单
        /// </summary>
        /// <returns></returns>
        public bool DeleteBlacklist(BlacklistModel model)
        {
            bool flag = _iBlacklistContext.DeleteBlacklist(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_BLACK.GetDesc();
                return false;
            }
            //删除的时候，将Enable设为不启用，以便发送给相机的数据进行处理
            foreach (var item in model.List)
            {
                item.Enable = false;
            }
            return SendTrafficRestriction(model);
        }

        /// <summary>
        /// 获取黑名单
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        public BlacklistModel GetBlacklist(string parkcode)
        {
           return _iBlacklistContext.GetBlacklist(parkcode);
        }

        /// <summary>
        /// 下发黑名单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool SendTrafficRestriction(BlacklistModel models)
        {
            bool result = false;
            try
            {
                List<BlacklistDetialModel> sendmodel = new List<BlacklistDetialModel>();
                foreach (var model in models.List)
                {
                    BlacklistDetialModel detialmodel = new BlacklistDetialModel();
                    detialmodel.CarNo = model.CarNo;
                    detialmodel.Always = model.Always;
                    detialmodel.IsDelete = model.Enable ? false : true;
                    if (!model.Always)
                    {
                        if (model.ByDate)
                        {
                            DateTime startDt = DateTime.MinValue;
                            DateTime.TryParse(model.StartDate, out startDt);
                            DateTime endDt = DateTime.MinValue;
                            DateTime.TryParse(model.EndDate, out endDt);
                            detialmodel.DateInterval = startDt.ToString("yyMMdd") + endDt.ToString("yyMMdd");
                        }
                        if (model.ByTime)
                        {
                            DateTime startTime = DateTime.MinValue;
                            DateTime.TryParse(model.StatrtTime, out startTime);
                            DateTime endTime = DateTime.MinValue;
                            DateTime.TryParse(model.EndTime, out endTime);
                            detialmodel.TimeInterval = startTime.ToString("HHmmss") + endTime.ToString("HHmmss");
                        }
                        if (model.ByWeek)
                            detialmodel.WeekInterval = model.AssignDay;
                    }
                    sendmodel.Add(detialmodel);
                }
               
                CommandEntity<List<BlacklistDetialModel>> entity = new CommandEntity<List<BlacklistDetialModel>>()
                {
                    command = BussineCommand.BlackList,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = sendmodel
                };
                result = m_rabbitMQ.SendMessageForRabbitMQ("发送黑名单指令", m_serializer.Serialize(entity), entity.idMsg, models.ParkCode);
                if (!result)
                    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return result;
            }
            catch (Exception ex)
            { 
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", models.ParkCode, "", "Fujica.com.cn.Business.ParkLot.BlacklistManager.SendTrafficRestriction", "下发黑名单时发生异常", ex.ToString());
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_MQ_SEND_ERROR.GetDesc();
                return false;
            }
        }


        /// <summary>
        /// 下发黑名单
        /// </summary>
        /// <param name="parkingCode"></param>
        /// <returns></returns>
        public bool SendTrafficRestriction(string parkCode)
        {
            bool flag = true;
            BlacklistModel models = GetBlacklist(parkCode);
            if (models != null) 
            {
                flag= SendTrafficRestriction(models);
            }
            return flag;
        }

    }
}
