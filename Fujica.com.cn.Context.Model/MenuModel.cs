using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuModel : IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 菜单详情
        /// </summary>
        public List<MenuDetialModel> MenuList { get; set; }
    }

    /// <summary>
    /// 菜单模型详情
    /// </summary>
    public class MenuDetialModel : IBaseModel
    {

        /// <summary>
        /// 菜单序号
        /// </summary>
        public string MenuSerial { get; set; }
        
        /// <summary>
        /// 菜单名
        /// </summary>
        public string MenuName { get; set; }

        /// <summary>
        /// 页面地址
        /// </summary>
        public string PageUrl { get; set; }
    }
}
