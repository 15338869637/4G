using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Base
{
    /// <summary>
    /// 车场业务错误枚举
    /// </summary>
    public enum BussinessErrorCodeEnum
    {
        #region 基础错误枚举 0000

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        BUSINESS_PARAM_ERROR = 0001,
        /// <summary>
        /// MQ数据同步失败
        /// </summary>
        [Description("MQ数据同步失败")]
        BUSINESS_MQ_SEND_ERROR = 0002,
        /// <summary>
        /// 项目编码错误
        /// </summary>
        [Description("项目编码错误")]
        BUSINESS_PARAM_ERROR_PROJECTGUID = 0003,
        /// <summary>
        /// 停车场编码错误
        /// </summary>
        [Description("停车场编码错误")]
        BUSINESS_PARAM_ERROR_PARKINGCODE = 0004,
        /// <summary>
        /// 保存失败
        /// </summary>
        [Description("保存失败")]
        BUSINESS_SAVE_ERROR = 0005,

        /// <summary>
        /// 数据添加至主平台失败
        /// </summary>
        [Description("保存至主平台失败")]
        BUSINESS_SAVE_CARD_TOFUJICA = 0006,

        #endregion

        #region 车场错误枚举 1000

        /// <summary>
        /// 车场编码已存在
        /// </summary>
        [Description("当前停车场编码已使用")]
        BUSINESS_EXISTS_PARKINGCODE = 1001,

        /// <summary>
        /// 停车场编号不存在
        /// </summary>
        [Description("当前停车场不存在，请重试")]
        BUSINESS_NOTEXISTS_PARKLOT = 1002,

        /// <summary>
        /// 车场保存失败
        /// </summary>
        [Description("保存失败，请联系运营服务商")]
        BUSINESS_SAVE_PARKLOT = 1003,
        
        /// <summary>
        /// 删除车场失败
        /// </summary>
        [Description("删除停车场失败")]
        BUSINESS_DELETE_PARKLOT = 1004,

        #endregion

        #region 车道错误枚举 2000
        
        /// <summary>
        /// 车道保存失败
        /// </summary>
        [Description("保存失败，请联系运营服务商")]
        BUSINESS_SAVE_DRIVEWAY = 2001,

        /// <summary>
        /// 删除车道失败
        /// </summary>
        [Description("删除失败，请联系运营服务商")]
        BUSINESS_DELETE_DRIVEWAY = 2002,

        /// <summary>
        /// 车道不存在
        /// </summary>
        [Description("车道不存在")]
        BUSINESS_NOTEXISTS_DRIVEWAY = 2003,

        /// <summary>
        /// 车道设备标识已存在（相机mac地址）
        /// </summary>
        [Description("当前车道设备标识已使用")]
        BUSINESS_EXISTS_MACADDRESS_DRIVEWAY = 2004,
        #endregion

        #region 卡务错误枚举 3000

        /// <summary>
        /// 卡务-保存失败
        /// </summary>
        [Description("保存失败，请重试")]
        BUSINESS_SAVE_CARD = 3001,

        /// <summary>
        /// 卡务-起始日期小于当前日期
        /// </summary>
        [Description("起始日期小于当前日期")]
        BUSINESS_SAVE_CARD_STARTDATE = 3002,

        /// <summary>
        /// 卡务-截至日期小于当前日期
        /// </summary>
        [Description("截至日期小于当前日期")]
        BUSINESS_SAVE_CARD_ENDDATE = 3003,
        
        /// <summary>
        /// 卡务-车牌已存在
        /// </summary>
        [Description("当前车牌已存在")]
        BUSINESS_EXISTS_CARD = 3005,

        /// <summary>
        /// 卡务-车牌不存在
        /// </summary>
        [Description("当前车牌不存在")]
        BUSINESS_NOTEXISTS_CARD = 3006,

        #endregion

        #region 车辆错误枚举 4000

        /// <summary>
        /// 未找到车辆在场记录
        /// </summary>
        [Description("未找到车辆在场记录")]
        BUSINESS_NOTEXISTS_CAR = 4001,

        #endregion

        #region 计费错误枚举 5000

        /// <summary>
        /// 保存计费模板失败
        /// </summary>
        [Description("保存计费模板失败")]
        BUSINESS_SAVE_BILLING = 5001,

        /// <summary>
        /// 计费模板添加至主平台失败
        /// </summary>
        [Description("计费模板保存至主平台失败")]
        BUSINESS_SAVE_BILLING_TOFUJICA = 5002,

        /// <summary>
        /// 删除计费模板失败
        /// </summary>
        [Description("删除计费模板失败")]
        BUSINESS_DELETE_BILLING = 5003,

        #endregion

        #region 车类错误枚举 6000

        /// <summary>
        /// 车类不存在
        /// </summary>
        [Description("车类不存在")]
        BUSINESS_NOTEXISTS_CARTYPE = 6001,

        /// <summary>
        /// 车类保存失败
        /// </summary>
        [Description("保存失败")]
        BUSINESS_SAVE_CARTYPE = 6002,

        /// <summary>
        /// 车类删除失败
        /// </summary>
        [Description("删除失败")]
        BUSINESS_DELETE_CARTYPE = 6003,

        #endregion

        #region 黑名单错误枚举 7000

        /// <summary>
        /// 黑名单保存失败
        /// </summary>
        [Description("保存失败，请联系运营服务商")]
        BUSINESS_SAVE_BLACK = 7001,

        /// <summary>
        /// 删除黑名单失败
        /// </summary>
        [Description("删除黑名单失败")]
        BUSINESS_DELETE_BLACK = 7002,

        #endregion

        #region 语音命令错误枚举 8000
                
        /// <summary>
        /// 语音命令保存失败
        /// </summary>
        [Description("保存失败")]
        BUSINESS_SAVE_VOICECOMMAND = 8001,

        #endregion
    }


    public static class ErrorCodeBase
    {
        /// <summary>
        /// 扩展方法，获得枚举的Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstend">当枚举没有定义DescriptionAttribute,是否用枚举名代替，默认使用</param>
        /// <returns>枚举的Description</returns>
        public static string GetDesc(this Enum value, bool nameInstend = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }
            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (attribute == null && nameInstend == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }
    }

}
