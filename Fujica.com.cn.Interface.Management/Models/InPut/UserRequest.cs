using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.InPut
{
    /// <summary>
    /// 用户账户请求实体
    /// </summary>
    public class UserRequest : RequestBase
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "用户密码不能为空")]
        public string UserPswd { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 角色标识
        /// </summary>
        public string RoleGuid { get; set; }
    }

    /// <summary>
    /// 用户账户修改请求实体
    /// </summary>
    public class ModifyUserRequest : UserRequest
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [Required(ErrorMessage = "用户标识不能为空")]
        public string Guid { get; set; }
    }

    /// <summary>
    /// 用户账户删除请求实体
    /// </summary>
    public class DeleteUserRequest : RequestBase
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [Required(ErrorMessage = "用户标识不能为空")]
        public string Guid { get; set; }
    }

    /// <summary>
    /// 用户修改密码请求实体
    /// </summary>
    public class ModifyUserPasswordRequest : RequestBase
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [Required(ErrorMessage = "用户标识不能为空")]
        public string Guid { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        [Required(ErrorMessage = "用户旧密码不能为空")]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "用户新密码不能为空")]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// 重置账户密码请求实体
    /// </summary>
    public class ResetUserPassword : RequestBase
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        [Required(ErrorMessage = "用户标识不能为空")]
        public string Guid { get; set; }
    }
}