/***************************************************************************************
 * *
 * *        File Name        : CarTypeModel.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-17 
 * *        Remark           :  储值卡类模型
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/

namespace Fujica.com.cn.Context.Model
{

    /// <summary>
    /// 储值卡类模型.
    /// </summary>
    /// <remarks>
    /// 2019.09.17: 新增版本备注信息 llp <br/> 
    /// 2019.09.17: 修改枚举引用 llp<br/>
    /// </remarks>  
    public class ValueCardFeeModel: IBaseModel
    {
        /// <summary>
        /// 停车场编号
        /// </summary>
        public string ParkingCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNo { get; set; }


        /// <summary>
        /// 停车费
        /// </summary>
        public decimal ParkingFee { get; set; }


        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 付款二维码
        /// </summary>
        public string QRCode { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// 停车记录编码
        /// </summary>
        public string LineRecordCode { get; set; }

        /// <summary>
        /// 是否扣费成功
        /// </summary>
        public bool IsPaid { get; set; }

        /// <summary>
        /// 命令类型(名称)
        /// </summary>
        public ValueCardFeeType ResponseType { get; set; }

        /// <summary>
        /// 预留扩展字段
        /// </summary>
        public string Remarks { get; set; }

    }

   

}
