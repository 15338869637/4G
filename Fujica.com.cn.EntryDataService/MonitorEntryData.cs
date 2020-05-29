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
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.EntryDataService
{
    /// <summary>
    /// 监听进场数据
    /// </summary>
    public class MonitorEntryData 
    {
        public static void Execute()
        {
            Task.Factory.StartNew(() => {
                //从mq获取进场数据
                ILogger m_ilogger = new Logger.Logger();
                ISerializer m_serializer = new JsonSerializer(m_ilogger);
                RabbitMQPeeker m_rabbitMQ = new RabbitMQPeeker(m_ilogger, m_serializer);
                IDatabase db;

                m_rabbitMQ.PeekMessageByRabbitMQ("4GCameraEntryDataQueue", "4GCameraEntryDataQueue",
                (string content) => {
                    db = FollowRedisHelper.GetDatabase(0);
                    VehicleInRequest inmodel = m_serializer.Deserialize<VehicleInRequest>(content);
                    if (inmodel != null)
                    {
                        //不为空说明是入场数据  
                        string drivewayguid = db.HashGet("DrivewayLinkMACList", inmodel.DriveWayMAC);
                        DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
                        if (drivewaymodel == null) return RabbitMQAction.REJECT;
                        CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", inmodel.CarTypeGuid));
                        if (cartypemodel == null) return RabbitMQAction.REJECT;
                        ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", cartypemodel.ParkCode));
                        if (parklotmodel == null) return RabbitMQAction.REJECT;
                        VehicleEntryDetailModel entrymodel = new VehicleEntryDetailModel()
                        {
                            RecordGuid = inmodel.Guid,
                            ParkingName = parklotmodel.ParkName,
                            ParkingCode = parklotmodel.ParkCode,
                            CarNo = inmodel.CarNo,
                            InImgUrl = inmodel.ImgUrl,
                            BeginTime = inmodel.InTime,
                            CarTypeGuid = inmodel.CarTypeGuid,
                            Description = inmodel.Remark,
                            CarType = cartypemodel.CarType,
                            CarTypeName = cartypemodel.CarTypeName,
                            Entrance = drivewaymodel.DrivewayName,
                            DriveWayMAC=inmodel.DriveWayMAC
                        };
                        //存到redis
                        try
                        {
                            db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(entrymodel.CarNo));
                            db.HashSet(entrymodel.CarNo, entrymodel.ParkingCode, m_serializer.Serialize(entrymodel)); //存储入场数据
                            bool flag = db.HashExists(entrymodel.CarNo, entrymodel.ParkingCode);

                            //存储成功后在1号db存储在场车牌 在2号db存储待广播的车牌
                            ////3号db缓存半小时该进场数据
                            if (flag)
                            {
                                db = FollowRedisHelper.GetDatabase(1);
                                db.HashSet(entrymodel.ParkingCode, entrymodel.CarNo, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); //存储车场在场车牌索引
                                flag = db.HashExists(entrymodel.ParkingCode, entrymodel.CarNo);
                            }
                            //存储成功后在2号db存储待广播的车牌
                            if (flag)
                            {
                                db = FollowRedisHelper.GetDatabase(2);
                                db.HashSet(entrymodel.ParkingCode, entrymodel.CarNo, content);
                                flag = db.HashExists(entrymodel.ParkingCode, entrymodel.CarNo);
                            }
                            //存储成功后在3号db存储入场车数据半小时
                            if (flag)
                            {
                                db = FollowRedisHelper.GetDatabase(3);
                                string key = string.Format("{0}_{1}", drivewaymodel.Guid, entrymodel.CarNo);
                                db.StringSet(key, content.Replace("InTime", "EventTime"), TimeSpan.FromSeconds(0.5 * 3600));
                                flag = db.KeyExists(key);
                            }

                            //如果是临时车辆，并且redis存储成功后
                            if (entrymodel.CarType == 0 && flag)
                            {
                                //再往主平台Fujica补发入场数据
                                EntryDataToFujica(entrymodel.CarNo, entrymodel.ParkingCode, entrymodel.ParkingName, entrymodel.RecordGuid, entrymodel.DriveWayMAC, entrymodel.Entrance, entrymodel.InImgUrl, entrymodel.BeginTime, cartypemodel.Idx);
                            }


                            if (!flag) return RabbitMQAction.RETRY;
                        }
                        catch (Exception ex)
                        {
                            m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", entrymodel.ParkingCode, "", "Fujica.com.cn.EntryDataService.MonitorEnterData", "保存入场数据到redis异常", ex.ToString());
                            return RabbitMQAction.RETRY;
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
        /// 补发入场数据给主平台Fujica
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="parkingName">停车场名称</param>
        /// <param name="entryGuid">线下停车记录编号</param>
        /// <param name="driveWayMAC">出入口设备机号</param>
        /// <param name="entrance">入口名</param>
        /// <param name="inImgUrl">入场车辆图片地址</param>
        /// <param name="beginTime">车辆的入场时间</param>
        /// <param name="carType">车类</param>
        /// <returns>true:补发成功  false:补发失败</returns>
        private static bool EntryDataToFujica(string carNumber,string parkingCode,string parkingName,string entryGuid,string driveWayMAC,string entrance,string inImgUrl,DateTime beginTime,string carType)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/ApiReVehicleEntryRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["CarNo"] = carNumber;//车牌号
            dicParam["ParkingCode"] = parkingCode;//停车场编码
            dicParam["ParkName"] = parkingName;//停车场名称
            dicParam["CustomDate"] = DateTime.Now;//客户端时间
            dicParam["LineRecordCode"] = entryGuid;//线下停车记录编号
            dicParam["OperatorName"] = "入场服务程序";//操作员
            dicParam["WatchhouseCode"] = driveWayMAC;//出入口设备机号
            dicParam["Entrance"] = entrance;//入口名
            dicParam["InImgUrl"] = inImgUrl;//入场车辆图片地址
            dicParam["BeginTime"] = beginTime;//车辆的入场时间
            dicParam["CarType"] = carType;//车类，对应车辆模板类型
            dicParam["CardType"] = 3;//停车卡类型 1月卡 2储值卡 3 临时卡

            //返回fujica api补发车辆入场记录 接口的结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }

    }
}
