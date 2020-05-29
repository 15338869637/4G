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
using System.Text;
using System.Threading.Tasks;

namespace HandlingExitData
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger m_ilogger = new Logger();
            ISerializer m_serializer = new JsonSerializer(m_ilogger);
            RabbitMQPeeker m_rabbitMQ = new RabbitMQPeeker(m_ilogger, m_serializer);
            IDatabase db;

            Console.WriteLine(string.Format("出场数据处理工具运行中:"));
            //队列名与路由名一致
            m_rabbitMQ.PeekMessageByRabbitMQ("4GCameraExitDataQueue", "4GCameraExitDataQueue",
            (string content) => {

                Console.WriteLine(string.Format("收到出场数据:{0}", content));
                Console.WriteLine("");

                db = FollowRedisHelper.GetDatabase(0);
                VehicleOutRequest outmodel = m_serializer.Deserialize<VehicleOutRequest>(content);
                if (outmodel != null)
                {
                    //不为空说明是出场数据
                    string drivewayguid = db.HashGet("DrivewayLinkMACList", outmodel.DriveWayMAC);
                    DrivewayModel drivewaymodel = m_serializer.Deserialize<DrivewayModel>(db.HashGet("DrivewayList", drivewayguid ?? ""));
                    if (drivewaymodel == null) return RabbitMQAction.REJECT;
                    CarTypeModel cartypemodel = m_serializer.Deserialize<CarTypeModel>(db.HashGet("CarTypeList", outmodel.CarTypeGuid));
                    if (cartypemodel == null) return RabbitMQAction.REJECT;
                    ParkLotModel parklotmodel = m_serializer.Deserialize<ParkLotModel>(db.HashGet("ParkLotList", drivewaymodel.ParkCode));
                    if (parklotmodel == null) return RabbitMQAction.REJECT;
                    VehicleExitDetailModel exitmodel = new VehicleExitDetailModel()
                    {
                        RecordGuid = outmodel.Guid,
                        ParkingName = parklotmodel.ParkName,
                        ParkingCode = parklotmodel.ParkCode,
                        CarNo = outmodel.CarNo,
                        OutImgUrl = outmodel.ImgUrl,
                        LeaveTime = outmodel.OutTime,
                        Description = outmodel.Remark,
                        Exit = drivewaymodel.DrivewayName
                    };
                    //redis操作
                    try
                    {
                        db = FollowRedisHelper.GetDatabase(GetDatabaseNumber(exitmodel.CarNo));
                        //删除前先拿到实体内容
                        VehicleEntryDetailModel entrymodel = m_serializer.Deserialize<VehicleEntryDetailModel>(db.HashGet(exitmodel.CarNo, exitmodel.ParkingCode));

                        db.HashDelete(exitmodel.CarNo, exitmodel.ParkingCode);
                        //删除成功后在1号db移除该在场车牌,在2号db移除待广播的车牌(如果未广播的情况下)
                        //3号db缓存半小时该出场数据
                        //1号
                        db = FollowRedisHelper.GetDatabase(1);
                        db.HashDelete(exitmodel.ParkingCode, exitmodel.CarNo);
                        //2号
                        db = FollowRedisHelper.GetDatabase(2);
                        db.HashDelete(exitmodel.ParkingCode, exitmodel.CarNo);
                        //3号
                        db = FollowRedisHelper.GetDatabase(3);
                        string key = string.Format("{0}_{1}", drivewaymodel.Guid, exitmodel.CarNo);
                        db.StringSet(key, content.Replace("OutTime", "EventTime"), TimeSpan.FromSeconds(0.5 * 3600));


                        //如果是临时车辆，并且入场实体不为空，则往主平台Fujica补发出场数据
                        if (cartypemodel.CarType == 0 && entrymodel != null)
                        {
                            ExitDataToFujica(exitmodel.CarNo, exitmodel.ParkingCode, entrymodel.Entrance, exitmodel.Exit, entrymodel.BeginTime, exitmodel.LeaveTime, entrymodel.InImgUrl, exitmodel.OutImgUrl, exitmodel.RecordGuid, cartypemodel.Idx);
                        }

                        return RabbitMQAction.ACCEPT;
                    }
                    catch (Exception ex)
                    {
                        m_ilogger.LogFatal(LoggerLogicEnum.Tools, "", exitmodel.ParkingCode, "", "Fujica.com.cn.BroadcastService.JobsDetails.MonitorEnterData", "在redis移除停车数据异常", ex.ToString());
                        return RabbitMQAction.RETRY;
                    }
                }
                return RabbitMQAction.ACCEPT; //无效数据的时候直接返回成功
            });

            Console.WriteLine("程序逻辑不幸退出");
            Console.ReadKey();
        }

        static int GetDatabaseNumber(string carNo)
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
        /// 补发出场数据给主平台Fujica
        /// </summary>
        /// <param name="carNumber">车牌号</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="entrance">入口名</param>
        /// <param name="export">出口名</param>
        /// <param name="beginTime">入场时间</param>
        /// <param name="endTime">出场时间</param>
        /// <param name="inImgUrl">入场车辆图片地址</param>
        /// <param name="outImgUrl">出场车辆图片地址</param>
        /// <param name="recordGuid">当次停车记录编号</param>
        /// <param name="carType">车类</param>
        /// <returns>true:补发成功  false:补发失败</returns>
        private static bool ExitDataToFujica(string carNumber, string parkingCode, string entrance, string export, DateTime beginTime, DateTime endTime, string inImgUrl, string outImgUrl, string recordGuid, string carType)
        {
            RequestFujicaStandard requestFujica = new RequestFujicaStandard();
            //请求方法
            string servername = "/Park/ReVehicleOutRecord";
            //请求参数
            Dictionary<string, object> dicParam = new Dictionary<string, object>();
            dicParam["CarNo"] = carNumber;//车牌号
            dicParam["ParkingCode"] = parkingCode;//停车场编码
            dicParam["LongStop"] = (endTime - beginTime).Minutes;//停驶时间（分钟）
            dicParam["Entrance"] = entrance;//入口名
            dicParam["Export"] = export;//出口名
            dicParam["CustomDate"] = DateTime.Now;//客户端时间
            dicParam["AppearanceDate"] = endTime;//出场时间
            dicParam["AdmissionDate"] = beginTime;//入场时间
            dicParam["InImgUrl"] = inImgUrl;//入场车辆图片地址
            dicParam["OutImgUrl"] = outImgUrl;//出场车辆图片地址
            dicParam["LineRecordCode"] = recordGuid;//线下停车记录编号
            dicParam["CarType"] = carType;//车类
            dicParam["CardType"] = 3;//停车卡类型 1月卡 2储值卡 3 临时卡

            //返回fujica api补发车辆入场记录 接口的结果
            return requestFujica.RequestInterfaceV2(servername, dicParam);
        }
    }
}
