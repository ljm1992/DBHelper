using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
    public class DBTable_Temp
    {
        public string connstr { set; get; }
        public string tableName { set; get; }
        public DBTable_Temp(string connstr, string tableName)
        {
            this.connstr = connstr;
            this.tableName = tableName;
        }

        /// <summary>
        /// 在表中删除一条数据
        /// </summary>
        /// <param name="GUID">要删除的数据的GUID</param>
        /// <returns>删除掉的行数</returns>
        public int DeleteDate(Guid GUID)
        {
            string keyName = GetKeyName();
            if (keyName == null) keyName = "GUID";
            string sql = "DELETE FROM " + tableName + " WHERE " + keyName + " = @keyName";
            DBHelper helper = new DBHelper(connstr, sql);
            SqlParameter parameter = new SqlParameter("@keyName", GUID);
            return helper.ExecuteNonQuery(parameter);
        }

        /// <summary>
        /// 在表中删除一条数据
        /// </summary>
        /// <param name="strGUID">要删除的数据的GUID</param>
        /// <returns>删除掉的行数</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 13:11:05
        public int DeleteDate(string strGUID)
        {
            Guid GUID = StrToGuid(strGUID);
            return DeleteDate(GUID);
        }

        /// <summary>
        /// 将字符串转化成Guid类型
        /// </summary>
        /// <param name="strGUID">字符串类型的GUID</param>
        /// <returns>Guid类型的值</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 20:10:19
        private static Guid StrToGuid(string strGUID)
        {
            return new Guid(strGUID);
        }

        /// <summary>
        /// 获取表的主键名
        /// </summary>
        /// <returns>该表的主键名称</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 12:47:03
        private string GetKeyName()
        {
            string sql = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = @TABLE_NAME";
            DBHelper helper = new DBHelper(connstr, sql);
            SqlParameter parameter = new SqlParameter("@TABLE_NAME", tableName);
            return (string)helper.ExecuteScalar(parameter);
        }

        /// <summary>
        /// 在表中删除多条数据
        /// </summary>
        /// <param name="GUIDs">要删除的数据的GUID组成的数组</param>
        /// <returns>删除掉的行数</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 20:43:37
        public int DeleteDate(Guid[] GUIDs)
        {
            string keyName = GetKeyName();
            if (keyName == null) keyName = "GUID";
            string sql = "DELETE FROM " + tableName + " WHERE " + keyName + " IN (" + GetStrGUID(GUIDs) + ")";
            DBHelper helper = new DBHelper(connstr, sql);
            try
            {
                return helper.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 将GUID中的元素取出，按照'***', '***', '***'的格式输出
        /// </summary>
        /// <param name="GUIDs">GUID数组</param>
        /// <returns>输出的字符串</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 20:29:50
        private string GetStrGUID(Guid[] GUIDs)
        {
            StringBuilder sb = new StringBuilder();
            string Delimiter = ", ";
            foreach (Guid GUID in GUIDs)
            {
                sb.Append("'");
                sb.Append(GUID);
                sb.Append("'");
                sb.Append(Delimiter);
            }
            sb.Remove(sb.Length - Delimiter.Length, Delimiter.Length);
            return sb.ToString();
        }

        /// <summary>
        /// 在表中删除多条数据
        /// </summary>
        /// <param name="strGUIDs">要删除的数据的GUID组成的数组</param>
        /// <returns>删除掉的行数</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 20:09:01
        public int DeleteDate(string[] strGUIDs)
        {
            Guid[] GUIDs = Array.ConvertAll<string, Guid>(strGUIDs, new Converter<string, Guid>(StrToGuid));
            return DeleteDate(GUIDs);
        }

        /// <summary>
        /// 查找表中是否已经存在某条数据
        /// </summary>
        /// <param name="fieldName">相关字段的名称</param>
        /// <param name="fieldValue">相关字段的取值</param>
        /// <returns>查找结果</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 22:38:31
        public bool HaveValue(string fieldName, object fieldValue)
        {
            if (!HavaFieldName(fieldName)) return false;
            string sql = "SELECT COUNT(" + fieldName + ") FROM " + tableName + " WHERE " + fieldName + " = '" + fieldValue + "'";
            DBHelper helper = new DBHelper(connstr, sql);
            return (int)helper.ExecuteScalar() > 0;
        }

        /// <summary>
        /// 检查表中是否含有某字段
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns>检查结果</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 21:28:47
        public bool HavaFieldName(string name) 
        {
            string sql = "SELECT COUNT(name) FROM syscolumns WHERE id = object_id(@tableName) AND name = @name";
            DBHelper helper = new DBHelper(connstr, sql);
            SqlParameter[] parameters = {new SqlParameter("@name", name),
                                           new SqlParameter("@tableName", tableName)};
            return (int)helper.ExecuteScalar(parameters) > 0;
        } 

        /// <summary>
        /// 查找表中是否已经存在除去当前数据外的某条数据
        /// </summary>
        /// <param name="fieldName">相关字段的名称</param>
        /// <param name="fieldValue">相关字段的取值</param>
        /// <param name="GUID">相关字段的GUID</param>
        /// <returns>查找结果</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 22:48:45
        public bool HaveSameValue(string fieldName, string fieldValue, Guid GUID)
        {
            if (!HavaFieldName(fieldName)) return false;
            string keyName = GetKeyName();
            if (keyName == null) keyName = "GUID";
            string sql = @"SELECT COUNT(" + fieldName + @") 
                            FROM " + tableName + @" 
                            WHERE " + fieldName + @" = @fieldValue 
                            AND " + keyName + " <> @GUID";
            DBHelper helper = new DBHelper(connstr, sql);
            SqlParameter[] parameters = {new SqlParameter("@GUID", GUID),
                                           new SqlParameter("@fieldValue", fieldValue)};
            return (int)helper.ExecuteScalar(parameters) > 0;
        }

        /// <summary>
        /// 根据GUID获取某个字段的值
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="GUID">当前数据的GUID</param>
        /// <returns>当前数据该字段的值</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-13 23:00:07
        public object GetFieldValue(string fieldName, Guid GUID)
        {
            if (!HavaFieldName(fieldName)) return null;
            string keyName = GetKeyName();
            if (keyName == null) keyName = "GUID";
            string sql = @"SELECT " + fieldName + " FROM " + tableName + " WHERE " + keyName + " = @keyName";
            DBHelper helper = new DBHelper(connstr, sql);
            SqlParameter parameter = new SqlParameter("@keyName", GUID);
            return helper.ExecuteScalar(parameter);
        }

        /// <summary>
        /// 查询某个字段的所有值并返回List
        /// </summary>
        /// <typeparam name="T">该字段的类型</typeparam>
        /// <param name="fieldName">字段名</param>
        /// <returns>结果列表</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-2-14 14:31:13
        public List<T> GetFieldValue<T>(string fieldName)
        {
            if (!HavaFieldName(fieldName)) return null;
            string sql = "SELETE " + fieldName +" FROM " + tableName;
            DBHelper helper = new DBHelper(connstr, sql);
            return helper.ExeuteToList<T>();
        }
    }

    
}


