using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 操作员列表返回实体
    /// </summary>
    public class UserListResponse: ResponseBaseCommon
    {
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<UserModel> UserList { get; set; }
    }

    /// <summary>
    /// 用户模型
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 所属角色标识
        /// </summary>
        public string RoleGuid { get; set; }

        /// <summary>
        /// 所属角色名称
        /// </summary>
        public string RoleName { get; set; }
    }
}