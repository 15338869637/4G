using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 角色权限管理模型
    /// </summary>
    public class RolePermissionModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 唯一识别标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 权限详情 格式为 菜单序号(三位)+0或1 (0=代表无 1=代表有)
        /// 例如 0011002100300040.... 按四位拆分
        /// </summary>
        public string ContentDetial { get; set; }

        /// <summary>
        /// 授权停车场编号集合 英文逗号分隔
        /// </summary>
        public string ParkingCodeList { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
