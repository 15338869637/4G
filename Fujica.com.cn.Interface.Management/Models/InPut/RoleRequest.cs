using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 角色请求实体
    /// </summary>
    public class RoleRequest : RequestBase
    {
        /// <summary>
        /// 角色标识
        /// </summary>
        [Required(ErrorMessage = "角色标识不能为空")]
        public string Guid { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [Required(ErrorMessage = "角色名不能为空")]
        public string RoleName { get; set; }

    }

    /// <summary>
    /// 角色修改请求实体
    /// </summary>
    public class ModifyRoleRequest : RoleRequest
    {        
        /// <summary>
        /// 菜单序号
        /// </summary>
        [Required(ErrorMessage = "角色菜单序列不能为空")]
        public List<string> MenuSerials { get; set; }

        /// <summary>
        /// 授权停车场编号集合
        /// </summary>
        public List<string> ParkingCodeList { get; set; }
    }
}