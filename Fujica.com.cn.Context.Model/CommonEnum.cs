/***************************************************************************************
* *
* *        File Name        : CommonEnum.cs
* *        Creator          : llp
* *        Create Time      : 2019-09-17 
* *        Remark           : 公共枚举类
* *
* *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
* ***************************************************************************************/
using System.ComponentModel;

namespace Fujica.com.cn.Context.Model
{

    /// <summary>
    /// 公共枚举类
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 创建. llp <br/>   
    /// 2019.09.17: 增加 OpenTypeEnum 通行方式枚举类型  llp <br/> 
    /// </remarks> 
 
    /// <summary>
    /// 通行方式
    /// </summary>
    public enum OpenTypeEnum
    {

        /// <summary>
        /// 自动开闸
        /// </summary>
        [Description("自动开闸")]
        Automatic = 1,
        /// <summary>
        /// 手动开闸
        /// </summary>
        [Description("手动开闸")]
        Manual = 2,
        /// <summary>
        /// 免费开闸
        /// </summary>
        [Description("免费开闸")]
        Free = 3,
        /// <summary>
        /// 收费放行
        /// </summary>
        [Description("收费放行")]
        Charge = 4,
        /// <summary>
        /// 场内删除
        /// </summary>
        [Description("场内删除")]
        FieldDelete = 5
    }
    /// <summary>
    /// 卡类型枚举
    /// </summary>
    public enum CarTypeEnum
    {
        /// <summary>
        /// 临时卡
        /// </summary>
        TempCar = 0,
        /// <summary>
        /// 月租车
        /// </summary>
        MonthCar = 1,
        /// <summary>
        /// 储值车
        /// </summary>
        ValueCar = 2,
        /// <summary>
        /// 贵宾车
        /// </summary>
        VIPCar = 3
    }

    /// <summary>
    /// 车道类型
    /// </summary>
    public enum DrivewayType
    {
        /// <summary>
        /// 入口车道
        /// </summary>
        In = 0,
        /// <summary>
        /// 出口车道
        /// </summary>
        Out = 1
    }


    /// <summary>
    /// 缴费失败枚举
    /// </summary>
    public enum ValueCardFeeType
    {
        /// <summary>
        /// 缴费完成
        /// </summary> 
        BalanceSuccess = 200,
        /// <summary>
        /// 余额不足
        /// </summary> 
        BalanceNoEnough = 201,
        /// <summary>
        /// 扣费失败
        /// </summary>
        BalanceFail = 202,
        /// <summary>
        /// 需要确认开闸
        /// </summary>
        BalanceConfirm = 203
    }

}
