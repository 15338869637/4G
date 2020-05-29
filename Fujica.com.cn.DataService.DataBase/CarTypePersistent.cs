using Fujica.com.cn.BaseConnect;
using Fujica.com.cn.Context.Model;
using Fujica.com.cn.Logger;
using Fujica.com.cn.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
/***************************************************************************************
 * *
 * *        File Name        : CarTypePersistent.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-18 
 * *        Remark           : 卡类型数据库交互管理
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.DataService.DataBase
{
    /// <remarks>
    /// 2019.10.18: 修改.新增注释信息,方法命名规则修改 LLP <br/> 
    /// </remarks>
    public class CarTypePersistent : IBaseDataBaseOperate<CarTypeModel>, IExtenDataBaseOperate<CarTypeModel>
    {

        /// <summary>
        /// 日志记录器
        /// </summary>
 
        internal readonly ILogger m_logger;

        /// <summary>
        /// 序列化器
        /// </summary>
        internal readonly ISerializer m_serializer = null;

        public CarTypePersistent(ILogger _logger, ISerializer _serializer)
        {
            m_logger = _logger;
            m_serializer = _serializer;
        }

        public bool DeleteInDataBase(CarTypeModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("delete from t_cartype where guid='{0}'", model.Guid));
            if (ret > 0) return true;
            return false;
        }

        public bool DeleteMostInDataBase(IList<CarTypeModel> modlelist)
        {
            return false;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public CarTypeModel GetFromDataBase(string guid)
        { 
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_cartype where guid='{0}'", guid));
            if (table.Rows.Count > 0)
            {
                CarTypeModel model = new CarTypeModel();
                model.ProjectGuid = (string)table.Rows[0]["projectGuid"];
                model.ParkCode = (string)table.Rows[0]["parkCode"];
                model.Guid = (string)table.Rows[0]["guid"];
                model.CarTypeName = (string)table.Rows[0]["carTypeName"];
                model.CarType = (CarTypeEnum)table.Rows[0]["carType"];
                model.DefaultType = (table.Rows[0]["defaultType"].ToString() == "0" ? false : true);
                model.Enable = (table.Rows[0]["enable"].ToString() == "0" ? false : true);
                return model;
            }
            return null;
        }

        public IList<CarTypeModel> GetFromDataBaseByPage<T>(T model)
        {
            throw new NotImplementedException();
        }

        public IList<CarTypeModel> GetMostFromDataBase(string commandText)
        {
            List<CarTypeModel> model = new List<CarTypeModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            // DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_cartype where parkCode='{0}' and projectGuid='{1}'  order by carType ", parkcode,projectguid));
            // DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_cartype where parkCode='{0}'  order by carType ", commandText));
            DataTable table = dbhelper.ExecuteDataTable(commandText);
            foreach (DataRow item in table.Rows)
            {
                model.Add(new CarTypeModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    Guid = (string)item["guid"],
                    CarTypeName = (string)item["carTypeName"],
                    CarType = (CarTypeEnum)item["carType"],
                    DefaultType = (item["defaultType"].ToString() == "0" ? false : true),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Idx = (string)item["idx"],
                });
            }
            return model;
        }

        public IList<CarTypeModel> GetMostFromDataBase(string parkcode,string projectguid)
        {
            List<CarTypeModel> model = new List<CarTypeModel>();
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
             DataTable table = dbhelper.ExecuteDataTable(string.Format("select * from t_cartype where parkCode='{0}' and projectGuid='{1}'  order by carType ", parkcode,projectguid));
              foreach (DataRow item in table.Rows)
            {
                model.Add(new CarTypeModel()
                {
                    ProjectGuid = (string)item["projectGuid"],
                    ParkCode = (string)item["parkCode"],
                    Guid = (string)item["guid"],
                    CarTypeName = (string)item["carTypeName"],
                    CarType = (CarTypeEnum)item["carType"],
                    DefaultType = (item["defaultType"].ToString() == "0" ? false : true),
                    Enable = (item["enable"].ToString() == "0" ? false : true),
                    Idx = (string)item["idx"],
                });
            }
            return model;
        }

        public bool SaveToDataBase(CarTypeModel model)
        {
            //判断当前请求是新增还修改
            bool isAddOrEdit = CarTypeIsAddOrEdit(model.Guid);
            //新增或者修改操作
            if (isAddOrEdit)
            {
                if (model.Idx== null)
                {
                    string Idx = GetIdx(model);
                    model.Idx = Idx;
                }
                //如果是新增操作，则判断当前的idx字段是否有重复
                bool idxExists = CarTypeIdxExists(model.ProjectGuid, model.ParkCode, model.Guid, model.Idx);
                //如果重复则直接返回false，添加失败
                if (idxExists)
                {
                    return false;
                }
                else
                { 
                    //执行新增命令
                    return AddDataBase(model);
                }
            }
            else
            {
                //执行修改命令
                return EditDataBase(model);
            }
        }

        /// <summary>
        /// 获取每个车类的最大idx
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public string GetIdx(CarTypeModel model)
        { 
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int carType = -1;
            int idxType = -1;
            string idx;
            switch (model.CarType)
            {
                // 车类 0=时租车 1=月租车 2=储值车 3=贵宾车
                case CarTypeEnum.TempCar:
                    carType = 0;
                    idxType = 1;
                    break;
                case CarTypeEnum.MonthCar:
                    carType = 1;
                    idxType = 3;
                    break;
                case CarTypeEnum.ValueCar:
                    carType = 2;
                    idxType = 2;
                    break;
                case CarTypeEnum.VIPCar:
                    carType = 3;
                    idxType = 4;
                    break;

            }
            DataTable table = dbhelper.ExecuteDataTable(string.Format("SELECT max(idx) FROM t_cartype where parkCode='{0}' and carType={1} and projectGuid='{2}' ", model.ParkCode, carType, model.ProjectGuid));
            if (table.Rows[0][0].ToString() == "")
            {
                idx = "A";
            }
            else
            {
                idx = StringIncreaseOne(table.Rows[0][0].ToString());
            } 
            return idxType+idx.Substring(idx.Length - 1, 1);


        }
        /// <summary>
        /// 通过ASCII码值，对字符串自增1
        /// </summary>
        /// <param name="pStr">输入字符串</param>
        /// <returns></returns>
        public static string StringIncreaseOne(string pStr)
        {
            var vRetStr = pStr;
            if (0 == pStr.Length)
            {
                vRetStr = "1";
            }
            else
            {
                // 将最后一个字符与之前的字符串分开
                string vOtherStr = pStr.Substring(1, pStr.Length - 1);
                int vIntChar = (int)pStr[pStr.Length - 1]; //转ASCII码值
                if (48 <= vIntChar && vIntChar <= 57) //是数字（0 - 9）
                {
                    vIntChar++; //自增1
                    if (vIntChar == 58) // 进一位
                    {
                        vIntChar = 48;
                        vOtherStr = StringIncreaseOne(vOtherStr);
                    }
                }
                else if (65 <= vIntChar && vIntChar <= 90) //是字母（A - Z）
                {
                    vIntChar++; //自增1
                    if (vIntChar == 91)
                    {
                        vIntChar = 65;
                        vOtherStr = StringIncreaseOne(vOtherStr);
                    }
                }
                else if (97 <= vIntChar && vIntChar <= 122) //是字母（a - z）
                {
                    vIntChar++; //自增1
                    if (vIntChar == 123)
                    {
                        vIntChar = 97;
                        vOtherStr = StringIncreaseOne(vOtherStr);
                    }
                }
                else // 其它字符 -> 跳过
                {
                    vOtherStr = StringIncreaseOne(vOtherStr);
                }
                vRetStr = vOtherStr+(char)vIntChar; 
            } 
            return vRetStr;
        }


        /// <summary>
        /// 切换默认值(车类)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool ToggleValue(CarTypeModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            int ret = dbhelper.ExecuteNonQuery(string.Format("update t_cartype set defaultType=1 where guid='{0}'", model.Guid)); //更新相同的
            if (ret > 0)
            {
                ret = dbhelper.ExecuteNonQuery(string.Format("update t_cartype set defaultType=0 where guid<>'{0}' and parkCode='{1}'", model.Guid, model.ParkCode)); //去掉原来的
                if(ret>0) return true;
            }
            return false;
        }

        public string GetGuidByKey(string key)
        {
            return "";
        }

        public List<CarTypeModel> GetListByKeys(string key)
        {
            throw new NotImplementedException();
        }

        #region 内部业务私有方法

        /// <summary>
        /// 判断当前保存是新增还是修改
        /// </summary>
        /// <param name="guid">卡类唯一标识</param>
        /// <returns>true:新增 false:修改</returns>
        private bool CarTypeIsAddOrEdit(string guid)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = $"SELECT COUNT(1) FROM t_cartype WHERE guid = '{guid}'";
            int countResult = Convert.ToInt32(dbhelper.ExecuteScalar(commandtext));

            return countResult > 0 ? false : true;
        }

        /// <summary>
        /// 验证当前车场的车类标识是否重复
        /// </summary>
        /// <param name="projectGuid">项目唯一标识</param>
        /// <param name="parkingCode">停车场编码</param>
        /// <param name="idx">车类标识</param>
        /// <returns>true:存在  false:不存在</returns>
        private bool CarTypeIdxExists(string projectGuid, string parkingCode,string guid, string idx)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = $"SELECT COUNT(1) FROM t_cartype WHERE projectGuid = '{projectGuid}' AND parkCode = '{parkingCode}' AND idx = '{idx}' ";

            int countResult = Convert.ToInt32(dbhelper.ExecuteScalar(commandtext));
            
            return countResult > 0 ? true : false;
        }
        
        /// <summary>
        /// 新增卡类信息
        /// </summary>
        /// <param name="model">卡类实体</param>
        /// <returns></returns>
        private bool AddDataBase(CarTypeModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"insert into t_cartype(projectGuid,parkCode,guid,carTypeName,carType,defaultType,enable,idx)
                                    values(@projectGuid,@parkCode,@guid,@carTypeName,@carType,@defaultType,@enable,@idx)";

            DbParameter projectGuid = dbhelper.factory.CreateParameter();
            projectGuid.ParameterName = "@projectGuid";
            projectGuid.Value = model.ProjectGuid;

            DbParameter parkCode = dbhelper.factory.CreateParameter();
            parkCode.ParameterName = "@parkCode";
            parkCode.Value = model.ParkCode;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter carTypeName = dbhelper.factory.CreateParameter();
            carTypeName.ParameterName = "@carTypeName";
            carTypeName.Value = model.CarTypeName;

            DbParameter carType = dbhelper.factory.CreateParameter();
            carType.ParameterName = "@carType";
            carType.Value = model.CarType;

            DbParameter defaultType = dbhelper.factory.CreateParameter();
            defaultType.ParameterName = "@defaultType";
            defaultType.Value = model.DefaultType;

            DbParameter enable = dbhelper.factory.CreateParameter();
            enable.ParameterName = "@enable";
            enable.Value = model.Enable;

            DbParameter idx = dbhelper.factory.CreateParameter();
            idx.ParameterName = "@idx";
            idx.Value = model.Idx;

            DbParameter[] parameter = new DbParameter[] { projectGuid, parkCode, guid, carTypeName, carType, defaultType, enable, idx };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }

        /// <summary>
        /// 修改卡类信息
        /// </summary>
        /// <param name="model">卡类实体</param>
        /// <returns></returns>
        public bool EditDataBase(CarTypeModel model)
        {
            MysqlHelper dbhelper = new MysqlHelper("name=parklotManager", "MySql.Data.MySqlClient");
            string commandtext = @"update t_cartype set carTypeName = @carTypeName , carType = @carType , idx = @idx where guid = @guid";

            DbParameter carTypeName = dbhelper.factory.CreateParameter();
            carTypeName.ParameterName = "@carTypeName";
            carTypeName.Value = model.CarTypeName;

            DbParameter carType = dbhelper.factory.CreateParameter();
            carType.ParameterName = "@carType";
            carType.Value = model.CarType;

            DbParameter idx = dbhelper.factory.CreateParameter();
            idx.ParameterName = "@idx";
            idx.Value = model.Idx;

            DbParameter guid = dbhelper.factory.CreateParameter();
            guid.ParameterName = "@guid";
            guid.Value = model.Guid;

            DbParameter[] parameter = new DbParameter[] { carTypeName, carType, idx, guid };
            return dbhelper.ExecuteNonQuery(commandtext, parameter) > 0 ? true : false;
        }

        
        #endregion

    }
}
