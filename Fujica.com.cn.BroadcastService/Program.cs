using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Model;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Interface.DataUpload.Models.InPut;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.BroadcastService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BroadcastService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        #region 以下作调试
        //手工广播入场数据
        static void Test1Main(string[] args)
        {
            ILogger m_ilogger = new Logger.Logger();
            ISerializer m_serializer = new JsonSerializer(m_ilogger);
            RabbitMQSender m_rabbitMQ = new RabbitMQSender(m_ilogger, m_serializer);

            IDatabase db = FollowRedisHelper.GetDatabase(2);
            IServer srv = FollowRedisHelper.GetCurrentServer();

            IEnumerable<RedisKey> allParkingCode = srv.Keys(2);
            foreach (var parkingcode in allParkingCode)
            {
                HashEntry[] hashenrtyarray = db.HashGetAll(parkingcode); //所有实体
                string[] allCarNo = db.HashKeys(parkingcode).ToStringArray(); //所有车牌

                db = FollowRedisHelper.GetDatabase(0);
                RedisValue parklotredis = db.HashGet("ParkLotList", parkingcode.ToString());
                if (parklotredis != RedisValue.Null)
                {
                    ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(parklotredis);
                    List<string> drivewaylist = parklotmodel.DrivewayList; //所有车道
                    if (drivewaylist != null)
                    {
                        foreach (var carno in allCarNo)
                        {
                            HashEntry hashenrty = hashenrtyarray.SingleOrDefault(o => o.Name == carno);
                            string enterdata = db.HashGet(parkingcode, carno); //要广播的入场数据
                            foreach (var drivewayguid in drivewaylist)
                            {
                                DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid));
                                if (drivewaymodel.Type == DrivewayType.Out)
                                {
                                    //广播到出口车道
                                    string sendmsg = string.Format("{{\"command\":{0},\"idMsg\":\"{1}\",\"message\":{2}}}",
                                        13, Convert.ToBase64String(Guid.NewGuid().ToByteArray()), enterdata);
                                    if (m_rabbitMQ.SendMessageForRabbitMQ("发送入场数据广播命令", sendmsg, drivewaymodel.DeviceMacAddress, parkingcode))
                                    {
                                        //广播成功，移除缓存的数据
                                        //db.HashDelete(parkingcode, carno);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
