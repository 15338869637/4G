using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{

    /// <summary>
    /// 功能点管理器
    /// </summary>
    public interface IFunctionPointContext
    {
        /// <summary>
        /// 设置功能点
        /// 设置固定车延期方式 0=顺延 1=跳延
        /// 设置固定车过期处理方式 -1=禁止入场 0=看做临时车 大于等于N(N>=1) 过期N天后禁止入场
        /// 设置满位禁止入场车类
        /// 设置参与剩余车位统计的车类
        /// 设置不同颜色车牌的收费车类
        /// 设置月租车延期提醒
        /// 设置储值车充值提醒
        /// </summary>
        /// <returns></returns>
        bool SetFunctionPoint(FunctionPointModel model);

        /// <summary>
        /// 获取功能点
        /// </summary>
        /// <param name="parkingcode"></param>
        /// <returns></returns>
        FunctionPointModel GetFunctionPoint(string projectguid, string parkcode);

    }
}
