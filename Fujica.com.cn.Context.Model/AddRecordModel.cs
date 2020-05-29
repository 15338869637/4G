using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 补录记录模型
    /// </summary>
    public class AddRecordModel:IBaseModel
    {
        /// <summary>
        /// 设备标识
        /// </summary>
        
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车类guid
        /// </summary>
        public string CarTypeGuid { get; set; } 
        
        /// <summary>
        /// 入场通道
        /// </summary>
        public string Entrance { get; set; }

        /// <summary>
        /// 车类0=时租车 1=月租车 2=储值车 3=贵宾车
        /// </summary>
        public CarTypeEnum CarType { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime InTime { get; set; } 

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime RecordTime { get; set; }

        /// <summary>
        /// 入场图片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
        

    }


    /// <summary>
    /// 补录记录模型 查询模型
    /// </summary>
    public class RecordInSearch 
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
        /// 车牌
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车类guid
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 补录开始时间
        /// </summary>
        public DateTime StrTime { get; set; }

        /// <summary>
        /// 补录结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }

    }
}
