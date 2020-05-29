/***************************************************************************************
 * *
 * *        File Name        : CarTypeModel.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 车类模型
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车类模型.
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 新增版本备注信息 llp <br/> 
    /// 2019.09.17: 修改枚举引用 llp<br/>
    /// </remarks>  
    public class CarTypeModel : IBaseModel
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
        /// 车类名
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 车类 0=时租车 1=月租车 2=储值车 3=贵宾车
        /// </summary>
        public CarTypeEnum CarType { get; set; }

        /// <summary>
        /// 默认车类
        /// </summary>
        public bool DefaultType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string Idx { get; set; }
    }
   
}
