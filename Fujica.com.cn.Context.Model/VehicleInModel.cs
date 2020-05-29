using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/***************************************************************************************
* *
* *        File Name        : VehicleInModel.cs
* *        Creator          : Ase
* *        Create Time      : 2019-09-17 
* *        Remark           : 车辆入场原始数据请求实体类
* *
* *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
* ***************************************************************************************/

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 车辆入场原始数据请求实体
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 修改 开闸类型字段(OpenType)为枚举类型. Ase <br/>       
    /// </remarks>
    public class VehicleInModel
    {
        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DriveWayMAC { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车辆图片地址
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime InTime { get; set; }

        /// <summary>
        /// 车类
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 开闸类型
        /// </summary>
        public OpenTypeEnum OpenType { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 车牌图片url地址
        /// </summary>
        public string PlateUrl { get; set; }
    }
}
