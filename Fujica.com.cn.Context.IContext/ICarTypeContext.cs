using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.Context.IContext
{
    /// <summary>
    /// 车类管理
    /// </summary>
    public interface ICarTypeContext
    {
        /// <summary>
        /// 添加车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool AddCarType(CarTypeModel model);

        /// <summary>
        /// 修改车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool ModifyCarType(CarTypeModel model);

        /// <summary>
        /// 删除车类
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool DeleteCarType(CarTypeModel model);

        /// <summary>
        /// 获取某车类
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        CarTypeModel GetCarType(string guid);

        /// <summary>
        /// 某车场的所有车类
        /// </summary>
        /// <param name="parkcode"></param>
        /// <returns></returns>
        List<CarTypeModel> AllCarType(string parkcode,string projectguid);

        /// <summary>
        /// 设置默认车类（临时车用）
        /// </summary>
        /// <param name="guid">车类guid</param>
        /// <returns></returns>
        bool SetDefaultCarType(string guid);

    }
}
