using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
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
    /// 发送缴费数据到相机
    /// </summary>
    public class SendPayData
    {
        public static bool Execute(string parkingCode,string guid, string carNo, DateTime beginTime, DateTime lastTime)
        {
            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(carNo) || beginTime == null || lastTime == null)
                return false;
            
            ILogger m_ilogger = new Logger.Logger();
            ISerializer m_serializer = new JsonSerializer(m_ilogger);
            RabbitMQSender m_rabbitMQ = new RabbitMQSender(m_ilogger, m_serializer);
            IDatabase db;

            db = FollowRedisHelper.GetDatabase(0);
            RedisValue parklotredis = db.HashGet("ParkLotList", parkingCode);
            if (parklotredis != RedisValue.Null)
            {
                ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(parklotredis);
                if (parklotmodel != null)
                {
                    List<string> drivewaylist = parklotmodel.DrivewayList; //所有车道
                    if (drivewaylist != null)
                    {
                        //要广播的缴费数据
                        TempCardModel tempCarModel = new TempCardModel();
                        tempCarModel.Guid = guid;
                        tempCarModel.CarNo = carNo;
                        tempCarModel.BeginTime = beginTime;
                        tempCarModel.LatestTime = lastTime;
                        tempCarModel.HavePaid = true;

                        foreach (var drivewayguid in drivewaylist)
                        {
                            DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid));
                            //广播到所有出口车道
                            if (drivewaymodel.Type == DrivewayType.Out)
                            {
                                CommandEntity<TempCardModel> sendCommand = new CommandEntity<TempCardModel>()
                                {
                                    command = BussineCommand.TempCar,
                                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                                    message = tempCarModel
                                };

                                if (m_rabbitMQ.SendMessageForRabbitMQ("发送缴费数据广播命令", m_serializer.Serialize(sendCommand), drivewaymodel.DeviceMacAddress, parkingCode))
                                {
                                    //广播成功
                                }
                            }
                        }
                    }
                }
            }
            return true;

        }
    }
}
