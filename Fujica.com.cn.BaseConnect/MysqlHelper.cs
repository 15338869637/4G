using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text;
/***************************************************************************************
 * *
 * *        File Name        : MysqlHelper.cs
 * *        Creator          : llp
 * *        Create Time      : 2019-10-17 
 * *        Remark           : 基础类，读取Mysql配置文件封装
 * *
 * *  Copyright (c) 深圳市富士智能系统有限公司.  All rights reserved. 
 * ***************************************************************************************/
namespace Fujica.com.cn.BaseConnect
{
    /// <summary>
    /// mysql帮助类
    /// </summary>
    /// <remarks> 
    /// 2019.10.17: 修改: 接口命名修改. llp <br/>  
    /// </remarks>  
    public class MysqlHelper: IDisposable
    {
        public DbProviderFactory factory { get { return _factory; } }

        private DbProviderFactory _factory;
        private ObjectPool<DbConnectionWrapper, DbConnection> pool = null;
        /// <summary>
        /// 存储实体属性的字典,key:泛型类型名称,value:泛型类型属性
        /// </summary>
        private static ConcurrentDictionary<string, PropertyInfo[]> Properties = new ConcurrentDictionary<string, PropertyInfo[]>();
        /// <summary>
        /// 超时时间 10分钟
        /// </summary>
        public static int CommandTimeOut = 600;
        private string _connectionString = string.Empty; //连接串
        private string _providerInvariantName = string.Empty; //提供数据库连接程序的固定名称
        private DbConnectionStringBuilder _dcsb; //连接字符串构造器

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbConnectionString">数据库连接字符串</param>
        /// <param name="providerInvariantName">提供程序的固定名称</param>
        public MysqlHelper(string dbConnectionString, string providerInvariantName)
        {
            //pool = new ConnectionPool(dbConnectionString, providerInvariantName);
            //factory = pool.GetDbProviderFactory();
   
            if (dbConnectionString == null)
            {
                throw new ArgumentNullException("dbConnectionString");
            }
            var flag = dbConnectionString.ToLower().StartsWith("name=");
            if (flag)
            {
                //从配置文件读取
                var name = dbConnectionString.Substring(5);
                _connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            else
            {
                //直接赋值
                _connectionString = dbConnectionString;
            }
            _providerInvariantName = providerInvariantName;

            _factory = DbProviderFactories.GetFactory(providerInvariantName);
            _dcsb = factory.CreateConnectionStringBuilder();
            _dcsb.ConnectionString = _connectionString;

            int _minPoolSize = 0, _maxPoolSize = 0;
            //最大池对象
            if (_dcsb.ContainsKey("maxpoolsize"))
            {
                if (!int.TryParse(_dcsb["maxpoolsize"].ToString(), out _maxPoolSize)) _maxPoolSize = 4;
            }
            //最小池对象
            if (_dcsb.ContainsKey("minpoolsize"))
            {
                if (!int.TryParse(_dcsb["minpoolsize"].ToString(), out _minPoolSize))  _minPoolSize = 1;
            }
            if (_minPoolSize > _maxPoolSize)
            {
                _maxPoolSize = _minPoolSize;
                _minPoolSize = 0;
            }
            _dcsb["maxpoolsize"] = _maxPoolSize;
            _dcsb["minpoolsize"] = _minPoolSize;
            _connectionString = _dcsb.ConnectionString;

            pool = new ObjectPool<DbConnectionWrapper, DbConnection>();
            pool.Init(_minPoolSize, _maxPoolSize, _connectionString, _providerInvariantName, 600);
        }

        /// <summary>
        /// 从数据库连接池中获取数据库连接对象
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            return pool.GetObjectFromPool();
        }

        /// <summary>
        /// 释放数据库连接对象到数据库连接池中
        /// </summary>
        public void FreeConnection(DbConnection connection)
        {
            pool.FreeObjectToPool(connection);
        }

        #region 数据库操作

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parms)
        {
            return ExecuteNonQuery(commandText, CommandType.Text, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回影响的行数
        /// </summary>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回影响的行数</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parms)
        {
            DbCommand command = factory.CreateCommand();
            try
            {
                PrepareCommand(command, commandText, commandType, parms);
                int retval = command.ExecuteNonQuery();
                return retval;
            }
            finally
            {
                FreeConnection(command.Connection);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText, params DbParameter[] parms)
        {
            return ExecuteScalar(commandText, CommandType.Text, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一行第一列
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        protected object ExecuteScalar(string commandText, CommandType commandType, params DbParameter[] parms)
        {
            DbCommand command = factory.CreateCommand();
            try
            {
                PrepareCommand(command, commandText, commandType, parms);
                object retval = command.ExecuteScalar();
                return retval;
            }
            finally
            {
                FreeConnection(command.Connection);
            }
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            var dataSet = ExecuteDataSet(commandText, CommandType.Text);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(string commandText, params DbParameter[] parms)
        {
            var dataSet = ExecuteDataSet(commandText, CommandType.Text, parms);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 执行SQL语句,返回结果集中的第一个数据表
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集中的第一个数据表</returns>
        public DataTable ExecuteDataTable(string commandText, CommandType commandType, params DbParameter[] parms)
        {
            var dataSet = ExecuteDataSet(commandText, commandType, parms);

            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, CommandType.Text);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet(string commandText, params DbParameter[] parms)
        {
            return ExecuteDataSet(commandText, CommandType.Text, parms);
        }

        /// <summary>
        /// 执行SQL语句,返回结果集
        /// </summary>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="commandType">命令类型(存储过程,命令文本, 其它.)</param>
        /// <param name="parms">查询参数</param>
        /// <returns>返回结果集</returns>
        public DataSet ExecuteDataSet(string commandText, CommandType commandType, params DbParameter[] parms)
        {
            DbCommand command = factory.CreateCommand();
            try
            {
                PrepareCommand(command, commandText, commandType, parms);
                DbDataAdapter adapter = factory.CreateDataAdapter();
                adapter.SelectCommand = command;
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
            finally
            {
                FreeConnection(command.Connection);
            }
        }

        /// <summary>
        /// 准备命令（组装DbCommand对象，准备执行）
        /// </summary>
        /// <param name="command">
        /// 表示要对数据源执行的 SQL 语句或存储过程。为表示命令的、数据库特有的类提供一个基类。
        /// </param>
        /// <param name="transaction">事务的基类。</param>
        /// <param name="commandType">指定如何解释命令字符串。</param>
        /// <param name="commandText">命令字符串</param>
        /// <param name="parms">
        /// 表示 System.Data.Common.DbCommand 的参数，还可表示该参数到一个 System.Data.DataSet 列的映射。有关参数的更多信息，请参见
        /// 配置参数和参数数据类型。</param>
        protected void PrepareCommand(DbCommand command, string commandText, CommandType commandType, DbParameter[] parms)
        {
            command.Connection = GetConnection();
            if (command.Connection.State != ConnectionState.Open)
            {
                command.Connection.Open();
            }
            command.CommandTimeout = CommandTimeOut;
            // 设置命令文本(存储过程名或SQL语句)
            command.CommandText = commandText;
            // 设置命令类型.
            command.CommandType = commandType;
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value;
                foreach (var parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput
                        || parameter.Direction == ParameterDirection.Input)
                        && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
                command.Parameters.AddRange(parms);
            }
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entity">实体</param>
        public bool Add<T>(T entity)
        {
            var type = typeof(T);

            var tableName = string.Empty;

            TableAttribute tableAttr = type.GetCustomAttribute<TableAttribute>();

            if (tableAttr != null)
            {
                tableName = tableAttr.Name;
            }
            else
            {
                tableName = type.Name;
            }
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance ^ BindingFlags.DeclaredOnly);

            StringBuilder insertBuilder = new StringBuilder();

            StringBuilder valueBuilder = new StringBuilder();

            insertBuilder.AppendFormat("INSERT INTO {0} (", tableName);
            valueBuilder.Append("VALUES(");
            List<DbParameter> paramters = new List<DbParameter>();
            foreach (var property in properties)
            {
                var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                var propertyValue = property.GetValue(entity);

                if (columnAttr != null && propertyValue != null)
                {
                    insertBuilder.AppendFormat("{0},", columnAttr.Name);
                    valueBuilder.AppendFormat("@{0},", columnAttr.Name);
                    var param = factory.CreateParameter();
                    param.ParameterName = columnAttr.Name;
                    param.Value = propertyValue;
                    paramters.Add(param);
                }
            }
            insertBuilder.Remove(insertBuilder.Length - 1, 1);
            valueBuilder.Remove(valueBuilder.Length - 1, 1);
            insertBuilder.Append(") ");
            valueBuilder.Append(");");
            var SQL = string.Concat(insertBuilder.ToString(), valueBuilder.ToString());
            int flag = ExecuteNonQuery(SQL, paramters.ToArray());
            if (flag > 0) return true;
            return false;
        }

        /// <summary>
        /// 导入实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="entities">实体列表</param>
        /// <returns>返回受影响的行数</returns>
        public int Load<T>(List<T> entities)
        {
            int result = 0;
            var type = typeof(T);

            var fileName = string.Format(@"{0}/{1}_{2}.txt", AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"), type.Name, DateTime.Now.Ticks);
            try
            {
                #region MyRegion

                var tableName = string.Empty;

                TableAttribute tableAttr = type.GetCustomAttribute<TableAttribute>();

                if (tableAttr != null)
                {
                    tableName = tableAttr.Name;
                }
                else
                {
                    tableName = type.Name;
                }
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance ^ BindingFlags.DeclaredOnly);

                StringBuilder insertBuilder = new StringBuilder();

                StringBuilder valueBuilder = new StringBuilder();

                insertBuilder.Append("(");

                foreach (var property in properties)
                {
                    var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

                    if (columnAttr != null)
                    {
                        insertBuilder.AppendFormat("{0},", columnAttr.Name);
                    }
                }
                insertBuilder.Remove(insertBuilder.Length - 1, 1);
                insertBuilder.Append(")");
                var insertString = insertBuilder.ToString();
                #endregion

                var command = string.Format("LOAD DATA LOCAL INFILE '{0}' INTO TABLE {1} CHARACTER SET UTF8 FIELDS TERMINATED BY '\t' ENCLOSED BY '' ESCAPED BY '' LINES TERMINATED BY '\n' STARTING BY '' {2};", fileName, tableName, insertString);

                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    using (var sw = new StreamWriter(fs) { AutoFlush = true, NewLine = "\r\n" })
                    {
                        StringBuilder builder = new StringBuilder();

                        int index = 0;

                        foreach (var item in entities)
                        {
                            foreach (var property in properties)
                            {
                                builder.AppendFormat("{0}\t", property.GetValue(item));
                            }
                            builder.AppendLine();

                            index++;

                            if (index == 100)
                            {
                                index = 0;
                                sw.Write(builder.ToString());
                                builder.Clear();
                            }
                        }
                        sw.Write(builder.ToString());
                    }
                }
                result = ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                result = -1;
            }
            finally
            {
                File.Delete(fileName);
            }

            return result;
        }

        /// <summary>
        /// 返回实体
        /// </summary>
        /// <typeparam name="T">指定需要返回的类型</typeparam>
        /// <param name="commandText">SQL语句或存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>返回实体</returns>
        public T Get<T>(string commandText, params DbParameter[] parameters) where T : class, new()
        {
            T item = default(T);

            var dt = ExecuteDataTable(commandText, parameters);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                if (row == null)
                {
                    return item;
                }
                var key = typeof(T).FullName;

                PropertyInfo[] properties = null;

                if (!Properties.ContainsKey(key))
                {
                    properties = typeof(T).GetProperties();

                    Properties.TryAdd(key, properties);
                }
                else
                {
                    Properties.TryGetValue(key, out properties);
                }
                if (properties == null || properties.Length == 0)
                {
                    return item;
                }
                foreach (var property in properties)
                {
                    var obj = row[property.Name];

                    if (obj.GetType() != typeof(DBNull))
                    {
                        property.SetValue(item, Convert.ChangeType(obj, property.PropertyType));
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// 返回实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string commandText, params DbParameter[] parameters) where T : class, new()
        {
            List<T> list = new List<T>();

            var dt = ExecuteDataTable(commandText, parameters);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                DataTable table = dt;

                var key = typeof(T).FullName;

                PropertyInfo[] properties = null;

                if (!Properties.ContainsKey(key))
                {
                    properties = typeof(T).GetProperties();

                    Properties.TryAdd(key, properties);
                }
                else
                {
                    Properties.TryGetValue(key, out properties);
                }
                if (properties == null || properties.Length == 0)
                {
                    return list;
                }
                foreach (DataRow row in table.Rows)
                {
                    var item = new T();

                    foreach (var property in properties)
                    {
                        if (table.Columns.Contains(property.Name))
                        {
                            var obj = row[property.Name];

                            if (obj.GetType() != typeof(DBNull))
                            {
                                property.SetValue(item, Convert.ChangeType(obj, property.PropertyType));
                            }
                        }
                    }
                    list.Add(item);
                }
            }
            return list;
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;

                pool.Dispose();
            }
        }
        #endregion
    }

    /// <summary>
    /// 连接包装器
    /// </summary>
    internal sealed class DbConnectionWrapper : IPoolObjectWrapper<DbConnection>
    {
        private DbConnection _innerObject;
        private int _timeOut;

        /// <summary>
        /// 对象创建器
        /// </summary>
        /// <param name="constructorArgs">构造内部对象需要的参数，至少三个参数 第一个连接字符串，第二个数据库连接程序名，第三个超时值</param>
        /// <returns></returns>
        public void ObjectCreator(object[] args)
        {
            if (args == null || args.Length < 3)
            {
                throw new ArgumentException("参数不能为空", "args");
            }
            string dbConnectionString = args[0].ToString();
            DbProviderFactory provider = DbProviderFactories.GetFactory(args[1].ToString());
            _timeOut = Convert.ToInt32(args[2]);

            _innerObject = provider.CreateConnection();
            _innerObject.ConnectionString = dbConnectionString;
            _innerObject.Open();
            _isUsing = false;
            _isValid = true;
            _lastActiveTime = DateTime.Now;
        }

        /// <summary>
        /// 获取内部真实对象
        /// </summary>
        /// <returns></returns>
        public DbConnection GetInnerObject()
        {
            ThrowIfNotInitialized();

            _lastActiveTime = DateTime.Now;

            _isUsing = true;

            return _innerObject;
        }

        /// <summary>
        /// 重置对象状态
        /// </summary>
        public void Reset()
        {
            _lastActiveTime = DateTime.Now;

            _isUsing = false;

            _isValid = true;
        }

        /// <summary>
        /// 获取内部真实对象hashcode
        /// </summary>
        public int InnerObjectHashCode
        {
            get
            {
                ThrowIfNotInitialized();

                return _innerObject.GetHashCode();
            }
        }

        /// <summary>
        /// 对象存活检测
        /// </summary>
        /// <returns></returns>
        public bool HeartbeatTest()
        {
            if (!_isUsing && _lastActiveTime.AddSeconds(_timeOut) < DateTime.Now) // 标识不再使用与使用超时了 则该对象不再有效
            {
                _isValid = false;

                return false;
            }
            return true;
        }

        private bool _isUsing = false;
        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool IsUsing
        {
            get
            {
                return _isUsing;
            }
        }

        bool _isValid = true;
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (_isValid && _innerObject != null && _innerObject.State == ConnectionState.Open);
            }
            private set
            {
                _isValid = value;
            }
        }

        /// <summary>
        /// 最后一次活动时间
        /// </summary>
        private DateTime _lastActiveTime;

        private void ThrowIfNotInitialized()
        {
            if (_innerObject == null)
            {
                throw new InvalidOperationException("对象未初始化前无法使用.");
            }
        }

        bool _disposed = false;
        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _isValid = false;

                _disposed = true;

                if (_innerObject == null) return;

                if (_innerObject.State == ConnectionState.Open)
                {
                    _innerObject.Close();
                }
                _innerObject.Dispose();

                _innerObject = null;
            }
        }
    }

    /// <summary>
    /// ConnectionPool 本类纯属多余，直接放到 FollowMysqlHelper里面即可
    /// </summary>
    //internal class ConnectionPool : ObjectPool<DbConnectionWrapper,DbConnection>
    //{
    //    DbProviderFactory _factory = null;

    //    string _connectionString = string.Empty; //连接串
    //    string _providerInvariantName = string.Empty; //提供数据库连接程序的固定名称

    //    DbConnectionStringBuilder _dcsb;

    //    /// <summary>
    //    /// 数据库连接池
    //    /// </summary>
    //    /// <param name="dbConnectionString">数据库连接字符串</param>
    //    /// <param name="providerInvariantName">提供程序的固定名称</param>
    //    public ConnectionPool(string dbConnectionString, string providerInvariantName)
    //    {
    //        if (dbConnectionString == null)
    //        {
    //            throw new ArgumentNullException("dbConnectionString");
    //        }
    //        var flag = dbConnectionString.ToLower().StartsWith("name=");
    //        if (flag)
    //        {
    //            //从配置文件读取
    //            var name = dbConnectionString.Substring(5);
    //            _connectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
    //        }
    //        else
    //        {
    //            //直接赋值
    //            _connectionString = dbConnectionString;
    //        }
    //        _providerInvariantName = providerInvariantName;
    //        _factory = DbProviderFactories.GetFactory(providerInvariantName);
    //        _dcsb = _factory.CreateConnectionStringBuilder();
    //        _dcsb.ConnectionString = _connectionString;
    //        int _minPoolSize = 0, _maxPoolSize = 0;

    //        //最大池对象
    //        if (_dcsb.ContainsKey("maxpoolsize"))
    //        {
    //            if (!int.TryParse(_dcsb["maxpoolsize"].ToString(), out _maxPoolSize))
    //            {
    //                _maxPoolSize = 10;
    //            }
    //        }
    //        //最小池对象
    //        if (_dcsb.ContainsKey("minpoolsize"))
    //        {
    //            if (!int.TryParse(_dcsb["minpoolsize"].ToString(), out _minPoolSize))
    //            {
    //                _minPoolSize = 1;
    //            }
    //        }
    //        if (_minPoolSize > _maxPoolSize)
    //        {
    //            _maxPoolSize = _minPoolSize;
    //            _minPoolSize = 0;
    //        }
    //        _dcsb["maxpoolsize"] = _maxPoolSize;
    //        _dcsb["minpoolsize"] = _minPoolSize;
    //        _connectionString = _dcsb.ConnectionString;

    //        base.Init(_minPoolSize, _maxPoolSize, _connectionString, _providerInvariantName, 600);
    //    }

    //    /// <summary>
    //    /// 获取连接工厂
    //    /// </summary>
    //    /// <returns></returns>
    //    public DbProviderFactory GetDbProviderFactory()
    //    {
    //        return _factory;
    //    }

    //    /// <summary>
    //    /// 从数据库连接池中获取数据库连接对象
    //    /// </summary>
    //    /// <returns></returns>
    //    public DbConnection GetConnection()
    //    {
    //        return GetObjectFromPool();
    //    }

    //    /// <summary>
    //    /// 释放数据库连接对象到数据库连接池中
    //    /// </summary>
    //    public void FreeConnection(DbConnection connection)
    //    {
    //        FreeObjectToPool(connection);
    //    }
    //}
}
