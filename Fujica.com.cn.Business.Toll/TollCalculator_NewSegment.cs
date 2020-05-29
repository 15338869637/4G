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
    /// 新分段算费模型=9
    /// </summary>
    public class TollCalculator_NewSegment : IBaseBusiness, ITollCalculator
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
        internal NewSegmentTollModel model = null;

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

        public TollCalculator_NewSegment(ILogger _logger, ISerializer _serializer)
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
            model = m_serializer.Deserialize<NewSegmentTollModel>(_model.TemplateJson);
            decimal parktollmoney = -1;
            parktollmoney = Park_CalParkingFee(beginTime.ToString("yyyy-MM-dd HH:mm:ss"), endTime.ToString("yyyy-MM-dd HH:mm:ss"), !hasFreeMinute ? 1 : 0, 9, GetTollFeesTemplateStr());
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
                model = m_serializer.Deserialize<NewSegmentTollModel>(_model.TemplateJson);
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
                model = m_serializer.Deserialize<NewSegmentTollModel>(_model.TemplateJson);
            }
            if (model != null)
            {
                try
                {
                    string time1 = model.BeginTime.Split(':')[0].PadLeft(2, '0');
                    string time2 = model.BeginTime.Split(':')[1].PadLeft(2, '0');
                    string time3 = model.EndTime.Split(':')[0].PadLeft(2, '0');
                    string time4 = model.EndTime.Split(':')[1].PadLeft(2, '0');
                    strtoll = model.FreeMinutes.ToString("X").PadLeft(2, '0') + time1 + time2 + time3 + time4 + model.DayTimeSt1.ToString("X").PadLeft(2, '0') +
                        model.DayTimeFt1.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.DayTimeFee1).ToString("X").PadLeft(2, '0') +
                        model.DayTimeSt2.ToString("X").PadLeft(2, '0') +
                        model.DayTimeFt2.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.DayTimeFee2).ToString("X").PadLeft(2, '0') +
                        //model.DayTimeSt3.ToString("X").PadLeft(2, '0') +
                        model.DayTimeFt3.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.DayTimeFee3).ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.DayTimeMaxAmount).ToString("X").PadLeft(4, '0')
                        + model.NightTimeSt1.ToString("X").PadLeft(2, '0') +
                        model.NightTimeFt1.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.NightTimeFee1).ToString("X").PadLeft(2, '0') +
                        model.NightTimeSt2.ToString("X").PadLeft(2, '0') +
                        model.NightTimeFt2.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.NightTimeFee2).ToString("X").PadLeft(2, '0') +
                        //model.NightTimeSt3.ToString("X").PadLeft(2, '0') +
                        model.NightTimeFt3.ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.NightTimeFee3).ToString("X").PadLeft(2, '0') +
                        Convert.ToInt32(model.NightTimeMaxAmount).ToString("X").PadLeft(4, '0') +
                        model.Timeout.ToString().PadLeft(2, '0') +
                        Convert.ToInt32(model.DayTopLimit).ToString("X").PadLeft(4, '0');
                    string bin = "000" + model.CrossSegmentsType + model.CrossSegmentsFreeType + 
                        model.FreeTreatType + model.FreeTreatTypeIsEnable + model.NoSegmentedIsEnable;
                    strtoll += string.Format("{0:x}", Convert.ToInt32(bin, 2)).PadLeft(2, '0');
                    bin = model.MonetaryUnit == 1 ? "00000000" : "00000001";
                    strtoll += string.Format("{0:x}", Convert.ToInt32(bin, 2)).PadLeft(2, '0');
                    strtoll += "0000";
                }
                catch
                {
                }
            }
            return strtoll;
        }
    }
}
