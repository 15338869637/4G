
/***************************************************************************************
 * *
 * *        File Name        : CarTypeModel.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 车道模型
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车道模型.
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 新增版本备注信息 llp <br/> 
    /// 2019.09.17: 修改枚举引用 llp<br/>
    /// </remarks>   
    public class DrivewayModel: IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车道名
        /// </summary>
        public string DrivewayName { get; set; }

        /// <summary>
        /// 车道类型(模式)
        /// </summary>
        public DrivewayType Type { get; set; }

        /// <summary>
        /// 设备名称 自定义就好
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备标识 如MAC地址等
        /// </summary>
        public string DeviceMacAddress { get; set; }

        /// <summary>
        /// 设备登录地址
        /// </summary>
        public string DeviceEntranceURI { get; set; }

        /// <summary>
        /// 设备账户
        /// </summary>
        public string DeviceAccount { get; set; }

        /// <summary>
        /// 设备验证口令
        /// </summary>
        public string DeviceVerification { get; set; }
    }

  
}
