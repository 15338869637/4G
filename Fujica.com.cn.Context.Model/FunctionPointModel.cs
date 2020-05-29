using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.Model
{
    /// <summary>
    /// 功能点模型(其它设置页)
    /// </summary>
    public class FunctionPointModel: IBaseModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectGuid { get; set; }

        /// <summary>
        /// 停车场编码
        /// </summary>
        public string ParkCode { get; set; }

        /// <summary>
        /// 月卡延期模式  0=顺延 1=跳延(仅当 cartype=1 也即月租车时候有效)
        /// </summary>
        public int PostponeMode { get; set; }

        /// <summary>
        /// 月卡过期处理模式 -1=禁止入场 0=看做临时车 大于等于N(N>=1) 过期N天后禁止入场(仅当 cartype=1 也即月租车时候有效)
        /// </summary>
        public int PastDueMode { get; set; }

        /// <summary>
        /// 满位禁入车类guid集合
        /// </summary>
        public string[] BarredEntryCarTypeOnParkingFull { get; set; }

        /// <summary>
        /// 参与剩余车位统计的车类的guid集合
        /// </summary>
        public string[] RemainingSpaceCarType { get; set; }

        /// <summary>
        /// 蓝牌车 所属车类 收费(空表示按默认的)
        /// </summary>
        public string BluePlateCarTypeGuid { get; set; }

        /// <summary>
        /// 黄牌车 所属车类 收费(空表示按默认的)
        /// </summary>
        public string YellowPlateCarTypeGuid { get; set; }

        /// <summary>
        /// 白牌车 所属车类 收费(空表示按默认的)
        /// </summary>
        public string WhitePlateCarTypeGuid { get; set; }

        /// <summary>
        /// 绿牌车 所属车类 收费(空表示按默认的)
        /// </summary>
        public string GreenPlateCarTypeGuid { get; set; }

        /// <summary>
        /// 黑牌车 所属车类 收费(空表示按默认的)
        /// </summary>
        public string BlackPlateCarTypeGuid { get; set; }

        /// <summary>
        /// 月租车低于多少天延期提醒
        /// </summary>
        public int MinDays { get; set; }

        /// <summary>
        /// 储值车低于多少元充值提醒
        /// </summary>
        public int MinBalance { get; set; }

        /// <summary>
        /// 手动开闸车类guid集合
        /// </summary>
        public string[] ManualOpenGateGuid { get; set; }
    }
}
