using Fujica.com.cn.Context.Model;
using Fujica.com.cn.DataService.DataBase;
using Fujica.com.cn.DataService.RedisCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Base
{
    public interface IBaseBusiness
    {
        /// <summary>
        /// 最近一次调用出错时候的错误描述
        /// </summary>
        string LastErrorDescribe { get; set; }
    }
}
