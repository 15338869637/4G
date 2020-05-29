using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fujica.com.cn.Interface.Management.Models.OutPut
{
    /// <summary>
    /// 角色权限返回实体
    /// </summary>
    public class RolePermissionResponse: ResponseBaseCommon
    {
        /// <summary>
        /// 开通的菜单序号
        /// </summary>
        public List<string> OpenUpMenuSerial { get; set; }

        /// <summary>
        /// 授权停车场编号集合
        /// </summary>
        public List<string> ParkingCodeList { get; set; }

        /// <summary>
        /// 管理员true管理员 false非管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}