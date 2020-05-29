using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/***************************************************************************************
 * *
 * *        File Name        : CorrectCarnoModel.cs
 * *        Creator          : Ase
 * *        Create Time      : 2019-10-08 
 * *        Remark           : 修正车牌模型
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 修正车牌模型
    /// </summary>
    /// <remarks>
    /// 2019.10.08: 新增字段NewCarTypeGuid. Ase<br/>
    /// </remarks>   
    public class CorrectCarnoModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary> 
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 识别相机
        /// </summary>
        public string Discerncamera { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 旧停车唯一标识
        /// </summary>
        public string OldGuid { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ThroughName { get; set; }


        /// <summary>
        /// 旧车牌
        /// </summary>
        public string OldCarno { get; set; }

        /// <summary>
        /// 新车牌
        /// </summary>
        public string NewCarno { get; set; }

        /// <summary>
        /// 入场照片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime InTime { get; set; }

        /// <summary>
        /// 车类guid
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 新车类guid
        /// </summary>
        public string NewCarTypeGuid { get; set; }

        /// <summary>
        /// 车类
        /// </summary>
        public CarTypeEnum CarType { get; set; }

        /// <summary>
        /// 操作类型 0-入场 1-出场
        /// </summary>
        public int OperationType { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }
    }
    /// <summary>
    /// 补录记录模型 查询模型
    /// </summary>
    public class CorrectCarnoSearch 
    {
        /// <summary>
        /// 当前页码
        /// </summary> 
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary> 
        public string ProjectGuid { get; set; }
         

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; } 

        /// <summary>
        /// 旧车牌
        /// </summary>
        public string OldCarno { get; set; }

        /// <summary>
        /// 新车牌
        /// </summary>
        public string NewCarno { get; set; } 
        /// <summary>
        /// 操作开始时间
        /// </summary>
        public DateTime StrTime { get; set; }

        /// <summary>
        /// 操作结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
          

        /// <summary>
        /// 操作类型 0-入场 1-出场
        /// </summary>
        public int OperationType { get; set; } 

        /// <summary>
        /// 操作员
        /// </summary>
        public string Operator { get; set; }

    }
}
