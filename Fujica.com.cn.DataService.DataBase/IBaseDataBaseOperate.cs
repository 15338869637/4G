using Fujica.com.cn.Context.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujica.com.cn.DataService.DataBase
{
    public interface IBaseDataBaseOperate<IBaseModel>: IDisposable //where IBaseModel :new()
    {
        /// <summary>
        /// 将对象保存到数据库
        /// </summary>
        /// <returns></returns>
        bool SaveToDataBase(IBaseModel model);

        /// <summary>
        /// 从数据库读出对象
        /// </summary>
        /// <returns></returns>
        IBaseModel GetFromDataBase(string commandText = "");

        /// <summary>
        /// 从数据库批量读取对象
        /// </summary>
        /// <param name="sqltext"></param>
        /// <returns></returns>
        IList<IBaseModel> GetMostFromDataBase(string commandText = "");

        /// <summary>
        /// 根据分页从数据库批量读取对象
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<IBaseModel> GetFromDataBaseByPage<T>(T model);

        /// <summary>
        /// 在数据库中删除对象
        /// </summary>
        /// <returns></returns>
        bool DeleteInDataBase(IBaseModel model);

        /// <summary>
        /// 批量在数据库删除对象
        /// </summary>
        /// <param name="modlelist"></param>
        /// <returns></returns>
        bool DeleteMostInDataBase(IList<IBaseModel> modlelist);

    }
}
