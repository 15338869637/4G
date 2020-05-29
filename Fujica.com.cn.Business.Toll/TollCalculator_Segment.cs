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
    /// 分段计费=2
    /// </summary>
    public class TollCalculator_Segment : IBaseBusiness, ITollCalculator
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
        internal SegmentTollModel model = null;

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

        public TollCalculator_Segment(ILogger _logger, ISerializer _serializer)
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
            model = m_serializer.Deserialize<SegmentTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 2, GetTollFeesTemplateStr());
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
                model = m_serializer.Deserialize<SegmentTollModel>(_model.TemplateJson);
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
                model = m_serializer.Deserialize<SegmentTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                strtoll = model.BeginTime1.ToString("HHmm") +
                  model.BeginTime2.ToString("HHmm") +
                  (model.BeginTime3.ToString("HHmm") == "0000" ? "FFFF" : model.BeginTime3.ToString("HHmm"))
                   + model.FreeMinute1.ToString("X").PadLeft(2, '0')
                      + model.FreeMinute2.ToString("X").PadLeft(2, '0')
                         + model.FreeMinute3.ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.Fee1).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.Fee2).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.Fee3).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.MaxDayMoney).ToString("X").PadLeft(2, '0')
                   + MinuteToHour(model.SameTimeFreeMinute)
                   + Convert.ToInt32(model.SameTimeMoney1).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.SameTimeMoney2).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.SameTimeMoney3).ToString("X").PadLeft(2, '0')
                    + MinuteToHour(model.SpanFreeMinute)
                    + ((int)System.Math.Pow(Convert.ToDouble(2), Convert.ToDouble(model.SpanNoOut))).ToString("X").PadLeft(2, Convert.ToChar("0"))
                    + Convert.ToInt32(model.SpanNoOutMoney).ToString("X").PadLeft(2, '0')
                    + ((int)System.Math.Pow(Convert.ToDouble(2), Convert.ToDouble(model.SpanOut))).ToString("X").PadLeft(2, Convert.ToChar("0"))
                    + Convert.ToInt32(model.SpanOutMoney).ToString("X").PadLeft(2, '0')

                    + MinuteToHour(model.OverFreeMinute)
                    + ((int)System.Math.Pow(Convert.ToDouble(2), Convert.ToDouble(model.OverNoOut))).ToString("X").PadLeft(2, Convert.ToChar("0"))
                    + Convert.ToInt32(model.OverNoOutMoney).ToString("X").PadLeft(2, '0')
                    + ((int)System.Math.Pow(Convert.ToDouble(2), Convert.ToDouble(model.OverOut))).ToString("X").PadLeft(2, Convert.ToChar("0"))
                    + Convert.ToInt32(model.OverOutMoney).ToString("X").PadLeft(2, '0');
                string strTemp = "";
                if (model.MonetaryUnit == 1) strTemp = "0000"; else strTemp = "0001";
                if (model.OverMaxValue == 1) strTemp += "1"; else strTemp += "0";
                if (model.SpanAccount == 1) strTemp += "1"; else strTemp += "0";
                strTemp += model.OverFlag.ToString();
                if (model.FreeMiniteAll == 1) strTemp += "1"; else strTemp += "0";
                strtoll += string.Format("{0:x}", Convert.ToInt32(strTemp.ToString(), 2)).PadLeft(2, '0') + "00";

            }
            return strtoll;
        }

        /// <summary>
        /// 分钟转小时
        /// </summary>
        /// <param name="nMinute"></param>
        /// <returns></returns>
        private string MinuteToHour(int nMinute)
        {
            int nHour = (int)(nMinute / 60);
            int nMin = nMinute % 60;
            return nHour.ToString().PadLeft(2, Convert.ToChar("0")) + nMin.ToString().PadLeft(2, Convert.ToChar("0"));

        }
    }
}
