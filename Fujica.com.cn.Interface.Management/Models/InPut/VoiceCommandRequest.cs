using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 语音指令请求实体
    /// </summary>
    public class VoiceCommandRequest:RequestBase
    {
        /// <summary>
        /// 停车场编码
        /// </summary>
        [Required(ErrorMessage = "停车场编码不能为空")]
        public string ParkingCode { get; set; }

        /// <summary>
        /// 对应车道
        /// </summary>
        public string DrivewayGuid { get; set; }

        /// <summary>
        /// 指令列表
        /// </summary>
        public List<CommandDetialModel> CommandList { get; set; }
    }
}