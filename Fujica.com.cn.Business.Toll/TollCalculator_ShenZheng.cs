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
    /// 深圳算费=3
    /// </summary>
    public class TollCalculator_ShenZheng : IBaseBusiness, ITollCalculator
    {
        private string ShenZhengHolidayBizData = "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";

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
        internal ShenZhengTollModel model = null;

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

        public TollCalculator_ShenZheng(ILogger _logger, ISerializer _serializer)
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
            model = m_serializer.Deserialize<ShenZhengTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 3, GetTollFeesTemplateStr());
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
                model = m_serializer.Deserialize<ShenZhengTollModel>(_model.TemplateJson);
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
                model = m_serializer.Deserialize<ShenZhengTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                strtoll = model.FreeMinutes.ToString().PadLeft(2, '0')
                  + model.RHBeginTime.ToString("HHmm") + model.RHEndTime.ToString("HHmm")
                + Convert.ToInt32(model.RHFirstHour).ToString("X").PadLeft(4, '0')
                   + Convert.ToInt32(model.NRHFirstHour).ToString("X").PadLeft(4, '0')
                   + Convert.ToInt32(model.NWDFirstHour).ToString("X").PadLeft(4, '0')
                  + Convert.ToInt32(model.RHEveryHalfHour).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.NRHEveryHour).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.NWDEveryHour).ToString("X").PadLeft(2, '0')
                   + Convert.ToInt32(model.WDEveryDay).ToString("X").PadLeft(4, '0')
                   + Convert.ToInt32(model.NWDEveryDay).ToString("X").PadLeft(4, '0')
                   + Convert.ToInt32(model.AmountTopLimit).ToString("X").PadLeft(4, '0')
                   + Convert.ToInt32(model.EveryDay).ToString("X").PadLeft(2, '0')
                  ;
                string EightKz = "000000" + model.TollFlag.ToString() + (model.MonetaryUnit == 1 ? "0" : "1");
                strtoll += string.Format("{0:x}", Convert.ToInt32(EightKz.ToString(), 2)).PadLeft(2, '0');
                strtoll += model.Timeout.ToString().PadLeft(2, '0');
                strtoll += "00".PadLeft(18, '0');
            }
            return strtoll + ShenZhengHolidayBizData;
        }
    }
}
