using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
/***************************************************************************************
 * *
 * *        File Name        : MenuPersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 菜单数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class MenuPersistent : IBaseDataBaseOperate<MenuModel>
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public MenuPersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(MenuModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_projectmenu where projectGuid='{0}'", model.ProjectGuid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<MenuModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public MenuModel GetFromDataBase(string projectGuid)
        {
            MenuModel model = new MenuModel();
            model.MenuList = new List<MenuDetialModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_projectmenu where projectGuid='{0}'", projectGuid));
            foreach (DataRow item in table.Rows)
            {
                model.ProjectGuid = projectGuid;
                model.MenuList.Add(new MenuDetialModel() {
                    MenuName = item["menuName"].ToString(),
                    MenuSerial = item["menuSerial"].ToString(),
                    PageUrl = (item["pageUrl"] == null ? "" : item["pageUrl"].ToString())
                });
            }
            return model;
        }

        public IList<MenuModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<MenuModel> GetMostFromDataBase(string projectGuid)
        {
            return new List<MenuModel>();
        }

        public bool SaveToDataBase(MenuModel model)
        {
            int flag = 0;
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            foreach (MenuDetialModel item in model.MenuList)
            {
                string commandtext = @"replace into t_projectmenu(projectGuid,menuSerial,menuName,pageUrl) 
                                   values(@projectGuid,@menuSerial,@menuName,@pageUrl)";

                DbParameter projectGuid = dbhelper.factory.CreateParameter();
                projectGuid.ParameterName = "@projectGuid";
                projectGuid.Value = model.ProjectGuid;

                DbParameter menuSerial = dbhelper.factory.CreateParameter();
                menuSerial.ParameterName = "@menuSerial";
                menuSerial.Value = item.MenuSerial;

                DbParameter menuName = dbhelper.factory.CreateParameter();
                menuName.ParameterName = "@menuName";
                menuName.Value = item.MenuName;

                DbParameter pageUrl = dbhelper.factory.CreateParameter();
                pageUrl.ParameterName = "@pageUrl";
                pageUrl.Value = item.PageUrl;

                DbParameter[] parameter = new DbParameter[] { projectGuid, menuSerial, menuName, pageUrl };

                flag = dbhelper.ExecuteNonQuery(commandtext, parameter);
                if (flag == 0) return false;//有一项失败则返回失败
            }
            return true;
        }
    }
}
