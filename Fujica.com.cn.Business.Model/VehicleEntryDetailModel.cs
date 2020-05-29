using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujica.com.cn.Context.Model;


/***************************************************************************************
* *
* *        File Name        : VehicleEntryDetailModel.cs
* *        Creator          : Ase
* *        Create Time      : 2019-09-17 
* *        Remark           : 车辆入场记录实体详情类
* *
* *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
* ***************************************************************************************/

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 入场记录实体详情
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 修改 开闸类型字段(OpenType)为枚举类型. Ase <br/>       
    /// </remarks>
    public class VehicleEntryDetailModel
    {
        /// <summary>
        /// 当次停车记录唯一标识
        /// </summary>
        public string RecordGuid { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车场名称
        /// </summary>
        public string ParkingName { get; set; }

        /// <summary>
        /// 入口车道名
        /// </summary>
        public string Entrance { get; set; }

        /// <summary>
        /// 入口相机名
        /// </summary>
        public string EntranceCamera { get; set; }

        /// <summary>
        /// 车道MAC地址
        /// </summary>
        public string DriveWayMAC { get; set; }
        
        /// <summary>
        /// 入场图片
        /// </summary>
        public string InImgUrl { get; set; }

        /// <summary>
        /// 车位
        /// </summary>
        public string ParkingCard { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 车类，0=时租车 1=月租车 2=储值车 3=贵宾车
        /// </summary>
        public int CarType { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 车类名称
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 进场说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 开闸类型
        /// </summary>
        public OpenTypeEnum OpenType { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

    }
}
