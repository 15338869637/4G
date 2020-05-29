using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    public interface IParkLotContext
    {
        /// <summary>
        /// 添加车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddParkLot(ParkLotModel model);

        /// <summary>
        /// 修改车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyParkLot(ParkLotModel model);

        /// <summary>
        /// 注销车场
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool CancelParkLot(ParkLotModel model);

        /// <summary>
        /// 根据停车场编码获取停车场
        /// </summary>
        /// <param name="parkingCode">停车场编码</param>
        /// <returns>停车场实体</returns>
        ParkLotModel GetParkLot(string parkingCode);

        /// <summary>
        /// 根据项目编码获取所有停车场
        /// </summary>
        /// <param name="projectGuid">项目编码</param>
        /// <returns>所有停车场集合</returns>
        List<ParkLotModel> GetParklotAll(string projectGuid);
        ///<summary>
        /// 补录数据添加数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddRecordToDatabase(AddRecordModel model);

        /// <summary>
        /// 车牌修正 添加数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddCarnoRecorddatabaseoperate(CorrectCarnoModel model);
         
    }
}
