using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
    public class DBTable
    {
        #region 私有字段
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string connstr;

        private SqlConnection conn;

        private string _tableName;
        #endregion

        #region 属性
        /// <summary>
        /// 数据库中表的名称
        /// </summary>
        public string tableName {
            set
            {
                value = value.Replace("'", "''");
                CheckTable(value);
                _tableName = value;
            }
            get { return _tableName; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数，需正确配置连接字符串和表名
        /// </summary>
        /// <param name="connstr">连接字符串</param>
        /// <param name="tableName">表在数据库中的名称</param>
        /// 创建人：卢君默  创建时间：2015-5-6 22:41:35
        public DBTable(string connstr, string tableName)
        {
            DBCommon.CheckConnstr(connstr);
            this.connstr = connstr;
            this.conn = new SqlConnection(connstr);

            //先检测connstr可用并且赋值后才能检测表是否存在
            CheckTable(tableName);
            this.tableName = tableName;
        }
        #endregion

        #region 增
        /// <summary>
        /// 在数据库中插入字段，parameters分别给相应的字段赋值，其余字段为默认值
        /// </summary>
        /// <param name="sqlParameters">参数名称以及值</param>
        /// <returns>影响行数</returns>
        /// 创建人：卢君默  创建时间：2015-5-11 21:57:25
        public int Insert(SqlParameter[] sqlParameters)
        {
            if (sqlParameters.Length == 0)
                throw new ArgumentException("sqlParameters", DBWrongMessage.ParametersWrong);
                
            string paramName = string.Empty;
            string paramValue = string.Empty;
            SetValueForInsert(sqlParameters, out paramName, out paramValue);

            string sql = string.Format(@"INSERT INTO {0} ({1}) VALUES({2})", _tableName, paramName, paramValue);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExecuteNonQuery();
        }

        /// <summary>
        /// 在数据库中插入字段，parameters分别给相应的字段赋值，其余字段为默认值
        /// </summary>
        /// <param name="sqlParameter">参数名称以及值</param>
        /// <returns>影响行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-12 21:02:02
        public int Insert(SqlParameter sqlParameter)
        {
            if (sqlParameter == null)
                throw new ArgumentException("sqlParameter", DBWrongMessage.ParametersWrong);
            return Insert(DBCommon.GetSqlParameter(sqlParameter));
        }
        #endregion

        #region 删
        /// <summary>
        /// 通过SqlParameter数组删除数据，参数之间使用AND连接
        /// 即需要同时满足，如果需使用OR连接，请进行多次删除或使用DBHelper
        /// </summary>
        /// <param name="sqlParameters">参数数组</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 13:45:43
        public int Delete(SqlParameter[] sqlParameters)
        {
            string parameters = string.Empty;
            SetValueForWhere(sqlParameters, out parameters);
            string sql = string.Format(@"DELETE FROM {0} WHERE {1}", _tableName, parameters);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExecuteNonQuery();
        }

        /// <summary>
        /// 通过SqlParameter删除数据，支持IN条件查询
        /// </summary>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 13:46:25
        public int Delete(SqlParameter sqlParameter = null)
        {
            if (sqlParameter == null)
            {
                string sql = string.Format(@"DELETE FROM {0}", _tableName);
                DBHelper helper = new DBHelper(connstr, sql);
                return helper.ExecuteNonQuery();
            }
            return Delete(DBCommon.GetSqlParameter(sqlParameter));
        }
        #endregion

        #region 查
        #region GetValue获取一个Object对象
        /// <summary>
        /// 根据一个SqlParameter参数查询返回一个object对象
        /// </summary>
        /// <param name="fieldName">需要查询的字段</param>
        /// <param name="sqlParameter">查询条件的参数</param>
        /// <returns>查询结果的第一个值</returns>
        /// 创建人：卢君默    创建时间：2015-5-14 20:24:10
        public object GetValue(string fieldName, SqlParameter sqlParameter = null)
        {
            CheckFieldName(fieldName);
            if (sqlParameter == null)
            {
                string sql = string.Format(@"SELECT [{0}] FROM {1}", fieldName, _tableName);
                DBHelper helper = new DBHelper(connstr, sql);
                return helper.ExecuteScalar();
            }
            return GetValue(fieldName, DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 根据一个SqlParameter数组查询返回一个object对象
        /// </summary>
        /// <param name="fieldName">需要查询的字段</param>
        /// <param name="sqlParameters">查询条件的参数数组</param>
        /// <returns>查询结果的第一个值</returns>
        /// 创建人：卢君默    创建时间：2015-5-14 20:32:15
        public object GetValue(string fieldName, SqlParameter[] sqlParameters)
        {
            CheckFieldName(fieldName);
            string parameters = string.Empty;
            SetValueForWhere(sqlParameters, out parameters);
            string sql = string.Format(@"SELECT [{0}] FROM {1} WHERE {2}", fieldName, _tableName, parameters);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExecuteScalar();
        }
        #endregion

        #region GetList<T>获取一个List列表
        /// <summary>
        /// 获取某字段的值组成的List
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="fieldName">字段名称</param>
        /// <param name="sqlParameter">SqlParameter参数对象</param>
        /// <returns>查询结果组成的List</returns>
        /// 创建人：卢君默    创建时间：2015-5-15 13:46:43
        public List<T> GetList<T>(string fieldName, SqlParameter sqlParameter = null)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException(fieldName, DBWrongMessage.FieldNotExist);
            if (sqlParameter == null)
            {
                string sql = string.Format(@"SELECT [{0}] FROM {1}", fieldName, _tableName);
                DBHelper helper = new DBHelper(connstr, sql);
                return helper.ExeuteToList<T>();
            }
            return GetList<T>(fieldName, DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 获取某字段的值组成的List
        /// </summary>
        /// <typeparam name="T">值的类型</typeparam>
        /// <param name="fieldName">字段名称</param>
        /// <param name="sqlParameters">SqlParameter参数数组</param>
        /// <returns>查询结果组成的List</returns>
        /// 创建人：卢君默    创建时间：2015-5-15 13:47:35
        public List<T> GetList<T>(string fieldName, SqlParameter[] sqlParameters)
        {
            string parameters = string.Empty;
            SetValueForWhere(sqlParameters, out parameters);
            string sql = string.Format(@"SELECT [{0}] FROM {1} WHERE {2}", fieldName, _tableName, parameters);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExeuteToList<T>();
        }
        #endregion

        #region GetDataTable获取一个DataTable对象
        /// <summary>
        /// 通过字段名称和条件查询获取查询结果的DataTable
        /// </summary>
        /// <param name="fieldNames">要查询的字段名组成的数组</param>
        /// <param name="sqlParameter">参数</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 17:23:34
        public DataTable GetDataTable(string[] fieldNames, SqlParameter sqlParameter = null)
        {
            return GetDataTable(fieldNames, DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 通过字段名称和条件查询获取查询结果的DataTable
        /// </summary>
        /// <param name="fieldNames">要查询的字段名组成的数组</param>
        /// <param name="sqlParameters">参数数组</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-22 00:31:20
        public DataTable GetDataTable(string[] fieldNames, SqlParameter[] sqlParameters)
        {
            string fieldName = ConvertToFieldName(fieldNames);
            string parameters = string.Empty;
            SetValueForWhere(sqlParameters, out parameters);
            string sql = string.Format(@"SELECT {0} FROM {1} WHERE {2}",
                                        fieldName, _tableName, parameters);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExeuteToDataTable();
        }

        /// <summary>
        /// 通过SQLParameter对象查询表中fieldName字段返回dataTable
        /// </summary>
        /// <param name="fieldName">要查询的字段名称</param>
        /// <param name="sqlParameter">条件查询的sqlParemeter对象</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 17:11:29
        public DataTable GetDataTable(string fieldName, SqlParameter sqlParameter = null)
        {
            return GetDataTable(fieldName, DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 通过SQLParameter数组查询表中fieldName字段返回dataTable
        /// </summary>
        /// <param name="fieldName">要查询的字段名称</param>
        /// <param name="sqlParameters">条件查询的sqlParemeter数组</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 17:15:45
        public DataTable GetDataTable(string fieldName, SqlParameter[] sqlParameters)
        {
            string[] fieldNames = GetFieldNameArray(fieldName);
            return GetDataTable(fieldNames, sqlParameters);
        }

        /// <summary>
        /// 将要查询的字段转换为数组
        /// </summary>
        /// <param name="fieldName">要查询的字段</param>
        /// <returns>要查询的数组</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 23:38:21
        private string[] GetFieldNameArray(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");
            string[] fieldNames = { fieldName };
            return fieldNames;
        }

        /// <summary>
        /// 通过SQLParameter数组查询表中所有字段返回dataTable
        /// </summary>
        /// <param name="sqlParameters">条件查询的sqlParameter数组</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 17:07:00
        public DataTable GetDataTable(SqlParameter[] sqlParameters)
        {
            return GetDataTable("*", sqlParameters);
        }

        /// <summary>
        /// 通过SQLParameter对象查询表中所有字段返回dataTable
        /// </summary>
        /// <param name="sqlParameter">条件查询的sqlParameter对象</param>
        /// <returns>记录了查询结果的DataTable对象</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 17:06:28
        public DataTable GetDataTable(SqlParameter sqlParameter = null)
        {
            return GetDataTable(DBCommon.GetSqlParameter(sqlParameter));
        }
        #endregion
        #endregion

        #region 改
        /// <summary>
        /// 对表格进行Update操作
        /// </summary>
        /// <param name="valueParams">设置值的SqlParameter数组</param>
        /// <param name="queryParams">设置取值条件的SqlParameter数组</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 20:42:59
        public int Update(SqlParameter[] valueParams, SqlParameter[] queryParams)
        {
            string strValueParams = string.Empty;
            string strQueryParams = string.Empty;
            SetValueForUpdate(valueParams, out strValueParams);
            SetValueForWhere(queryParams, out strQueryParams);
            string sql = string.Format(@"UPDATE {0} SET {1} WHERE {2}", 
                _tableName, strValueParams, strQueryParams);
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExecuteNonQuery();
        }

        /// <summary>
        /// 对表格进行Update操作
        /// </summary>
        /// <param name="valueParams">设置值的SqlParameter数组</param>
        /// <param name="queryParam">设置取值条件的SqlParameter对象</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 20:39:38
        public int Update(SqlParameter[] valueParams, SqlParameter queryParam = null)
        {
            if (queryParam == null)
            {
                string strValueParams = string.Empty;
                SetValueForUpdate(valueParams, out strValueParams);
                string sql = string.Format(@"UPDATE {0} SET {1}", _tableName, strValueParams);
                DBHelper helper = new DBHelper(connstr, sql);
                return helper.ExecuteNonQuery();
            }
            SqlParameter[] queryParams = DBCommon.GetSqlParameter(queryParam);
            return Update(valueParams, queryParams);
        }

        /// <summary>
        /// 对表格进行Update操作
        /// </summary>
        /// <param name="valueParam">设置值的SqlParameter对象</param>
        /// <param name="queryParams">设置取值条件的SqlParameter数组</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 20:33:25
        public int Update(SqlParameter valueParam, SqlParameter[] queryParams)
        {
            SqlParameter[] valueParams = DBCommon.GetSqlParameter(valueParam);
            return Update(valueParams, queryParams);
        }

        /// <summary>
        /// 对表格进行Update操作
        /// </summary>
        /// <param name="valueParam">设置值的SqlParameter对象</param>
        /// <param name="queryParam">设置取值条件的SqlParameter对象</param>
        /// <returns>影响的行数</returns>
        /// 创建人：卢君默    创建时间：2015-5-13 20:31:09
        public int Update(SqlParameter valueParam, SqlParameter queryParam = null)
        {
            SqlParameter[] valueParams = DBCommon.GetSqlParameter(valueParam);
            return Update(valueParams, queryParam);
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 获取符合条件的SELECT后面的字符串
        /// </summary>
        /// <param name="fieldNames">查询字段组成的数组</param>
        /// <returns>符合条件的SELECT后面的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-5-21 23:57:01
        private string ConvertToFieldName(string[] fieldNames)
        {
            if (fieldNames == null)
                throw new ArgumentNullException("fieldNames");
            return DBCommon.ConvertToString(fieldNames, ", ", PackFieldName);
        }

        /// <summary>
        /// 加工要查询的列名，防止SQL注入等问题
        /// </summary>
        /// <param name="objFieldName">列名</param>
        /// <returns>加工后的结果</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 18:16:35
        private string PackFieldName(object objFieldName)
        {
            string fieldName = objFieldName as string;
            if (fieldName == "*")
                return fieldName;
            else
                return string.Format(@"[{0}]", fieldName);
        }

        /// <summary>
        /// 检查数据库中表是否存在
        /// </summary>
        /// <param name="tableName">待检测的表的名称</param>
        /// <returns>检测结果，存在则返回true</returns>
        /// 创建人：卢君默  创建时间：2015-5-6 22:40:10
        private bool IsExist(string tableName)
        {
            DBHelper helper = new DBHelper(connstr);
            helper.sqlCommand = "SELECT COUNT(1) FROM dbo.sysobjects where id = object_id(@tableName)";
            return helper.ExecuteScalar(new SqlParameter("@tableName", tableName)) != null;
        }

        /// <summary>
        /// 检查数据库中表是否存在，不存在则抛出异常
        /// </summary>
        /// <param name="tableName">待检测的表的名称</param>
        /// 创建人：卢君默  创建时间：2015-5-6 22:40:31
        private void CheckTable(string tableName)
        {
            if (!IsExist(tableName))
                throw new ArgumentException("tableName", DBWrongMessage.TableNotExist);
        }

        /// <summary>
        /// 根据参数数组获取满足INSERT后面的字符串
        /// </summary>
        /// <param name="sqlParameters">参数数组</param>
        /// <param name="paramName">参数名组成的字符串</param>
        /// <param name="paramValue">参数值组成的字符串</param>
        /// 创建人：卢君默    创建时间：2015-5-7 19:59:25
        private void SetValueForInsert(SqlParameter[] sqlParameters, out string paramName, out string paramValue)
        {
            string splitToken = ", ";
            paramName = string.Empty;
            paramValue = string.Empty;
            foreach (SqlParameter sqlParameter in sqlParameters)
            {
                paramName += sqlParameter.ParameterName + splitToken;
                object value = sqlParameter.Value;
                string temp;
                if (value is string || value is Guid)
                    temp = "N'" + value.ToString().Replace("'", "''") + "'";
                else if (value is Array)
                    throw new ArgumentException("sqlParameters", DBWrongMessage.ParametersWrongWhenInsert);
                else
                    temp = value.ToString();
                paramValue += temp + splitToken;
            }
            if (string.IsNullOrEmpty(paramName) || string.IsNullOrEmpty(paramValue))
                return;
            paramName = DBCommon.RemoveEnd(paramName, splitToken);
            paramValue = DBCommon.RemoveEnd(paramValue, splitToken);
        }

        /// <summary>
        /// 根据参数数组获取满足UPDATE赋值部分的字符串
        /// </summary>
        /// <param name="sqlParameter">参数数组</param>
        /// <param name="valueParams">满足UPDATE语句SET后面部分的字符串</param>
        /// 创建人：卢君默    创建时间：2015-5-14 12:32:26
        /// 修改人：卢君默  修改时间：2015-6-10 17:28:53    重载调用ConvertToString
        private void SetValueForUpdate(SqlParameter[] sqlParameters, out string valueParams)
        {
            if(sqlParameters.Length == 0)
                throw new ArgumentException("sqlParameters", DBWrongMessage.ParametersWrongWhenUpdate);

            valueParams = DBCommon.ConvertToString(sqlParameters, ", ", DBCommon.GetValueParam);
        }

        /// <summary>
        /// 根据参数数组获取满足WHERE条件查询的字符串
        /// </summary>
        /// <param name="sqlParameters">参数数组</param>
        /// <param name="parameters">满足WHERE条件查询的字符串</param>
        /// 创建人：卢君默    创建时间：2015-5-13 13:43:57
        /// 修改人：卢君默  修改时间：2015-6-10 17:29:13    重载 调用ConvertToString
        private void SetValueForWhere(SqlParameter[] sqlParameters, out string parameters)
        {
            if (!DBCommon.IsPermitted(sqlParameters))
            {
                parameters = "1=1";
                return;
            }
            parameters = DBCommon.ConvertToString(sqlParameters, @" AND ", DBCommon.GetQueryPaeam);
        }

        /// <summary>
        /// 检查参数是否为空，为空则抛出异常
        /// </summary>
        /// <param name="fieldName">要检查的参数</param>
        /// 创建人：卢君默    创建时间：2015-5-14 21:16:07
        private void CheckFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("fieldName", DBWrongMessage.FieldNotExist);
        }
        #endregion        
    }
}

/**
 * 2015-5-6 19:19:40：
 *  新建，搭框架
 *  
 * 2015-5-7 20:49:21
 *  初步写了下Insert
 *  在纠结到底是坚持写我这个还是先仿照小平台写一个小平台出来，书读得太少。。。
 *  
 * 2015-5-12 13:47:42
 *  背景换成了黑色，感觉酷酷的
 *  
 * 2015-5-12 21:05:25：
 *  insert应该是也完了，参数不存在或者参数值为数组都会抛出异常
 *  没有参数值得字段不赋值，执行SQL可能会抛异常，这个异常直接抛出来就好，应该不用管
 *  单元测试还是要写的整齐一点，要不然简直不好找
 *  功在当代，利在千秋
 *  
 * 2015-5-13 12:53:27：
 *  估计要重写一个类来定义参数，SqlParameter几乎不会用
 *  删除部分和单元测试写完了
 *  看了下后面的感觉有点难
 *  
 * 2015-5-14 08:58:58：
 *  才发现没必要把string和Guid单独判断出来加上引号，直接全部加上引号不就行了嘛
 *  sql会自动转换，伤感
 *  不过可能会有一个问题，就是某些版本要是不支持怎么办
 *  所以那些个函数要稍微调整一下，像现在我就找不到哪些地方判断了哪些没有，要集中写在一起
 *  
 * 2015年5月14日12:31:41：
 *  差不多写完了UPDATE，开始各种查询以及取值啦
 *  测试的代码多了都可能出现多线程冲突的问题，不知道加锁能不能解决
 *  
 * 2015-5-14 19:29:02：
 *  想到了一种OR条件查询的好办法，感觉可以重载，传入多个SqlParameter[] 单个数组里面AND连接，数组之间OR连接
 *  写完了GetValue和相关的单元测试，查询字段为空的异常提示说不定可以改一改
 *  查不出结果和查出很多结果都没有抛异常呀
 *  
 * 2015-5-15 12:43:54：
 *  异常类型要改，微软抛出的异常跟我的不一样
 *  查询部分的函数需要对参数进行判断，字段不存在什么的可以考虑先去数据库查一下，不存在直接抛异常
 *  有时候可能会有比如说fieldName = ***， *** 这样估计也会查出结果，想办法把这种情况过滤掉
 *  试了下，列名加上中括号应该就行了
 *  
 * 2015-5-15 18:54:17 
 *  感觉重载任重而道远，GetDataTable那块完全不会重载
 *  现在好多重复的代码了
 *  还有就是这个估计要稍微停一下了，都快不能顺利毕业了，毕业之后这个要作代码优化，至少一个星期
 *  不能赶进度，又没啥钱，做那么快干嘛
 *  
 * 2015-5-18 13:58:04：
 *  GetDataTable想到的异常有字段不存在异常
 *  其他的异常数据库应该都会直接抛出来，就不管了
 *  
 * 2015-5-19 14:00:56：
 *  将GetDataTable又弄了一个函数出来，并没有体会到可空参数的好处
 *  
 * 2015-5-22 00:31:52：
 *  可能写完了吧，明天测试
 *  
 * 2015-5-23 01:26:52：
 *  晚上写代码感觉真棒
*/