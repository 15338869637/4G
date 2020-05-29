using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Business.Toll;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.PayDataService.Model.Fujica;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.PayDataService
{
    /// <summary>
    /// 监听付款数据
    /// </summary>
    public class MonitorPayData
    {
        public static void Execute()
        {
            Task.Factory.StartNew(() =>
            {
                ILogger m_ilogger = new Logger.Logger();
                ISerializer m_serializer = new JsonSerializer(m_ilogger);
                RabbitMQPeeker m_rabbitMQ = new RabbitMQPeeker(m_ilogger, m_serializer);
                IDatabase db;

                //队列
                string queue = "";
                //路由key
                string routingKey = "";

                m_rabbitMQ.PeekMessageByRabbitMQ(queue, routingKey, (string content) =>
                {
                    //转换成Fujica缴费实体
                    IssuedRecord payModel = m_serializer.Deserialize<IssuedRecord>(content);
                    if (payModel != null)
                    {
                        //验证当前是否临时车
                        if (payModel.CardType == 3)
                        {
                            //验证当前车牌是否在场
                            string carNumber = payModel.CarNo;
                            int dbIndex = GetDatabaseNumber(carNumber);
                            db = FollowRedisHelper.GetDatabase(dbIndex);
                            //去redis中查询车辆是否在场，返回在场实体
                            VehicleEntryDetailModel entryModel = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(carNumber, payModel.ParkingCode));
                            //不为空，则代表车辆在场
                            if (entryModel != null)
                            {
                                //fujica缴费实体传过来的“停车记录编号”和本系统中的“当次停车唯一标识”的值相等，确认是同一条记录
                                if (payModel.OffLineOrderId == entryModel.RecordGuid)
                                {
                                    //查询当前车辆的计费模板，以获取缴费超时时间，计算最后出场时间
                                    db = FollowRedisHelper.GetDatabase(0);
                                    BillingTemplateModel billingModel = m_serializer.Deserialize<BillingTemplateModel>(db.HashGet("BillingTemplateList", entryModel.CarTypeGuid));
                                    int timeOut = TemplateDataTimeOut(billingModel, m_ilogger, m_serializer);

                                    //发送缴费数据到相机
                                    SendPayData.Execute(entryModel.ParkingCode, entryModel.RecordGuid, entryModel.CarNo, entryModel.BeginTime, payModel.FeeEndTime.AddMinutes(timeOut));
                                }
                            }
                        }
                    }

                    return RabbitMQAction.ACCEPT;
                });

            });
        }

        /// <summary>
        /// 散列车辆在场数据到不同db(10-15)
        /// </summary>
        /// <returns></returns>
        private static int GetDatabaseNumber(string carNo)
        {
            byte[] byts = Encoding.UTF8.GetBytes(carNo);
            int length = byts.Length;
            int sum = 0;
            for (int i = 0; i < length; i++)
            {
                sum += byts[i];
            }
            return (sum % 6) + 10; //6代表分配到6个db里,10代表从第10个db开始
        }



        /// <summary>
        /// 获得模板数据的超时时长
        /// </summary>
        /// <param name="chargeMode">计费方式</param>
        /// <returns>分钟数</returns>
        private static int TemplateDataTimeOut(BillingTemplateModel model, ILogger m_logger, ISerializer m_serializer)
        {
            int timeOut = 0;
            ITollCalculator tollModel = null;
            switch (model.ChargeMode)
            {
                case 1:
                    tollModel = new TollCalculator_Hourly(m_logger, m_serializer);
                    break;
                case 2:
                    tollModel = new TollCalculator_Segment(m_logger, m_serializer);
                    break;
                case 3:
                    tollModel = new TollCalculator_ShenZheng(m_logger, m_serializer);
                    break;
                case 4:
                    tollModel = new TollCalculator_HalfHourly(m_logger, m_serializer);
                    break;
                case 5:
                    tollModel = new TollCalculator_SimpleSegment(m_logger, m_serializer);
                    break;
                case 6:
                    tollModel = new TollCalculator_SegmentHourly(m_logger, m_serializer);
                    break;
                case 7:
                    tollModel = new TollCalculator_SegmentNone(m_logger, m_serializer);
                    break;
                case 8:
                    tollModel = new TollCalculator_SegmentHalfHour(m_logger, m_serializer);
                    break;
                case 9:
                    tollModel = new TollCalculator_NewSegment(m_logger, m_serializer);
                    break;
                case 10:
                    tollModel = new TollCalculator_SegmentQuarterHour(m_logger, m_serializer);
                    break;
            }
            if (tollModel != null)
                timeOut = tollModel.GetLeaveTimeOut(model);
            return timeOut;
        }

    }
}
