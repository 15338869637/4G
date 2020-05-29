﻿/***************************************************************************************
 * *
 * *        File Name        : GatherAccountManager.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-09-21 
 * *        Remark           : 账户管理器 业务逻辑层
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using static Fujica.com.cn.Context.Model.GatherAccountModel;

namespace Fujica.com.cn.Business.ParkLot
{
    /// <summary>
    /// 账户管理器.
    /// </summary> 
    /// <remarks>
    /// 2019.09.21: 日志参数完善. llp <br/> 
    /// </remarks>  
    public class GatherAccountManager : IBaseBusiness
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        /// <summary>
        /// redis操作类
        /// </summary>
        private IBaseRedisOperate<GatherAccountModel> redisoperate = null;

        /// <summary>
        /// 数据库操作类
        /// </summary>
        private IBaseDataBaseOperate<GatherAccountModel> databaseoperate = null;

        public string LastErrorDescribe
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public GatherAccountManager(ILogger _logger, ISerializer _serializer, IBaseRedisOperate<GatherAccountModel> _redisoperate, IBaseDataBaseOperate<GatherAccountModel> _databaseoperate)
        {
            m_logger = _logger;
            m_serializer = _serializer;
            redisoperate = _redisoperate;
            databaseoperate = _databaseoperate;

            //redisoperate = new GatherAccountRedisCache();
        }


        /// <summary>
        /// 添加收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddGatherAccount(GatherAccountModel model)
        {
            redisoperate.model = model;
            bool flag =  databaseoperate.SaveToDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.SaveToRedis();
        }

        /// <summary>
        /// 修改收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool ModifyGatherAccount(GatherAccountModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.SaveToDataBase(model);
            if (!flag) return false;
            //此时不需要判断redis是否成功，因为修改时redis一定会返回false
            redisoperate.SaveToRedis();
            return true;
        }

        /// <summary>
        /// 删除收款账户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteGatherAccount(GatherAccountModel model)
        {
            redisoperate.model = model;
            bool flag = databaseoperate.DeleteInDataBase(model);
            if (!flag) return false; //数据库不成功就不要往下执行了
            return redisoperate.DeleteInRedis();
        }

        /// <summary>
        /// 获取某收款账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
        public GatherAccountModel GetGatherAccount(string guid)
        {
            GatherAccountModel model = null;
            redisoperate.model = new GatherAccountModel() { Guid = guid };
            model = redisoperate.GetFromRedis();

            //从数据库读
            if (model == null)
            {
                model = databaseoperate.GetFromDataBase(guid);
                //缓存到redis
                if (model != null)
                {
                    redisoperate.model = model;
                    redisoperate.SaveToRedis();
                }
            }

            return model;
        }

        /// <summary>
        /// 获取收款账户列表
        /// </summary>
        /// <param name="projectguid"></param>
        /// <returns></returns>
        public List<GatherAccountModel> GetGatherAccountList(string projectguid)
        {
            //批量数据都从数据库获取
            List<GatherAccountModel> model = databaseoperate.GetMostFromDataBase(projectguid) as List<GatherAccountModel>;
            return model;
        }

        /// <summary>
        /// 获取支付宝账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
        public AliPayAccountModel GetAliPayAccount(string guid)
        {
            GatherAccountModel model = GetGatherAccount(guid);
            if (model != null)
            {
                //List<IBaseModel> isvlist = _model.paymentISV;
                //foreach (var item in isvlist)
                //{
                //    var isv = item as AliPayAccountModel;
                //    if (isv != null)  return isv; //成功转换 表示有账户存在
                //}
                return model.AlipayAccount;
            }
            return null;
        }

        /// <summary>
        /// 获取微信账户
        /// </summary>
        /// <param name="guid">该账户guid</param>
        /// <returns></returns>
        public WeChatAccountModel GetWeChatAccount(string guid)
        {
            GatherAccountModel model = GetGatherAccount(guid);
            if (model != null)
            {
                //List<IBaseModel> isvlist = _model.paymentISV;
                //foreach (var item in isvlist)
                //{
                //    var isv = item as WeChatAccountModel;
                //    if (isv != null) return isv; //成功转换 表示有账户存在
                //}
                return model.WechatAccount;
            }
            return null;
        }

    }
}
