using Fujica.com.cn.Business.Base;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Business.Toll
{
    /// <summary>
    /// 小时计费=1
    /// </summary>
    public class TollCalculator_Hourly : IBaseBusiness, ITollCalculator
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
        /// 计费模板
        /// </summary>
        internal HourlyTollModel model = null;

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

        public TollCalculator_Hourly(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        /// <summary>
        /// 声明引用算费sdk
        /// </summary>
        const string dllName = "FJCSDK.dll";
        [DllImport(dllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int Park_CalParkingFee(string beginTime, string endTime, int freeEnable, int templateIndex, string tollTemplate);
        /// <summary>
        /// 计算费用 -1:计算失败
        /// </summary>
        /// <param name="beginTime">计费起始时间</param>
        /// <param name="endTime">计费截止时间</param>
        /// <param name="hasFreeMinute">是否有免费分钟</param>
        /// <returns></returns>
        public decimal GetFees(BillingTemplateModel _model,DateTime beginTime, DateTime endTime, bool hasFreeMinute)
        {
            model = m_serializer.Deserialize<HourlyTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 1, GetTollFeesTemplateStr());
            parktollmoney = parktollmoney / 100;
            return parktollmoney;
        }

        /// <summary>
        /// 获取出场超时时长
        /// </summary>
        /// <returns></returns>
        public int GetLeaveTimeOut(BillingTemplateModel _model)
        {
            try
            {
                model = m_serializer.Deserialize<HourlyTollModel>(_model.TemplateJson);
                return model.LeaveTimeout;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取收费模板字符串
        /// </summary>
        /// <returns></returns>
        public string GetTollFeesTemplateStr(BillingTemplateModel _model = null)
        {
            string strHourtoll = string.Empty;
            if (_model != null)
            {
                model = m_serializer.Deserialize<HourlyTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                
                strHourtoll = model.FreeMinutes.ToString().PadLeft(2, '0') +
                   Convert.ToInt32(model.h1).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h2).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h3).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h4).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h5).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h6).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h7).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h8).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h9).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h10).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h11).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h12).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h13).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h14).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h15).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h16).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h17).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h18).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h19).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h20).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h21).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h22).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h23).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.h24).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.DayAmountTopLimit).ToString("X").PadLeft(2, '0') +
                   Convert.ToInt32(model.AmountTopLimit).ToString("X").PadLeft(4, '0') +
                   (model.MonetaryUnit == 1 ? "00" : "01") + "000000";
            }
            return strHourtoll;
        }
    }
}
