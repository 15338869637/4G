
/***************************************************************************************
 * *
 * *        File Name        : ParkLotManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 停车场管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Business.User;
using Fujica.com.cn.Communication.RabbitMQ;
using Fujica.com.cn.Context.IContext;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 停车场管理器 
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/>  
    /// </remarks> 
    public partial class ParkLotManager: IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger = null;
        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;
        /// <summary>
        /// MQ消息发送器
        /// </summary>
        private readonly RabbitMQSender m_rabbitMQ;

        /// <summary>
        /// 角色管理器
        /// </summary>
        private RoleManager _roleManager = null;
        /// <summary>
        /// 语音指令管理器
        /// </summary>
        private VoiceCommandManager _voiceCommandManager = null;
        /// <summary>
        /// 卡务管理器
        /// </summary>
        private CardServiceManager _cardServiceManager = null;

        /// <summary>
        /// 停车场信息操作上下文
        /// </summary>
        private IParkLotContext _iParkLotContext = null;
        /// <summary>
        /// 车道信息操作上下文
        /// </summary>
        private IDrivewayContext _iDrivewayContext = null;
        /// <summary>
        /// 车类信息操作上下文
        /// </summary>
        private ICarTypeContext _iCarTypeContext = null;
        /// <summary>
        ///计费模板信息操作上下文
        /// </summary>
        private IBillingTemplateContext _iBillingTemplateContext = null;
        /// <summary>
        /// 黑名单信息操作上下文
        /// </summary>
        private IBlacklistContext _iBlacklistContext = null;
        /// <summary>
        /// 车辆进出信息操作上下文
        /// </summary>
        private ICarInOutContext _iCarInOutContext = null;
        /// <summary>
        /// 城市区号信息操作上下文
        /// </summary>
        private ICityCodeContext _iCityCodeContext = null;
        /// <summary>
        /// 功能点信息操作上下文
        /// </summary>
        private IFunctionPointContext _iFunctionPointContext = null;
        /// <summary>
        /// 固定车模板信息操作上下文
        /// </summary>
        private IPermanentTemplateContext _iPermanentTemplateContext = null;
        /// <summary>
        /// 车进出信息操作上下文
        /// </summary>
        private IVehicleInOutContext _iVehicleInOutContext = null;
        /// <summary>
        /// 车位信息操作上下文
        /// </summary>
        private ISpaceNumberContext _iSpaceNumberContext = null;
        /// <summary>
        ///补录数据 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<AddRecordModel> _iAddRecorddatabaseoperate = null; 
        /// <summary>
        ///车牌修正记录 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<CorrectCarnoModel> _iAddCarnoRecorddatabaseoperate = null;
        public string LastErrorDescribe
        {
            get;set;
        }

        public ParkLotManager(ILogger logger, ISerializer serializer, RabbitMQSender rabbitMQ,
            RoleManager roleManager,
            VoiceCommandManager voiceCommandManager,
            CardServiceManager cardServiceManager,
            IParkLotContext iParkLotContext,
            IDrivewayContext iDrivewayContext,
            ICarTypeContext iCarTypeContext,
            IBillingTemplateContext iBillingTemplateContext,
            IBlacklistContext iBlacklistContext,
            ICarInOutContext iCarInOutContext,
            ICityCodeContext iCityCodeContext,
            IFunctionPointContext iFunctionPointContext,
            IPermanentTemplateContext iPermanentTemplateContext,
            IVehicleInOutContext iVehicleInOutContext,
            ISpaceNumberContext iSpaceNumberContext,
            IBaseDataBaseOperate<AddRecordModel> iAddRecorddatabaseoperate,
            IBaseDataBaseOperate<CorrectCarnoModel> iAddCarnoRecorddatabaseoperate)
        {
            m_logger = logger;
            m_serializer = serializer;
            m_rabbitMQ = rabbitMQ;
            _roleManager = roleManager;
            _voiceCommandManager = voiceCommandManager;
            _cardServiceManager = cardServiceManager;
            _iParkLotContext = iParkLotContext;
            _iDrivewayContext = iDrivewayContext;
            _iCarTypeContext = iCarTypeContext;
            _iBillingTemplateContext = iBillingTemplateContext;
            _iBlacklistContext = iBlacklistContext;
            _iCarInOutContext = iCarInOutContext;
            _iCityCodeContext = iCityCodeContext;
            _iFunctionPointContext = iFunctionPointContext;
            _iPermanentTemplateContext = iPermanentTemplateContext;
            _iVehicleInOutContext = iVehicleInOutContext;
            _iSpaceNumberContext = iSpaceNumberContext;
            _iAddRecorddatabaseoperate = iAddRecorddatabaseoperate;
            _iAddCarnoRecorddatabaseoperate = iAddCarnoRecorddatabaseoperate;
        }

        /// <summary>
        /// 添加车场
        /// </summary>
        /// <returns></returns>
        public bool AddNewParkLot(ParkLotModel model,string roleGuid)
        {            
            ParkLotModel dbModel = GetParkLot(model.ParkCode);
            if (dbModel != null)
            {
                //相同的停车场编码已经存在，则不让继续添加
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_EXISTS_PARKINGCODE.GetDesc();
                return false;
            }

            bool flag = _iParkLotContext.AddParkLot(model);
            if (!flag)
            {
                //redis执行不成功，直接返回false
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_PARKLOT.GetDesc();
                return false;
            }

            //配置剩余车位数控制
            SetRemainingSpace(new SpaceNumberModel() { ParkCode = model.ParkCode, RemainingSpace = model.RemainingSpace });

            //添加车场后，需要默认添加多条车类（时租车、储值车、月租车、贵宾车）
            List<CarTypeModel> carTypeList = InitNewCarTypeAll(model.ProjectGuid, model.ParkCode);

            //给时租车、储值车车类添加默认计费模板（按小时算费）
            if (carTypeList != null)
                foreach (var item in carTypeList.Where(m => m.CarType == CarTypeEnum.TempCar || m.CarType == CarTypeEnum.ValueCar))
                {
                    AddNewBillingTemplate(new BillingTemplateModel()
                    {
                        ProjectGuid = model.ProjectGuid,
                        ParkCode = model.ParkCode,
                        CarTypeGuid = item.Guid,
                        ChargeMode = 1,
                        TemplateJson = "{\"MonetaryUnit\":\"1\",\"FreeMinutes\":\"30\",\"LeaveTimeout\":\"15\",\"DayAmountTopLimit\":\"255\",\"AmountTopLimit\":\"65535\",\"h1\":\"1\",\"h2\":\"2\",\"h3\":\"3\",\"h4\":\"4\",\"h5\":\"5\",\"h6\":\"6\",\"h7\":\"7\",\"h8\":\"8\",\"h9\":\"9\",\"h10\":\"10\",\"h11\":\"10\",\"h12\":\"10\",\"h13\":\"10\",\"h14\":\"10\",\"h15\":\"10\",\"h16\":\"10\",\"h17\":\"10\",\"h18\":\"10\",\"h19\":\"10\",\"h20\":\"10\",\"h21\":\"10\",\"h22\":\"10\",\"h23\":\"10\",\"h24\":\"10\"}"
                    });
                }

            //将默认的车类发送给相机
            SendTempCarTypeOfPlateColor(model.ProjectGuid, model.ParkCode);

            //添加车场后，给当前角色增加授权车场列表
            if (!string.IsNullOrEmpty(roleGuid))
            {
                //添加新车编
                _roleManager.ModifyRoleAddParkingCode(roleGuid, model.ParkCode);
            }

            //发送创建新车场的命令
            SendNewParkLotToCameras(model.ParkCode);

            //添加城市区号
            if (model.ParkCode.Length > 10)
            {
                string cityID = model.ParkCode.Substring(6, 4);
                AddCityCode(cityID);
            }
            
            //返回值以添加车场方法返回值为准
            return flag;
        }

        /// <summary>
        /// 修改车场
        /// </summary>
        /// <returns></returns>
        public bool ModifyParkLot(ParkLotModel model)
        {
            ParkLotModel dbModel = GetParkLot(model.ParkCode);
            if (dbModel != null && dbModel.ProjectGuid != model.ProjectGuid)
            {
                //不让修改其他项目的停车场
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_EXISTS_PARKINGCODE.GetDesc();
                return false;
            }
            bool flag = _iParkLotContext.ModifyParkLot(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_SAVE_PARKLOT.GetDesc();
                return false;
            }

            //配置剩余车位数控制
            SetRemainingSpace(new SpaceNumberModel() { ParkCode = model.ParkCode, RemainingSpace = model.RemainingSpace });

            //添加城市区号
            if (model.ParkCode.Length > 10)
            {
                string cityID = model.ParkCode.Substring(6, 4);
                AddCityCode(cityID);
            }

            return true;
        }

        /// <summary>
        /// 注销车场
        /// </summary>
        /// <returns></returns>
        public bool CancelParkLot(ParkLotModel model)
        {
            bool flag = false;

            //删除角色授权车编

            //删除所有车类和计费模板
            List<CarTypeModel> carTypeList = AllCarType(model.ParkCode, model.ProjectGuid);
            if (carTypeList != null)
                foreach (var item in carTypeList)
                {
                    flag = DeleteCarType(item);
                    if (!flag) return false;
                }

            //删除所有车道（业务层的车道删除方法里面，还包含一些其他的操作，比如下发mq、修改车场参数之类的。但此处的删除可以仅删除数据库数据，因为接下来立马会删除车场，所以不需要再执行相关业务动作，则直接调用context层的删除方法）
            List<DrivewayModel> drivewayList = AllDriveway(model.ParkCode);
            if (drivewayList != null)
                foreach (var item in drivewayList)
                {
                    flag = DeleteDriveway(item);
                    if (!flag) return false;
                    //flag = _iDrivewayContext.DeleteDriveway(item);
                    //if (!flag)
                    //{
                    //    LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_DRIVEWAY.GetDesc();
                    //    return false;
                    //}
                }


            //删除车场
            flag = _iParkLotContext.CancelParkLot(model);
            if (!flag)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_DELETE_PARKLOT.GetDesc();
            }
            return flag;
        }

        /// <summary>
        /// 所有车场
        /// </summary>
        /// <returns></returns>
        public List<ParkLotModel> AllParklot(string projectGuid)
        {
            return _iParkLotContext.GetParklotAll(projectGuid);
        }

        /// <summary>
        /// 获取某停车场
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        public ParkLotModel GetParkLot(string parkingCode)
        {
            return _iParkLotContext.GetParkLot(parkingCode);
        }

        /// <summary>
        /// 发送创建新车场的命令
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns></returns>
        private bool SendNewParkLotToCameras(string parkingCode)
        {
            try
            {
                CommandEntity<bool> entity = new CommandEntity<bool>()
                {
                    command = BussineCommand.NewParkLot,
                    idMsg = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    message = true
                };

                return m_rabbitMQ.SendMessageForRabbitMQ("发送新建车场命令", m_serializer.Serialize(entity), entity.idMsg, parkingCode);
            }
            catch (Exception ex)
            {
                m_logger.LogFatal(LoggerLogicEnum.Bussiness, "", parkingCode, "", "Fujica.com.cn.Business.ParkLot.ParkLotManager.SendNewParkLotToCameras", "下发创建新车场发生异常", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 车场整体业务方法访问权限
        /// 验证车场编码和项目编码
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="projectGuid">项目编码</param>
        /// <param name="contentParkLot">out 车场模型</param>
        /// <returns></returns>
        public bool ParkLotAccessPermission(string parkingCode, string projectGuid, out ParkLotModel contentParkLot)
        {
            //验证当前车场是否存在
            contentParkLot = _iParkLotContext.GetParkLot(parkingCode);
            if (contentParkLot == null)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_NOTEXISTS_PARKLOT.GetDesc();
                return false;
            }
            //验证当前车场与项目id是否匹配
            if (projectGuid != contentParkLot.ProjectGuid)
            {
                LastErrorDescribe = BussinessErrorCodeEnum.BUSINESS_PARAM_ERROR_PROJECTGUID.GetDesc();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 车场整体业务方法访问权限
        /// 验证车场编码和项目编码
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="projectGuid">项目编码</param>
        /// <returns></returns>
        public bool ParkLotAccessPermission(string parkingCode, string projectGuid)
        {
            ParkLotModel parkLotModel = null;
            return ParkLotAccessPermission(parkingCode, projectGuid, out parkLotModel);
        } 

    }
}
