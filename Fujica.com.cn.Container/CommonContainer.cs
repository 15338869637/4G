using Autofac;
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Security.AdmissionControl;
using Fujica.com.cn.Tools;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fujica.com.cn.Container
{
    /// <summary>
    /// 公共对象容器
    /// </summary>
    public class CommonContainer
    {
        static string[] DLLs;
        static string binPath = AppDomain.CurrentDomain.BaseDirectory + "\\";
        static CommonContainer()
        {
            var appdomain = AppDomain.CurrentDomain;

            var fileName = appdomain.SetupInformation.ConfigurationFile.ToString().ToLower();
            //检测是web应用程序还是桌面应用程序
            if (fileName.Contains("web.config"))
            {
                binPath = Path.Combine(binPath, "bin");
            }
            var files = Directory.GetFileSystemEntries(binPath, "*.dll", SearchOption.TopDirectoryOnly);

            DLLs = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                DLLs[i] = Path.GetFileNameWithoutExtension(files[i]).ToLower();
            }
        }

        /// <summary>
        /// AutoFac注册基础类对象类型 也即Fujica.com.cn.Logger,Fujica.com.cn.Tools项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegTools(ContainerBuilder builder)
        {
            builder.RegisterType<Logger.Logger>().As<ILogger>().InstancePerRequest();
            builder.RegisterType<JsonSerializer>().As<ISerializer>().InstancePerRequest();
            //注册autofac的帮助类
            builder.RegisterType<AutofacHelper>().As<IServiceGetter>();
        }

        /// <summary>
        /// AutoFac注册安全类对象类型 也即Security下的项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegSecurity(ContainerBuilder builder)
        {
            //注册日志与序列化器以及api接入控制类型
            builder.RegisterType<APIAccessControl>().As<APIAccessControl>().InstancePerRequest();
        }

        /// <summary>
        /// AutoFac注册通信类对象类型 也即Communication下的项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegCommunication(ContainerBuilder builder)
        {
            //注册rabbitmq发送者
            builder.RegisterType<RabbitMQSender>().As<RabbitMQSender>().InstancePerRequest();
        }

        /// <summary>
        /// AutoFac注册业务层对象类型 也即Business下的项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegBusiness(ContainerBuilder builder)
        {
            var dlls = DLLs.Where(d => d.Contains("fujica.com.cn.business."));
            foreach (var item in dlls)
            {
                var asm = Assembly.Load(item);
                builder.RegisterAssemblyTypes(asm)
                    .Where(t => !t.IsAbstract && t.IsAssignableTo<IBaseBusiness>())
                    .AsSelf();
            }
        }

        /// <summary>
        /// AutoFac注册上下文层对象类型 也即Context下的项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegContext(ContainerBuilder builder)
        {
            var dlls = DLLs.Where(d => d.Contains("fujica.com.cn.context."));
            foreach (var item in dlls)
            {
                var asm = Assembly.Load(item);

                builder.RegisterAssemblyTypes(asm)
                    .Where(t => !t.IsAbstract && t.IsAssignableTo<IBasicContext>())
                    .AsImplementedInterfaces();
            }
        }
        

        /// <summary>
        /// AutoFac注册数据服务层对象实例 也即DataService下的项目
        /// </summary>
        /// <param name="builder"></param>
        public static void RegDataService(ContainerBuilder builder)
        {
            ILogger ilogger = new Logger.Logger();
            ISerializer iserializer = new JsonSerializer(ilogger);

            #region redis部分
            IBaseRedisOperate<APIAccessModel> iapiaccessredis = new APIAccessRedisCache(ilogger, iserializer);
            builder.RegisterInstance(iapiaccessredis);
            IBaseRedisOperate<BillingTemplateModel> ibillingtemplateredis = new BillingTemplateRedisCache(ilogger, iserializer);
            builder.RegisterInstance(ibillingtemplateredis);
            //IBaseRedisOperate<BlacklistModel> iblacklistredis = new BlacklistRedisCache(ilogger, iserializer);
            //builder.RegisterInstance(iblacklistredis);
            IBaseRedisOperate<CarTypeModel> icartyperedis = new CarTypeRedisCache(ilogger, iserializer);
            builder.RegisterInstance(icartyperedis);
            IBaseRedisOperate<CardServiceModel> icardserviceredis = new CardServiceRedisCache(ilogger, iserializer);
            builder.RegisterInstance(icardserviceredis);
            IBaseRedisOperate<DrivewayModel> idrivewayredis = new DrivewayRedisCache(ilogger, iserializer);
            builder.RegisterInstance(idrivewayredis);
            IBaseRedisOperate<DrivewayConnStatusModel> idrivewayConnStatusRedis = new DrivewayConnStatusRedisCache(ilogger, iserializer);
            builder.RegisterInstance(idrivewayConnStatusRedis);
            IBaseRedisOperate<FunctionPointModel> ifunctionpointredis = new FunctionPointRedisCache(ilogger, iserializer);
            builder.RegisterInstance(ifunctionpointredis);
            //IBaseRedisOperate<GatherAccountModel> igatheraccountredis = new GatherAccountRedisCache(ilogger, iserializer);
            //builder.RegisterInstance(igatheraccountredis);
            IBaseRedisOperate<MenuModel> imenuredis = new MenuRedisCache(ilogger, iserializer);
            builder.RegisterInstance(imenuredis);
            IBaseRedisOperate<ParkLotModel> iparklotredis = new ParkLotRedisCache(ilogger, iserializer);
            builder.RegisterInstance(iparklotredis);
            IBaseRedisOperate<PermanentTemplateModel> ipermanenttemplateredis = new PermanentTemplateRedisCache(ilogger, iserializer);
            builder.RegisterInstance(ipermanenttemplateredis);
            IBaseRedisOperate<RolePermissionModel> iroleredis = new RoleRedisCache(ilogger, iserializer);
            builder.RegisterInstance(iroleredis);
            IBaseRedisOperate<UserAccountModel> iuserredis = new UserRedisCache(ilogger, iserializer);
            builder.RegisterInstance(iuserredis);
            IBaseRedisOperate<CityCodeModel> icitycoderedis = new CityCodeRedisCache(ilogger, iserializer);
            builder.RegisterInstance(icitycoderedis);
            IBaseRedisOperate<GateKeepListModel> gatekeepredis = new GateKeepListCache(ilogger, iserializer);
            builder.RegisterInstance(gatekeepredis);
            IBaseRedisOperate<GateCatchDetailModel> gatecatchredis = new GateCatchRedisCache(ilogger, iserializer);
            builder.RegisterInstance(gatecatchredis);
            IBaseRedisOperate<CaptureInOutModel> gateDataRedis = new GateDataRedisCache(ilogger, iserializer);
            builder.RegisterInstance(gateDataRedis);
            IBaseRedisOperate<SpaceNumberModel> spacenumberredis = new SpaceNumberRedisCache(ilogger, iserializer);
            builder.RegisterInstance(spacenumberredis);
            #endregion

            #region 数据库部分
            builder.RegisterType<MonthCardServicePersistent>().Named<IBaseDataBaseOperate<CardServiceModel>>("monthCard");
            builder.RegisterType<ValueCardServicePersistent>().Named<IBaseDataBaseOperate<CardServiceModel>>("valueCard");
            builder.RegisterType<TempCardServicePersistent>().Named<IBaseDataBaseOperate<CardServiceModel>>("tempCard");

            IBaseDataBaseOperate<APIAccessModel> iapiaccessdatabase = new APIAccessPersistent(ilogger, iserializer);
            builder.RegisterInstance(iapiaccessdatabase);
            IBaseDataBaseOperate<BillingTemplateModel> ibillingtemplatedatabase = new BillingTemplatePersistent(ilogger, iserializer);
            builder.RegisterInstance(ibillingtemplatedatabase);
            IBaseDataBaseOperate<BlacklistModel> iblacklistdatabase = new BlacklistPersistent(ilogger, iserializer);
            builder.RegisterInstance(iblacklistdatabase);
            IBaseDataBaseOperate<CarTypeModel> icartypedatabase = new CarTypePersistent(ilogger, iserializer);
            builder.RegisterInstance(icartypedatabase);
            //IBaseDataBaseOperate<CardServiceModel> icardservicedatabase = new CardServicePersistent(ilogger, iserializer);
            //builder.RegisterInstance(icardservicedatabase);

            IBaseDataBaseOperate<DrivewayModel> idrivewaydatabase = new DrivewayPersistent(ilogger, iserializer);
            builder.RegisterInstance(idrivewaydatabase);
            IBaseDataBaseOperate<FunctionPointModel> ifunctionpointdatabase = new FunctionPointPersistent(ilogger, iserializer);
            builder.RegisterInstance(ifunctionpointdatabase);
            //IBaseDataBaseOperate<GatherAccountModel> igatheraccountdatabase = new GatherAccountPersistent(ilogger, iserializer);
            //builder.RegisterInstance(igatheraccountdatabase);
            IBaseDataBaseOperate<MenuModel> imenudatabase = new MenuPersistent(ilogger, iserializer);
            builder.RegisterInstance(imenudatabase);
            IBaseDataBaseOperate<OpenGateReasonModel> iopengatereasondatabase = new OpenGateReasonPersistent(ilogger, iserializer);
            builder.RegisterInstance(iopengatereasondatabase);
            IBaseDataBaseOperate<ParkLotModel> iparklotdatabase = new ParkLotPersistent(ilogger, iserializer);
            builder.RegisterInstance(iparklotdatabase);
            IBaseDataBaseOperate<PermanentTemplateModel> ipermanenttemplatedatabase = new PermanentTemplatePersistent(ilogger, iserializer);
            builder.RegisterInstance(ipermanenttemplatedatabase);
            IBaseDataBaseOperate<RolePermissionModel> iroledatabase = new RolePersistent(ilogger, iserializer);
            builder.RegisterInstance(iroledatabase);
            IBaseDataBaseOperate<TrafficRestrictionModel> itrafficrestrictiondatabase = new TrafficRestrictionPersistent(ilogger, iserializer);
            builder.RegisterInstance(itrafficrestrictiondatabase);
            IBaseDataBaseOperate<UserAccountModel> iuserdatabase = new UserPersistent(ilogger, iserializer);
            builder.RegisterInstance(iuserdatabase);
            IBaseDataBaseOperate<VoiceCommandModel> ivoicecommanddatabase = new VoiceCommandPersistent(ilogger, iserializer);
            builder.RegisterInstance(ivoicecommanddatabase);
            IBaseDataBaseOperate<AddRecordModel> addRecorddatabase = new AddRecordPersistent(ilogger, iserializer);
            builder.RegisterInstance(addRecorddatabase);
            IBaseDataBaseOperate<CorrectCarnoModel> correctCarnodatabase = new RecoverCarnoPersistent(ilogger, iserializer);
            builder.RegisterInstance(correctCarnodatabase);
            #endregion
        }
    }
}
