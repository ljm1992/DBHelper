using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
    public static class DBCommon
    {
        /// <summary>
        /// 检查连接字符串是否正确，不正确则抛出异常
        /// </summary>
        /// <param name="connstr">待检测的连接字符串</param>
        /// 创建人：卢君默  创建时间：2015-5-6 22:28:49
        public static void CheckConnstr(string connstr)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connstr);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                conn.Dispose();
            }
            catch (Exception)
            {
                throw new ArgumentException("connstr", DBWrongMessage.ConnstrWrong);
            }
        }

        /// <summary>
        /// 在主字符串的结尾去掉字符串
        /// </summary>
        /// <param name="mainString">主字符串</param>
        /// <param name="strToRemove">需要去掉的字符串</param>
        /// <returns>修改后的字符串</returns>
        /// 创建人：卢君默    创建时间：2015-5-7 19:56:05
        public static string RemoveEnd(string mainString, string strToRemove)
        {
            return mainString.Remove(mainString.Length - strToRemove.Length);
        }

        /// <summary>
        /// 在StringBuilder对象的结尾去掉字符串
        /// </summary>
        /// <param name="mainStringBuilder">StringBuilder对象</param>
        /// <param name="strToRemove">需要去掉的字符串</param>
        /// <returns>修改之后的StringBuilder对象</returns>
        /// 创建人：卢君默    创建时间：2015-5-12 19:26:54
        public static StringBuilder RemoveEnd(StringBuilder mainStringBuilder, string strToRemove)
        {
            return mainStringBuilder.Remove(mainStringBuilder.Length - strToRemove.Length, strToRemove.Length);
        }

        /// <summary>
        /// 判断一个对象是否是数组
        /// </summary>
        /// <param name="obj">需要判断的对象</param>
        /// <returns>判断结果</returns>
        /// 创建人：卢君默    创建时间：2015-5-1 17:07:37
        public static bool IsArray(object obj)
        {
            if (obj == null) return false;
            return obj.GetType().BaseType == typeof(Array);
        }

        #region ConvertToString重载 将数组中的值经过转换变化为一个字符串
        #region 重载部分
        /// <summary>
        /// 将数组中的值经过转换变化为字符串
        /// </summary>
        /// <param name="arr">需加工的数组(如果是sqlparameter数组则转化为WHERE后的QueryParam)</param>
        /// <param name="splitToken">数组中对象之间的分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <param name="ChangeObjInArray">数组中元素的变换规则</param>
        /// <returns>转换之后的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 00:57:37
        /// 修改人：卢君默  修改时间：2015-6-10 16:17:03    考虑为SqlParameter的情况
        public static string ConvertToString(Array arr, string splitToken,
            string prefix, string suffix,
            Func<object, string> ChangeObjInArray)
        {
            if (arr == null)
                throw new ArgumentNullException("arr");

            const int defaultCapacity = 256;
            StringBuilder sb = new StringBuilder(defaultCapacity);
            sb.Append(prefix);
            //这里少了一次循环，因为添加分隔符的时候最后一项不需要加上分隔符
            for (int i = 0; i < arr.Length - 1; i++)
            {
                sb.Append(ChangeObjInArray(arr.GetValue(i)));
                sb.Append(splitToken);
            }
            //在最后一个分隔符后面补上最后一项
            sb.Append(ChangeObjInArray(arr.GetValue(arr.Length - 1)));
            sb.Append(suffix);
            return sb.ToString();
        }

        /// <summary>
        /// 将数组中的值经过转换变化为字符串
        /// </summary>
        /// <param name="arr">需加工的数组</param>
        /// <param name="splitToken">数组中对象之间的分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <returns>转换之后的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 01:11:21
        public static string ConvertToString(Array arr, string splitToken,
            string prefix, string suffix)
        {
            return ConvertToString(arr, splitToken, prefix, suffix, ChangeObjInArrayreplaceArr);
        }

        /// <summary>
        /// 将数组中的值经过转换变化为字符串
        /// </summary>
        /// <param name="arr">需加工的数组</param>
        /// <param name="splitToken">数组中对象之间的分隔符</param>
        /// <returns>转换之后的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 01:11:36
        public static string ConvertToString(Array arr, string splitToken)
        {
            return ConvertToString(arr, splitToken, string.Empty, string.Empty, ChangeObjInArrayreplaceArr);
        }

        /// <summary>
        /// 将数组中的值经过转换变化为字符串
        /// </summary>
        /// <param name="arr">需加工的数组</param>
        /// <param name="splitToken">数组中对象之间的分隔符</param>
        /// <param name="ChangeObjInArray">数组中元素的变换规则</param>
        /// <returns>转换之后的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 01:11:47
        public static string ConvertToString(Array arr, string splitToken,
            Func<object, string> ChangeObjInArray)
        {
            return ConvertToString(arr, splitToken, string.Empty, string.Empty, ChangeObjInArray);
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 默认转换规则
        /// </summary>
        /// <param name="oldStr">替换之前的字符串</param>
        /// <returns>替换后的字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 00:58:23
        private static string ChangeObjInArrayreplaceArr(object oldStr)
        {
            return oldStr.ToString();
        }
        #endregion  
        #endregion

        #region FilterKeyWordInSql 过滤sql关键字
        /// <summary>
        /// 过滤SQL中的关键字
        /// </summary>
        /// <param name="sqlWithKeyWord">过滤前的sql字符串</param>
        /// <returns>过滤后的sql字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 14:12:09
        public static string FilterKeyWordInSql(object sqlWithKeyWord)
        {
            string result = sqlWithKeyWord.ToString().Replace(@"'", @"''");
            return string.Format(@"'{0}'", result);
        }
        #endregion

        #region GetSqlAfterIn 将数组中的值转换为符合In条件查询后的SQL字符串
        /// <summary>
        /// 将数组中的值转换为符合In条件查询后的SQL字符串
        /// </summary>
        /// <param name="arr">IN后面的取值集合</param>
        /// <returns>符合In条件查询后的SQL字符串</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 14:32:43
        public static string GetSqlAfterIn(Array arr)
        {
            string sqlAfterIn = @"(NULL)";
            if (arr.Length != 0)
                sqlAfterIn = ConvertToString(arr, @", ", "(", ")", FilterKeyWordInSql);
            return sqlAfterIn;
        }
        #endregion

        #region GetQueryPaeam 将SqlParameter对象转换成满足WHERE后条件的字符串
        /// <summary>
        /// 通过一个SQLParameter对象返回满足Where后面的sql语句
        /// 同时兼容IN和等于的条件
        /// </summary>
        /// <param name="objSqlParameter">条件查询的sqlparameter对象对象</param>
        /// <returns>满足Where后面的sql语句</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 15:02:22
        public static string GetQueryPaeam(object objSqlParameter)
        {
            SqlParameter sqlParameter = objSqlParameter as SqlParameter;
            if (sqlParameter != null && DBCommon.IsArray(sqlParameter.Value))
                return string.Format("{0} IN {1}", sqlParameter.ParameterName, GetSqlAfterIn((Array)sqlParameter.Value));
            else
                return string.Format("{0} = '{1}'", sqlParameter.ParameterName, sqlParameter.Value);
        }
        #endregion

        #region GetValueParam 将SqlParameter对象转换成满足Update操作中SET后面条件的字符串
        /// <summary>
        /// 通过一个SQLParameter对象返回满足Update操作中SET后面的sql语句
        /// </summary>
        /// <param name="objSqlParameter">条件查询的sqlparameter对象对象</param>
        /// <returns>满足Update操作中SET后面的sql语句</returns>
        /// 创建人：卢君默  创建时间：2015-6-10 16:47:58
        public static string GetValueParam(object objSqlParameter)
        {
            SqlParameter sqlParameter = objSqlParameter as SqlParameter;
            if (DBCommon.IsArray(sqlParameter.Value))
                throw new ArgumentException("sqlParameters", DBWrongMessage.ParametersWrongWhenUpdate);

            return string.Format("{0} = '{1}'", sqlParameter.ParameterName, sqlParameter.Value);
        }
        #endregion

        /// <summary>
        /// 将传入参数转换为SqlParameter数组传出
        /// </summary>
        /// <param name="sqlParameter">输入参数</param>
        /// <returns>参数组成的数组</returns>
        /// 创建人：卢君默    创建时间：2015-5-1 20:36:15
        /// 主要是为了重载，将SqlParameter参数变为SqlParameter数组
        /// 其中参数允许为NULL，转换出来的结果是第一个值为null的SqlParameter数组
        public static SqlParameter[] GetSqlParameter(SqlParameter sqlParameter = null)
        {
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = sqlParameter;
            return sqlParameters;
        }

        /// <summary>
        /// 判断参数化sql的参数数组是否合法
        /// </summary>
        /// <param name="sqlParameters">参数及其取值的数组</param>
        /// <returns>判断结果</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 12:00:40
        public static bool IsPermitted(SqlParameter[] sqlParameters)
        {
            return sqlParameters != null && sqlParameters[0] != null;
        }
    }
}

/**
 * 2015-5-6 22:29:08
 *  初始化，以后公共函数先全部写在这里，暂时不细分，要不然可能会跟ERP一样
 *  增加CheckConnstr方法
 *  写完之后一次重构感觉必不可少了
 *  书读得太少，感觉要仿照小平台了
*/