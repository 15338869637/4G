using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 角色列表返回实体
    /// </summary>
    public class RoleListResponse : ResponseBaseCommon
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        public List<RoleModel> RoleList { get; set;}
    }

    /// <summary>
    /// 角色模型
    /// </summary>
    public class RoleModel : IBaseModel
    {
        /// <summary>
        /// 唯一识别标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
    }
}