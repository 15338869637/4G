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
    /// 简易分段算费=5
    /// </summary>
    public class TollCalculator_SimpleSegment : IBaseBusiness, ITollCalculator
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
        internal SimpleSegmentTollModel model = null;

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

        public TollCalculator_SimpleSegment(ILogger _logger, ISerializer _serializer)
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
            model = m_serializer.Deserialize<SimpleSegmentTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 5, GetTollFeesTemplateStr());
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
                model = m_serializer.Deserialize<SimpleSegmentTollModel>(_model.TemplateJson);
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
            string strtoll = string.Empty;
            if (_model != null)
            {
                model = m_serializer.Deserialize<SimpleSegmentTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                strtoll = model.FreeMinutes.ToString("X").PadLeft(2, '0') +
                 model.DayBeginTime.ToString("HHmm") + model.DayEndTime.ToString("HHmm")
               + Convert.ToInt32(model.DayMoney).ToString("X").PadLeft(2, '0')
                + Convert.ToInt32(model.NightMoney).ToString("X").PadLeft(2, '0')
                + Convert.ToInt32(model.DayMaxMoney).ToString("X").PadLeft(4, '0')
                + MinuteToHour(model.DayMinute)
                + Convert.ToInt32(model.DayFirstMoney).ToString("X").PadLeft(2, '0')
                + MinuteToHour(model.NightMinute)
                + Convert.ToInt32(model.NightFirstMoney).ToString("X").PadLeft(2, '0');
                double nPow = System.Math.Pow(Convert.ToDouble(2), Convert.ToDouble(model.SpanProcessing));
                int nSpan = (int)nPow;
                strtoll += nSpan.ToString("X").PadLeft(2, Convert.ToChar("0"));
                strtoll += (model.MonetaryUnit == 1 ? "00" : "01") + "00".PadLeft(30, '0');
            }
            return strtoll;
        }

        private string MinuteToHour(int nMinute)
        {
            int nHour = (int)(nMinute / 60);
            int nMin = nMinute % 60;
            return nHour.ToString().PadLeft(2, Convert.ToChar("0")) + nMin.ToString().PadLeft(2, Convert.ToChar("0"));

        }
    }
}
