/***************************************************************************************
 * *
 * *        File Name        : CarTypeModel.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           : 在场车模型
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using System;
using System.ComponentModel.DataAnnotations;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 在场车模型.
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 新增版本备注信息 llp <br/> 
    /// 2019.09.17: 修改枚举引用 llp<br/>
    /// 2019.09.26: 修改实体名称InVehicleDelete为InVehicleDeleteModel Ase<br/>
    /// </remarks>   
    public class PresenceOfVehicleModel : IBaseModel
    {
        /// <summary>
        /// 车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 车场名称
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 当次停车唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 入口名称
        /// </summary>
        public string EntranceName { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }

        /// <summary>
        /// 车辆入场图片地址
        /// </summary>
        public string InImgUrl { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public DateTime InTime { get; set; }

        /// <summary>
        /// 停车时长
        /// </summary>
        public string StopLong { get; set; }

        /// <summary>
        /// 车类标识
        /// </summary>
        public string CarTypeGuid { get; set; }

        /// <summary>
        /// 车类名称
        /// </summary>
        public string CarTypeName { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 在场车场删除信息请求实体
    /// </summary>
    public class InVehicleDeleteModel
    {
        /// <summary>
        /// 停车记录编码
        /// </summary>
        [Required(ErrorMessage = "停车记录编码不能为空")]
        public string RecordGuid { get; set; }
        /// <summary>
        /// 车类
        /// </summary>
        public string CarTypeGuid { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
        [Required(ErrorMessage = "车牌不能为空")]
        public string CarNo { get; set; }

        /// <summary>
        /// 车道相机设备地址
        /// </summary>
        public string DeviceMACAddress { get; set; }

        /// <summary>
        /// 删除的出场照片
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 通行方式   
        /// 出场：1、自动开闸  2、手动开闸  3、免费开闸   4、收费放行  5、场内删除
        /// </summary>
        [Required(ErrorMessage = "通行方式不能为空")]
        public OpenTypeEnum OpenType { get; set; }
        /// <summary>
        /// 当前操作员
        /// </summary>
        [Required(ErrorMessage = "操作人员不能为空")]
        public string Operator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
