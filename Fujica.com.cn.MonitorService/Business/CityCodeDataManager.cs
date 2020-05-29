using Fujica.com.cn.BaseConnect;
using StackExchange.Redis;
using System.Collections.Generic;
/***************************************************************************************
 * *
 * *        File Name        : CityCodeDataManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 城市区号数据据管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.MonitorService.Business
{

    /// <summary>
    /// 城市区号数据管理
    /// </summary>
    public static class CityCodeDataManager
    {
        public static List<string> GetAllCityCodeList()
        {
            IDatabase db;
            db = RedisHelper.GetDatabase(0);
            
            List<string> list = null;
            RedisValue[] redisList = db.SetMembers("CityCodeList");

            if (redisList != null)
            {
                list = new List<string>();
                foreach (var item in redisList)
                {
                    list.Add(item.ToString());
                }
            }
            return list;
        }


    }
}
