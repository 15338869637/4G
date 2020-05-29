using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.DataUpload.Models.OutPut
{
    /// <summary>
    /// 业务响应公共基类 
    /// </summary>
    public class ResponseBaseCommon
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 响应结果编码
        /// </summary>
        public int MessageCode { get; set; }
        /// <summary>
        /// 响应结果描述
        /// </summary>
        public string MessageContent { get; set; }
    }

    /// <summary>
    /// 业务响应基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseBase<T> : ResponseBaseCommon where T : IBaseModel
    {
        /// <summary>
        /// 业务响应体 仅当IsSuccess为true时有效
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// 业务响应基类集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseBaseList<T> : ResponseBaseCommon where T : IBaseModel
    {
        /// <summary>
        /// 业务响应体 仅当IsSuccess为true时有效
        /// </summary>
        public List<T> Result { get; set; }
    }
}