using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLServer
{
    public class DBHelper
    {
        #region 私有字段
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string connstr;

        /// <summary>
        /// 内部Sql命令
        /// </summary>
        private string _sqlCommand;
        private string sqlCommand_temp;
        #endregion

        #region 属性
        /// <summary>
        /// 要执行的sql语句
        /// </summary>
        public string sqlCommand
        {
            get
            {
                return sqlCommand_temp;
            }
            set
            {
                _sqlCommand = value;
                sqlCommand_temp = value;
            }
            
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connstr">连接字符串</param>
        /// <param name="sqlCommand">要执行的sql语句</param>
        /// 创建人：卢君默
        /// 创建时间：2015-4-8 22:30:11
        /// 修改人：卢君默  修改时间：2015-4-18 01:25:24
        /// 将连接字符串相关的操作与异常放在初始化中
        /// 修改人：卢君默  修改时间：2015-6-10 19:44:55    这里检查之后直接关掉连接
        public DBHelper(string connstr, string sqlCommand = null)
        {
            this.connstr = connstr;
            this._sqlCommand = string.IsNullOrEmpty(sqlCommand) ? string.Empty : sqlCommand;
            DBCommon.CheckConnstr(connstr);
        }
        #endregion

        #region ExecuteNonQuery(+2重载)
        /// <summary>
        /// 执行有多个参数的参数化sql命令，并返回一个int型的值
        /// </summary>
        /// <param name="sqlParameters">参数及其取值</param>
        /// <returns>受影响的行数</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-9 17:14:58
        public int ExecuteNonQuery(SqlParameter[] sqlParameters)
        {
            sqlParameters = GetSqlParameters(sqlParameters);
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlCommand sc = new SqlCommand(_sqlCommand, conn))
                {
                    if (DBCommon.IsPermitted(sqlParameters))
                        sc.Parameters.AddRange(sqlParameters);
                    return sc.ExecuteNonQuery();
                }
            }

        }
        

        /// <summary>
        /// 执行有一个参数的参数化sql命令，并返回一个int型的值
        /// (如果是参数化sql，执行select时返回值为-1)
        /// </summary>
        /// <param name="sqlParameters">（可选参数）参数及其取值</param>
        /// <returns>受影响的行数</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-9 17:31:20
        public int ExecuteNonQuery(SqlParameter sqlParameter = null)
        {
            return ExecuteNonQuery(DBCommon.GetSqlParameter(sqlParameter));
        }
        #endregion

        #region ExecuteScalar(+2重载)
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行
        /// </summary>
        /// <param name="sqlParameters">参数及其取值</param>
        /// <returns>结果集中第一行的第一列</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:01:20
        public Object ExecuteScalar(SqlParameter[] sqlParameters)
        {
            sqlParameters = GetSqlParameters(sqlParameters);
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlCommand sc = new SqlCommand(_sqlCommand, conn))
                {
                    if (DBCommon.IsPermitted(sqlParameters))
                        sc.Parameters.AddRange(sqlParameters);
                    return sc.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行
        /// </summary>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>结果集中第一行的第一列</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:01:44
        public Object ExecuteScalar(SqlParameter sqlParameter = null)
        {
            return ExecuteScalar(DBCommon.GetSqlParameter(sqlParameter));
        }
        #endregion

        #region ExeuteToDataTable（+2重载）
        /// <summary>
        /// 执行查询，并将查询结果放入DataTable中
        /// </summary>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>记录了查询结果的DataTable</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:05:19
        public DataTable ExeuteToDataTable(SqlParameter sqlParameter = null)
        {
            return ExeuteToDataTable(DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 执行查询，并将查询结果放入DataTable中
        /// </summary>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>记录了查询结果的DataTable</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:02:40
        public DataTable ExeuteToDataTable(SqlParameter[] sqlParameters)
        {
            sqlParameters = GetSqlParameters(sqlParameters);
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlCommand sc = new SqlCommand(_sqlCommand, conn))
                {
                    if (DBCommon.IsPermitted(sqlParameters))
                        sc.Parameters.AddRange(sqlParameters);
                    return ExeuteToDataTable(sc);
                }
            }
        }

        /// <summary>
        /// 执行语句，并将结果存储在DataTable中
        /// </summary>
        /// <param name="sc">要对SQL Server数据库执行的T-SQL语句或存储过程</param>
        /// <returns>存储了结果集的DataTable</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-9 18:46:50
        private DataTable ExeuteToDataTable(SqlCommand sc)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter(sc))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        #endregion

        #region ExeuteToList<T>(+2重载)
        /// <summary>
        /// 执行查询语句，并将结果放入List中
        /// </summary>
        /// <typeparam name="T">List中元素的类型</typeparam>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>记录了查询结果的List</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:05:53
        public List<T> ExeuteToList<T>(SqlParameter sqlParameter = null)
        {
            return ExeuteToList<T>(DBCommon.GetSqlParameter(sqlParameter));
        }

        /// <summary>
        /// 执行查询语句，并将结果放入List中
        /// </summary>
        /// <typeparam name="T">List中元素的类型</typeparam>
        /// <param name="sqlParameter">参数及其取值</param>
        /// <returns>记录了查询结果的List</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 19:04:17 
        public List<T> ExeuteToList<T>(SqlParameter[] sqlParameters)
        {
            sqlParameters = GetSqlParameters(sqlParameters);
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlCommand sc = new SqlCommand(_sqlCommand, conn))
                {
                    if (DBCommon.IsPermitted(sqlParameters))
                        sc.Parameters.AddRange(sqlParameters);
                    return ExeuteToList<T>(sc);
                }
            }
        }

        /// <summary>
        /// 将查询结果的第一列放入List中并返回结果列表
        /// </summary>
        /// <typeparam name="T">List中元素的类型</typeparam>
        /// <param name="sc">要对SQL Server数据库执行的T-SQL语句或存储过程</param>
        /// <returns>存储了结果集的List</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-12 18:53:24
        private List<T> ExeuteToList<T>(SqlCommand sc)
        {
            DataTable dt = new DataTable();
            dt = ExeuteToDataTable(sc);
            List<T> resultList = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                resultList.Add((T)row[0]);
            }
            return resultList;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 获得可用的sqlParameter数组
        /// </summary>
        /// <param name="sqlParameters">传入的sqlParameters</param>
        /// <returns>去掉IN条件查询后的参数数组</returns>
        /// 2015-5-1 15:56:53 创建人：卢君默
        private SqlParameter[] GetSqlParameters(SqlParameter[] sqlParameters)
        {
            List<SqlParameter> parameterList = new List<SqlParameter>();
            foreach (SqlParameter sqlParameter in sqlParameters)
            {
                //当某参数的值为数组时认为这个是IN条件查询
                //如果数组是空的则IN后的条件为（NULL）
                if (sqlParameter != null && DBCommon.IsArray(sqlParameter.Value))
                    _sqlCommand = _sqlCommand.Replace(sqlParameter.ParameterName, DBCommon.GetSqlAfterIn((Array)sqlParameter.Value));
                else
                    parameterList.Add(sqlParameter);
            }
            if (parameterList.Count == 0)
                return null;
            return parameterList.ToArray();
        }
        #endregion
    }
}

//+-----------------------------------------------------------
//| 不足： 2015-2-13 23:08:32
//|     sql语句基本靠拼
//|     异常处理完全不会
//|     部分代码要复用
//|     sdk要开始了
//| 
//| 2015-4-9 23:22:59：
//|     添加单元测试，目前计划单元测试一个项目测试一个类，函数中可以各种耦合（但是不推荐），写完单元测试再写代码
//|     sdk什么的暂定，完全都不会呀
//| 
//| 2015-4-10 14:24:15：
//|     异常处理感觉有些问题，目前能想到的就是执行失败或者连接字符串错误这两种
//|     其中执行失败指的是sql语句问题，这个要是能读取数据库中的异常提示就好了，连接字符串报错不知道是什么类
//|     异常打算参考微软原生的异常，看人家是怎么写的
//|     有一个很严重的问题就是引用的时候为什么还要各种引用这个类里面引用的东西，这是个大问题
//| 
//| 2015-4-18 03:08:01：
//|     写完了DBHelper类的单元测试；
//|     将DBHelper的连接字符串异常写在了初始化里面（可能会导致数据库效率有些问题，TODO）
//|     看了小平台的代码，发现异常可以直接抛，不需要各种捕获异常
//|     感觉操作数据库的时候可以考虑加锁，防止多线程出问题（虽然完全不会多线程的问题）
//|     将连接字符串变成了私有变量（一是嫌麻烦，否则改变连接字符串需要校验是否正确等，二是强制性的不允许更换连接字符串）
//|     将异常信息封装成了一个静态类
//|     看了下DBTable类的代码，真是稀烂的说
//| 
//| 2015-4-21 23:04:21：
//|     拆分成三个文件了；
//|     打算重写DBTable的代码，各种方法都要重新设计
//|     命名的大小写好像还有点问题，这个要再看下
//|     不知道干什么的时候就多读书吧，有输入才能有输出，现在还太菜
//| 
//| 2015-4-28 13:41:28：
//|     可能要重构了
//| 
//| 2015-5-1 15:56:53：
//|     将构造函数的SQLCommand设置为可以为空
//|     添加了支持IN条件查询的方法，SQLParameter的值直接传入一个数组
//|     去掉了部分重复代码
//|     现在还不支持like关键字查询
//|     理想的状态是可以支持（ = ） AND （ IN () ）的查询，现在这个也不支持
//|     准备去掉DBQuery类了，感觉比较鸡肋，如果能直接支持上面那一条就去掉DBQuery
//| 
//| 2015-5-3 17:45:21：
//|     搞定了（ = ） AND （ IN () ）的情况，添加了一个测试
//|     like查询的问题好像不用解决，直接把%写到参数里面就行了
//|     目前还有几个问题，但是感觉问题都不太大
//| 
//| 2015-5-4 12:57:59：
//|     发布1.0.0.0版支持函数：
//|     ExecuteNonQuery(); ExecuteNonQuery(SqlParameter); ExecuteNonQuery(SqlParameters)
//|     ExecuteScalar();
//|     ExecuteToDataTable();
//|     ExecuteToList();
//|     支持多条件查询，支持IN和LIKE查询
//| 
//|     下一版本更新：
//|     将DBHelper写成静态类，不需要new对象出来
//|     使用IN查询的时候不要改变用户输入的SqlCommand
//|     未来要支持多线程环境，显然现在这个还不会，所有的sql都没有加锁
//+-----------------------------------------------------------