using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SQLServer;
using System.Collections.Generic;

namespace SQLBaseUnitTest
{
    [TestClass]
    public class DBHelperUnitTest
    {
        private string connstr = "data source=LUJUNMO;initial catalog=课程设计专用;user id=sa; password=95938";
        //private string connstr = "data source=WH-PC727;initial catalog=测试着玩的;user id=sa; password=95938";
        
        /// <summary>
        /// 这是一个正常的测试，没有抛出异常
        /// 测试ExecuteNonQuery方法
        /// </summary>
        /// 创建人：卢君默
        /// 创建时间：2015-4-10 13:57:40
        [TestMethod]
        public void DBHelperTest_ExecuteNonQuery()
        {
            //创建数据表
            string sql = CreateTableSql();
            for (int i = 0; i < 10; i++)
            {
                sql += " " + InsertSql(i);
            }
            //新建DBHelper对象
            DBHelper testSql = new DBHelper(connstr, sql);
            //测试ExecuteNonQuery(SqlParameter sqlParameter = null)（省略参数）
            int j = testSql.ExecuteNonQuery();
            Assert.AreEqual(10, j);

            //测试ExecuteNonQuery(SqlParameter sqlParameter = null)
            //参数化sql的select语句返回值为-1
            testSql.sqlCommand = "select * from UnitTest where id = @id";
            SqlParameter sqlParameter = new SqlParameter("@id", 1);
            j = testSql.ExecuteNonQuery(sqlParameter);
            Assert.AreEqual(-1, j);

            //测试ExecuteNonQuery(SqlParameter sqlParameter = null)
            testSql.sqlCommand = "delete from UnitTest where id = @id";
            sqlParameter = new SqlParameter("@id", 1);
            j = testSql.ExecuteNonQuery(sqlParameter);
            Assert.AreEqual(1, j);

            //测试ExecuteNonQuery(SqlParameter[] sqlParameters)
            testSql.sqlCommand = "delete from UnitTest where id = @id and value = @value";
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("@id", 2),
                                               new SqlParameter("@value", 0.2)
                                           };
            j = testSql.ExecuteNonQuery(sqlParameters);
            Assert.AreEqual(1, j);

            DropTable();
        }

        /// <summary>
        /// 这个测试使用错误的连接字符串，抛出异常
        /// </summary>
        /// 创建人：卢君默  创建时间：2015-4-18 01:28:57
        [TestMethod]
        public void DBHelperTest_WrongConnstr()
        {
            string sql = CreateTableSql();
            try
            {
                DBHelper testSql = new DBHelper("一个错误的连接字符串", sql);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.ConnstrWrong);
            }
        }

        /// <summary>
        /// 测试ExecuteScalar()方法
        /// </summary>
        /// 创建人：卢君默  创建时间：2015-4-18 02:00:52
        [TestMethod]
        public void DBHelperTest_ExecuteScalar(){
            //创建数据表
            string sql = CreateTableSql() + ";" + InsertSql(1);
            DBHelper testSql = new DBHelper(connstr, sql);
            testSql.ExecuteNonQuery();

            //测试ExecuteScalar(SqlParameter sqlParameter = null)方法
            testSql.sqlCommand = "select id from UnitTest";
            int i = (int)testSql.ExecuteScalar();
            Assert.AreEqual(1, i);

            //测试ExecuteScalar(SqlParameter sqlParameter = null)方法
            testSql.sqlCommand = "select id from UnitTest where id = @id";
            i = (int)testSql.ExecuteScalar(new SqlParameter("@id", 1));
            Assert.AreEqual(1, i);

            //测试ExecuteScalar(SqlParameter[] sqlParameters)方法
            testSql.sqlCommand = "select id from UnitTest where id = @id and value = @value";
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("@id", 1),
                                               new SqlParameter("@value", 0.1)
                                           };
            i = (int)testSql.ExecuteScalar(sqlParameters);
            Assert.AreEqual(1, i);

            DropTable();
        }

        /// <summary>
        /// 测试ExeuteToDataTable方法
        /// </summary>
        /// 创建人：卢君默  创建时间：2015-4-18 02:47:32
        [TestMethod]
        public void DBHelperTest_ExeuteToDataTable()
        {
            //创建数据表
            string sql = CreateTableSql();
            for (int i = 0; i < 10; i++)
            {
                sql += " " + InsertSql(i);
            }
            //新建DBHelper对象
            DBHelper testSql = new DBHelper(connstr, sql);
            testSql.ExecuteNonQuery();

            //测试ExeuteToDataTable(SqlParameter sqlParameter = null)方法
            testSql.sqlCommand = "select * from UnitTest where id > 3";
            DataTable dt1 = testSql.ExeuteToDataTable();

            testSql.sqlCommand = "select * from UnitTest where id > @id";
            DataTable dt2 = testSql.ExeuteToDataTable(new SqlParameter("@id", 3));

            Assert.AreEqual(true, DataTableTheSame(dt1, dt2));


            testSql.sqlCommand = "select * from UnitTest where id > @id and value > @value";
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("@id", 3),
                                               new SqlParameter("@value", 0.1)
                                           };
            DataTable dt3 = testSql.ExeuteToDataTable(sqlParameters);
            Assert.AreEqual(true, DataTableTheSame(dt1, dt3));

            DropTable();
        }

        [TestMethod]
        public void DBHelperTest_ExeuteToList()
        {
            //创建数据表
            string sql = CreateTableSql();
            for (int i = 0; i < 10; i++)
            {
                sql += " " + InsertSql(i);
            }
            //新建DBHelper对象
            DBHelper testSql = new DBHelper(connstr, sql);
            testSql.ExecuteNonQuery();

            testSql.sqlCommand = "select * from UnitTest where id > 3";
            List<Guid> list1 = testSql.ExeuteToList<Guid>();

            testSql.sqlCommand = "select * from UnitTest where id > @id";
            List<Guid> list2 = testSql.ExeuteToList<Guid>(new SqlParameter("@id", 3));

            Assert.AreEqual(true, ListTheSame<Guid>(list1, list2));


            testSql.sqlCommand = "select * from UnitTest where id > @id and value > @value";
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("@id", 3),
                                               new SqlParameter("@value", 0.1)
                                           };
            List<Guid> list3 = testSql.ExeuteToList<Guid>(sqlParameters);
            Assert.AreEqual(true, ListTheSame<Guid>(list1, list3));

            DropTable();
        }
        
        /// <summary>
        /// 建表语句
        /// </summary>
        /// <returns>返回建表语句</returns>
        /// 创建人：卢君默
        /// 创建时间：2015-4-9 18:12:45
        private string CreateTableSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"if object_id(N'UnitTest',N'U') is not null
	                        drop table UnitTest");
            sb.AppendLine("");
            sb.Append("create table UnitTest(guid uniqueidentifier, id int, value money, text varchar(50))");
            return sb.ToString();
        }

        /// <summary>
        /// 获取插值语句，插入的值为guid, i, 0.i, 'i'
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// 创建人：卢君默
        /// 创建时间：2015-4-9 18:22:25
        private string InsertSql(int i)
        {
            return string.Format("insert into UnitTest values(newid(), {0}, 0.{0}, '{0}')", i.ToString());
        }

        /// <summary>
        /// 删除unitTest表
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-1 20:43:24
        private void DropTable()
        {
            DBHelper helper = new DBHelper(connstr, "Drop table unitTest");
            helper.ExecuteNonQuery();
        }
        /// <summary>
        /// 比较两个非空的DataTable的内容是否相等
        /// </summary>
        /// <param name="Table1"></param>
        /// <param name="Table2"></param>
        /// <returns>比较结果</returns>
        /// 创建人：网上抄的  创建时间：2015-4-18 02:32:04
        private bool DataTableTheSame(DataTable Table1, DataTable Table2)
        {
            if (Table1 == null || Table2 == null 
                || Table1.Rows.Count != Table2.Rows.Count 
                || Table1.Columns.Count != Table2.Columns.Count)
                return false;
            for (int i = 0; i < Table1.Rows.Count; i++)
            {
                for (int j = 0; j < Table1.Columns.Count; j++)
                {
                    if (Table1.Rows[i][j].ToString() != Table2.Rows[i][j].ToString())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 比较list是否相等（顺序相同，值相等）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        /// 创建人：卢君默  创建时间：2015-4-18 03:00:57
        /// 没有想过效率问题。。。
        private bool ListTheSame<T>(List<T> list1, List<T> list2){
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 测试有IN条件查询的情况
        /// </summary>
        [TestMethod]
        public void DBHelperTest_KeyWordIn()
        {
            string sql = CreateTableSql();
            DBHelper helper = new DBHelper(connstr, sql);
            helper.ExecuteNonQuery();

            for (int i = 0; i < 10; i++)
            {
                sql = InsertSql(i);
                helper.sqlCommand = sql;
                helper.ExecuteNonQuery();
            }

            //测试有IN条件查询的情况
            sql = @"DELETE FROM dbo.UnitTest
                    WHERE id in @id";
            helper.sqlCommand = sql;
            int[] ids = { 1, 3, 5 };
            SqlParameter sp = new SqlParameter("@id", ids);
            int result = helper.ExecuteNonQuery(sp);
            Assert.AreEqual(3, result);

            //测试同时有参数化和IN查询的情况
            sql = @"DELETE FROM dbo.UnitTest
                    WHERE id in @id OR text = @text";
            helper.sqlCommand = sql;
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("@text", "7"),
                                               sp
                                           };
            result = helper.ExecuteNonQuery(sqlParameters);
            //之前删除了三行，所以这里只删掉了一行
            Assert.AreEqual(1, result);
            
            DropTable();
        }

        /// <summary>
        /// 有LIKE关键字的条件查询
        /// </summary>
        [TestMethod]
        public void DBHelperTest_KeyWordLike()
        {
            string sql = CreateTableSql();
            DBHelper helper = new DBHelper(connstr, sql);
            helper.ExecuteNonQuery();

            for (int i = 0; i < 20; i++)
            {
                sql = InsertSql(i);
                helper.sqlCommand = sql;
                helper.ExecuteNonQuery();
            }

            //测试有LIKE条件查询的情况
            sql = @"DELETE FROM dbo.UnitTest
                    WHERE text like @text";
            helper.sqlCommand = sql;
            SqlParameter sp = new SqlParameter("@text", "%1%");
            int result = helper.ExecuteNonQuery(sp);
            Assert.AreEqual(11, result);

            DropTable();
        }

        /// <summary>
        /// 当ExecuteScalar没有查出结果时应该返回NULL
        /// </summary>
        [TestMethod]
        public void DBHelperTest_ExecuteScalar_NoResultTest()
        {
            string sql = CreateTableSql();
            DBHelper helper = new DBHelper(connstr, sql);
            helper.ExecuteNonQuery();

            sql = @"SELECT * FROM UnitTest";
            helper.sqlCommand = sql;
            object i = helper.ExecuteScalar();
            Assert.AreEqual(null, i);
        }

        /// <summary>
        /// 有IN条件查询的时候我是直接把IN后面的参数化弄成了字符串拼在了查询的后面，
        /// 但是这样改变了输入的Sql语句
        /// 这个问题要解决
        /// </summary>
        [TestMethod]
        public void DBHelperTest_KeyWordInsqlCommand()
        {
            string sql = CreateTableSql();
            DBHelper helper = new DBHelper(connstr, sql);
            helper.ExecuteNonQuery();

            for (int i = 0; i < 20; i++)
            {
                sql = InsertSql(i);
                helper.sqlCommand = sql;
                helper.ExecuteNonQuery();
            }

            //测试有IN条件查询的情况
            sql = @"DELETE FROM dbo.UnitTest
                    WHERE id in @id";
            helper.sqlCommand = sql;
            int[] ids = { 1, 3, 5 };
            SqlParameter sp = new SqlParameter("@id", ids);
            int result = helper.ExecuteNonQuery(sp);
            Assert.AreEqual(helper.sqlCommand, sql);
        }

        ///// <summary>
        ///// 测试ConvertToString函数
        ///// </summary>
        ///// 创建人：卢君默    创建时间：2015-5-1 20:18:26
        //[TestMethod]
        //public void ConvertToStringTest()
        //{
        //    //测试int型
        //    int[] ints = { 1, 2, 3 };
        //    string str = DBHelper.ConvertToString(ints);
        //    string result = "(1, 2, 3)";
        //    Assert.AreEqual(result, str);

        //    //测试string型
        //    string[] strs = { "ad", "a'd", "a''d", "a,d" };
        //    str = DBHelper.ConvertToString(strs);
        //    result = "(N'ad', N'a''d', N'a''''d', N'a,d')";
        //    Assert.AreEqual(result, str);

        //    //测试空string数组
        //    strs = new string[0];
        //    str = DBHelper.ConvertToString(strs);
        //    result = "(NULL)";
        //    Assert.AreEqual(result, str);
        //}
    }
}
