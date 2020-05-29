using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Business.Parking;
using Fujica.com.cn.Business.ParkLot;
using Fujica.com.cn.Business.Toll;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsTestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger ilogger = new Logger();
            ISerializer iserializer = new JsonSerializer(ilogger);

 



            #region 车场
            //ParkLotModel model = new ParkLotModel()
            //{
            //    ProjectGuid = "f3c68fef27fd4300b195074237b44803",
            //    ParkCode = "17000107550002",
            //    ParkName = "测试车场3",
            //    Prefix = new string[] { "粤", "B" },
            //    Type = 0,
            //    SpacesNumber = 100
            //};
            //IBaseRedisOperate<ParkLotModel> iparklotredis = new ParkLotRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<ParkLotModel> iparklotdatabase = new ParkLotPersistent(ilogger, iserializer);
           // ParkLotManager parklotmanager = new ParkLotManager(ilogger, iserializer, iparklotredis, iparklotdatabase);
            //parklotmanager.AddNewParkLot(model);
            //ParkLotModel content = parklotmanager.GetParkLot("17000107550001");
            //List<ParkLotModel> content = parklotmanager.AllParklot("f3c68fef27fd4300b195074237b44803");
            #endregion

            #region 在场车辆

            //RabbitMQSender mq = new RabbitMQSender(ilogger, iserializer);

            //IBaseRedisOperate<ParkLotModel> iredis4 = new ParkLotRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<ParkLotModel> idatabase4 = new ParkLotPersistent(ilogger, iserializer);
            //ParkLotManager manager4 = new ParkLotManager(ilogger, iserializer, iredis4, idatabase4);

            //IBaseRedisOperate<CarTypeModel> iredis1 = new CarTypeRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<CarTypeModel> idatabase1 = new CarTypePersistent(ilogger, iserializer);
            //CarTypeManager manager1 = new CarTypeManager(ilogger, iserializer, iredis1, idatabase1, mq);

            //IBaseRedisOperate<DrivewayModel> iredis3 = new DrivewayRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<DrivewayModel> idatabase3 = new DrivewayPersistent(ilogger, iserializer);
            //DrivewayManager manager3 = new DrivewayManager(ilogger, iserializer, iredis3, idatabase3,mq);

            //VehicleInOutManager vehicleinoutmanager = new VehicleInOutManager(ilogger, iserializer, manager4, manager1, manager3);
            //vehicleinoutmanager.AllPresenceOfVehicle("f3c68fef27fd4300b195074237b44803");
            #endregion

            #region 车道
            //DrivewayModel model = new DrivewayModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    guid = "0db2d5d0a98842ff9603238295ea5e18",// "d482d7ab82fe4b5aad4ff5ef787ba3c2",
            //    drivewayName = "深南大道1007号东门入口",
            //    type = DrivewayType.In,
            //    deviceName = "无人值守相机2",
            //    deviceMacAddress = "04-45-53-54-A0-00",
            //    deviceEntranceURI = "192.168.177.63/login.jsp",
            //    deviceAccount = "admin",
            //    deviceVerification = "admin"
            //};

            //IBaseRedisOperate<DrivewayModel> iredis = new DrivewayRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<DrivewayModel> idatabase = new DrivewayPersistent(ilogger, iserializer);
            //DrivewayManager manager = new DrivewayManager(ilogger, iserializer, iredis, idatabase);
            //manager.AddDriveway(model);
            #endregion

            #region 其它
            //RabbitMQSender mq = new RabbitMQSender(ilogger, iserializer);

            //IBaseRedisOperate<FunctionPointModel> iredis = new FunctionPointRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<FunctionPointModel> idatabase = new FunctionPointPersistent(ilogger, iserializer);    

            //IBaseRedisOperate<CarTypeModel> iredis1 = new CarTypeRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<CarTypeModel> idatabase1 = new CarTypePersistent(ilogger, iserializer);
            //CarTypeManager manager1 = new CarTypeManager(ilogger, iserializer, iredis1, idatabase1, mq);

            //IBaseRedisOperate<CityCodeModel> citycodeRedis = new CityCodeRedisCache(ilogger, iserializer);
            //CityCodeManager citycodeManager = new CityCodeManager(ilogger, iserializer, citycodeRedis, mq);

            //IBaseRedisOperate<ParkLotModel> iredis2 = new ParkLotRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<ParkLotModel> idatabase2 = new ParkLotPersistent(ilogger, iserializer);
            //ParkLotManager manager2 = new ParkLotManager(ilogger, iserializer, iredis2, idatabase2, citycodeManager);

            //IBaseRedisOperate<DrivewayModel> iredis3 = new DrivewayRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<DrivewayModel> idatabase3 = new DrivewayPersistent(ilogger, iserializer);
            //DrivewayManager manager3 = new DrivewayManager(ilogger, iserializer, iredis3, idatabase3, mq, manager2);

            //IBaseRedisOperate<BillingTemplateModel> billingtemplateRedis = new BillingTemplateRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<BillingTemplateModel> billingtemplateDatabase = new BillingTemplatePersistent(ilogger, iserializer);
            //BillingTemplateManager billingtemplateManager = new BillingTemplateManager(ilogger, iserializer, billingtemplateRedis, billingtemplateDatabase, manager1);

            //FunctionPointManager manager = new FunctionPointManager(ilogger, iserializer, iredis, idatabase, mq, manager3, manager1, billingtemplateManager);
            //FunctionPointModel model=manager.GetFunctionPoint("f3c68fef27fd4300b195074237b44803", "17000107550001");
            //model.bluePlateCarTypeGuid = "543629aa276944b1adf1e4b35e7898c5";

            ////manager1.SetDefaultCarType("543629aa276944b1adf1e4b35e7898c5");//先设置一个默认车类
            //manager.SetFunctionPoint(model);
            #endregion

            #region 黑名单
            //BlacklistModel model = new BlacklistModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    list = new List<BlacklistModel.BlacklistSingleModel>()
            //    {
            //        new BlacklistModel.BlacklistSingleModel()
            //        {
            //            carNo="粤B24115",
            //            enable=true
            //        }
            //    }
            //};
            //IBaseRedisOperate<BlacklistModel> iparklotredis = new BlacklistRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<BlacklistModel> iparklotdatabase = new BlacklistPersistent(ilogger, iserializer);
            //BlacklistManager blacklistmanager = new BlacklistManager(ilogger, iserializer, iparklotredis, iparklotdatabase);
            //blacklistmanager.DeleteBlacklist(model);
            //BlacklistModel content = blacklistmanager.GetBlacklist("17000107550001");
            #endregion

            #region 临时车计费模板
            //BillingTemplateModel model = new BillingTemplateModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    carTypeGuid = "c3c68fef27fd4200b1ff074237b4712a",
            //    chargeMode=1,
            //    templateJson= iserializer.Serialize(new HourlyTollModel()
            //    {
            //        h1=5,h2=6,h3=7,h4=8,
            //        h5 = 9,
            //        h6 = 10,
            //        h7 = 11,
            //        h8 = 12,
            //        h9 = 13,
            //        h10 = 14,
            //        h11 = 15,
            //        h12 = 16,
            //        h13 = 17,
            //        h14 = 18,
            //        h15 = 19,
            //        h16 = 20,
            //        h17 = 5,
            //        h18 = 6,
            //        h19 = 7,
            //        h20 = 5,
            //        h21 = 5,
            //        h22 = 6,
            //        h23 = 7,
            //        h24 = 5,
            //        LeaveTimeout=15,
            //        FreeMinutes=15,
            //        MonetaryUnit=1,
            //        DayAmountTopLimit=10,
            //        AmountTopLimit=999
            //    })
            //};
            //IBaseRedisOperate<BillingTemplateModel> iparklotredis = new BillingTemplateRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<BillingTemplateModel> iparklotdatabase = new BillingTemplatePersistent(ilogger, iserializer);
            //BillingTemplateManager manager = new BillingTemplateManager(ilogger, iserializer, iparklotredis, iparklotdatabase);
            //manager.AddNewBillingTemplate(model);
            //BillingTemplateModel content = manager.GetBillingTemplate("c3c68fef27fd4200b1ff074237b4712a");

            //TollCalculator_Hourly toll = new TollCalculator_Hourly(ilogger, iserializer);
            //decimal fee = toll.GetFees(content, DateTime.Now.AddHours(-1030), DateTime.Now, true);
            #endregion

            #region 固定车模板
            //PermanentTemplateModel model = new PermanentTemplateModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    carTypeGuid = "a3c68fef27fd4300b19f074237b47823",
            //    months = 1,
            //    amount = 100,
            //    operateTime = "2018-11-07 09:41:03",
            //    operateUser = "admin"
            //};
            //IBaseRedisOperate<PermanentTemplateModel> iparklotredis = new PermanentTemplateRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<PermanentTemplateModel> iparklotdatabase = new PermanentTemplatePersistent(ilogger, iserializer);
            //PermanentTemplateManager permanenttemplatemanager = new PermanentTemplateManager(ilogger, iserializer, iparklotredis, iparklotdatabase);
            //permanenttemplatemanager.AddNewPermanentTemplate(model);
            //PermanentTemplateModel content = permanenttemplatemanager.GetPermanentTemplate("a3c68fef27fd4300b19f074237b47823");
            #endregion

            #region 车类
            //CarTypeModel model = new CarTypeModel()
            //{
            //    projectGuid= "f3c68fef27fd4300b195074237b44803",
            //    parkCode= "17000107550001",
            //    guid= "a3c68fef27fd4300b19f074237b47823",
            //    carTypeName="月租车A",
            //    carType=1,
            //    enable=true
            //};
            //IBaseRedisOperate<CarTypeModel> iredis = new CarTypeRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<CarTypeModel> idatabase = new CarTypePersistent(ilogger, iserializer);
            //RabbitMQSender mq = new RabbitMQSender(ilogger, iserializer);
            //CarTypeManager manager = new CarTypeManager(ilogger, iserializer, iredis, idatabase, mq);
            //manager.AddNewCarType(model);
            #endregion

            #region 卡务
            //CardServiceModel model = new CardServiceModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    carTypeGuid = "a3c68fef27fd4300b19f074237b47823",
            //    carOwnerName = "管理员",
            //    mobile = "18922700065",
            //    carNo = "粤BD50718",
            //    payAmount = 600,
            //    payStyle = "微信",
            //    startDate = DateTime.Now.AddMonths(1),
            //    endDate = DateTime.Now.AddMonths(3),
            //    enable = true,
            //    drivewayGuidList = new List<string>() { "8729d52afa264222b7e33aa6616bef8e" } //富士楼下停车场
            //};

            //RabbitMQSender mq = new RabbitMQSender(ilogger, iserializer);

            //IBaseRedisOperate<CarTypeModel> iredis1 = new CarTypeRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<CarTypeModel> idatabase1 = new CarTypePersistent(ilogger, iserializer);
            //CarTypeManager manager1 = new CarTypeManager(ilogger, iserializer, iredis1, idatabase1, mq);

            //IBaseRedisOperate<PermanentTemplateModel> iredis2 = new PermanentTemplateRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<PermanentTemplateModel> idatabase2 = new PermanentTemplatePersistent(ilogger, iserializer);
            //PermanentTemplateManager manager2 = new PermanentTemplateManager(ilogger, iserializer, iredis2, idatabase2);

            //IBaseRedisOperate<DrivewayModel> iredis3 = new DrivewayRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<DrivewayModel> idatabase3 = new DrivewayPersistent(ilogger, iserializer);
            //DrivewayManager manager3 = new DrivewayManager(ilogger, iserializer, iredis3, idatabase3,mq);

            //IBaseRedisOperate<CardServiceModel> iredis = new CardServiceRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<CardServiceModel> idatabase = new CardServicePersistent(ilogger, iserializer);

            //CardServiceManager manager = new CardServiceManager(ilogger, iserializer, iredis, idatabase, mq, manager1, manager2, manager3);
            //manager.ModifyCard(model);
            //manager.AddNewCard(model);
            //manager.PauseCard("湘L7K632", "17000107550001");
            //manager.ContinueCard("湘L7K632", "17000107550001");
            //List<CardServiceModel> content = manager.AllValueCard("17000107550001");
            //List<CardServiceModel> content = manager.AllMonthCard("17000107550001");
            //CardServiceModel content = manager.GetCard("湘L7K632", "17000107550001");
            //manager.LockedCard("湘L7K632", "17000107550001");
            //manager.UnLockedCard("湘L7K632", "17000107550001");
            //manager.RenewCard(model);
            //manager.DeleteCard("湘L7K632", "17000107550001");
            #endregion

            #region 语音指令
            //VoiceCommandModel model = new VoiceCommandModel()
            //{
            //    projectGuid = "f3c68fef27fd4300b195074237b44803",
            //    parkCode = "17000107550001",
            //    drivewayGuid = "d482d7ab82fe4b5aad4ff5ef787ba3c2",
            //    commandList = new List<CommandDetialModel>() {
            //        new CommandDetialModel() {
            //            commandType=VoiceCommand.TimeShow,
            //            showVoice="",
            //            showText="深圳市富士智能系统有限公司研制 电话：13012345678"
            //        },
            //        new CommandDetialModel() {
            //            commandType=VoiceCommand.TempCarIn,
            //            showVoice="<p>,欢迎光临",
            //            showText="<p>,欢迎光临"
            //        }
            //    }
            //};

            //IBaseRedisOperate<DrivewayModel> iredis1 = new DrivewayRedisCache(ilogger, iserializer);
            //IBaseDataBaseOperate<DrivewayModel> idatabase1 = new DrivewayPersistent(ilogger, iserializer);
            //DrivewayManager manager1 = new DrivewayManager(ilogger, iserializer, iredis1, idatabase1);

            //IBaseDataBaseOperate<VoiceCommandModel> idatabase = new VoiceCommandPersistent(ilogger, iserializer);
            //RabbitMQSender mq = new RabbitMQSender(ilogger, iserializer);
            //VoiceCommandManager manager = new VoiceCommandManager(ilogger, iserializer, idatabase, mq, manager1);
            //manager.SaveCommand(model);
            #endregion

            Console.ReadKey();
        }
    }
}
