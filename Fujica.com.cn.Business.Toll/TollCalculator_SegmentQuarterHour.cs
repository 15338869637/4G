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
    /// 按15分钟收费(可分段)=10
    /// </summary>
    public class TollCalculator_SegmentQuarterHour : IBaseBusiness, ITollCalculator
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
        internal SegmentQuarterHourTollModel model = null;

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

        public TollCalculator_SegmentQuarterHour(ILogger _logger, ISerializer _serializer)
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
            model = m_serializer.Deserialize<SegmentQuarterHourTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 10, GetTollFeesTemplateStr());
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
                model = m_serializer.Deserialize<SegmentQuarterHourTollModel>(_model.TemplateJson);
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
                model = m_serializer.Deserialize<SegmentQuarterHourTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                string DayTimeOut = model.DayTimeOut.ToString().PadLeft(2, '0');

                string NightTimeOut = model.NightTimeOut.ToString().PadLeft(2, '0');
                if (model.FeeSet10 == 1)
                {
                    //16进制
                    DayTimeOut = Convert.ToInt32(model.DayTimeOut).ToString("X").PadLeft(2, '0');
                    NightTimeOut = Convert.ToInt32(model.NightTimeOut).ToString("X").PadLeft(2, '0');
                }
                strtoll = "00" +
                 model.DayBeginTime.ToString("HHmm") + model.DayEndTime.ToString("HHmm")
                 + model.DayFreeMinutes.ToString("X").PadLeft(2, '0')
                   + model.DayFirstTime.ToString("X").PadLeft(2, '0')
                    + Convert.ToInt32(model.DayFirstMoney).ToString("X").PadLeft(4, '0')
                   + model.DayNextTime.ToString("X").PadLeft(2, '0')
                    + Convert.ToInt32(model.DayNextMoney).ToString("X").PadLeft(4, '0')
                    + DayTimeOut
                      + Convert.ToInt32(model.DayMaxMoney).ToString("X").PadLeft(4, '0')
                 + model.NightFreeMinutes.ToString("X").PadLeft(2, '0')
                   + model.NightFirstTime.ToString("X").PadLeft(2, '0')
                    + Convert.ToInt32(model.NightFirstMoney).ToString("X").PadLeft(4, '0')
                   + model.NightNextTime.ToString("X").PadLeft(2, '0')
                    + Convert.ToInt32(model.NightNextMoney).ToString("X").PadLeft(4, '0')
                    + NightTimeOut
                      + Convert.ToInt32(model.NightMaxMoney).ToString("X").PadLeft(4, '0')
                     + Convert.ToInt32(model.DayTopLimit).ToString("X").PadLeft(4, '0')
                     + Convert.ToInt32(model.AmountTopLimit).ToString("X").PadLeft(4, '0')
                  ;
                string EightKz = model.FeeSet8.ToString()
                    + model.FeeSet7.ToString()
                    + model.FeeSet6.ToString()
                    + model.FeeSet5.ToString()
                    + model.FeeSet4.ToString()
                    + model.FeeSet3.ToString()
                    + model.FeeSet2.ToString()
                    + model.FeeSet1.ToString();
                strtoll += string.Format("{0:x}", Convert.ToInt32(EightKz.ToString(), 2)).PadLeft(2, '0');
                //0 角 1元 2  分

                EightKz = "000";
                EightKz += model.FeeSet10 == 1 ? "1" : "0";
                EightKz += model.FeeSet9 == 0 ? "01" : model.FeeSet9 == 1 ? "10" : "00";
                EightKz += model.MonetaryUnit == 0 ? "01" : model.MonetaryUnit == 1 ? "00" : "10";

                strtoll += string.Format("{0:x}", Convert.ToInt32(EightKz.ToString(), 2)).PadLeft(2, '0');
                strtoll += "00";
            }
            return strtoll;
        }
    }
}
