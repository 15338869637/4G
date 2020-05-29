using Fujica.com.cn.Context.Model;
/***************************************************************************************
 * *
 * *        File Name        : OpenGateModel.cs
 * *        Creator          : llp
 * *          Remark           : 开闸模型请求类
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/

namespace Fujica.com.cn.Business.Model
{
    /// <summary>
    /// 开闸模型.
    /// </summary>
    /// <remarks>  
    /// 2019.09.17: 修改 枚举引用. llp<br/>
    /// </remarks>
    ///  
    public class OpenGateModel
    {
       
        /// <summary>
        /// 设备标识
        /// </summary>
        public string DeviceIdentify { get; set; }

        /// <summary>
        /// 值班人员  
        /// </summary>
        public string Operator { get; set; }


        /// <summary>
        /// 通行方式   
        /// 出场：1、自动开闸  2、手动开闸  3、免费开闸   4、收费放行  5、场内删除
        /// </summary>
        public OpenTypeEnum OpenType{ get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
    
    }
