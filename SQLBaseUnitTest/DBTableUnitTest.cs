using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLServer;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace SQLBaseUnitTest
{
    [TestClass]
    public class DBTableUnitTest
    {
        private string connstr = "data source=LUJUNMO;initial catalog=课程设计专用;user id=sa; password=95938";
        //private string connstr = "data source=WH-PC727;initial catalog=测试着玩的;user id=sa; password=95938";

        #region 构造函数测试
        /// <summary>
        /// 表不存在时引发异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 19:55:50
        [TestMethod]
        public void DBTableTest_TableNotExist()
        {
            try
            {
                DBTable UnitTest = new DBTable(connstr, "一个不存在的表的名称");
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.TableNotExist);
            }
        }

        /// <summary>
        /// 更换表的名称，当换到不存在的名称时抛出异常
        /// 为了验证每次更换表的名称都会检查表是否存在
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 20:17:06
        [TestMethod]
        public void DBTableTest_ChangeTableName()
        {
            CreateTableUnitTest();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.tableName = "一个不存在的表的名称";
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.TableNotExist);
            }
        }
        #endregion

        #region 增
        /// <summary>
        /// 常规增加一行数据，没有异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 19:55:55
        [TestMethod]
        public void DBTableTest_Insert()
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

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlparameters = {
                                           new SqlParameter("id", 1),
                                           new SqlParameter("text", '2')
                                           };
            Assert.AreEqual(1, UnitTest.Insert(sqlparameters));

            DropTable();
        }

        /// <summary>
        /// 不给参数传值,会抛出参数不合法异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 20:32:57
        [TestMethod]
        public void DBTableTest_InsertWithoutValue()
        {
            CreateTableUnitTest();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = { };
            try
            {
                UnitTest.Insert(sqlParameters);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.ParametersWrong);
            }
            finally
            {
                DropTable();
            }
        }

        /// <summary>
        /// 执行INSERT时参数的值为数组则抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 20:54:04
        [TestMethod]
        public void DBTableTest_InsertWithWrongParams()
        {
            CreateTableUnitTest();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            int[] ids = { 1, 3, 5 };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", ids)
                                           };
            try
            {
                UnitTest.Insert(sqlParameters);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.ParametersWrongWhenInsert);
            }
            finally
            {
                DropTable();
            }
        }

        /// <summary>
        /// 执行值为string或者Guid的Insert语句
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 21:03:46
        [TestMethod]
        public void DBTableTest_InsertWhenValueWithString()
        {
            CreateTableUnitTest();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter sqlParameter = new SqlParameter("text", "asd");
            Assert.AreEqual(1, UnitTest.Insert(sqlParameter));

            DropTable();
        }
        #endregion

        #region 删
        /// <summary>
        /// 删除一行数据
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:04:27
        [TestMethod]
        public void DBTableTest_Delete()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("text", "1"),
                                               new SqlParameter("id", 1)
                                           };
            Assert.AreEqual(1, UnitTest.Delete(sqlParameters));
            
            DropTable();
        }

        /// <summary>
        /// 通过SqlParameter删除数据
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:09:03
        [TestMethod]
        public void DBTableTest_DeleteWithParameter()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter sqlParameter = new SqlParameter("text", "1");
            Assert.AreEqual(1, UnitTest.Delete(sqlParameter));

            DropTable();
        }

        /// <summary>
        /// 通过SqlParameter删除多条数据
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:32:25
        [TestMethod]
        public void DBTableTest_DeleteDatasWithParameter()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string[] texts = { "1", "2", "2" };
            SqlParameter sqlParameter = new SqlParameter("text", texts);
            Assert.AreEqual(2, UnitTest.Delete(sqlParameter));

            DropTable();
        }

        /// <summary>
        /// 尝试删除一条数据库中不存在的数据，影响行数为0
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:11:59
        [TestMethod]
        public void DBTableTest_DeleteDataFail()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("text", "2"),
                                               new SqlParameter("id", 1)
                                           };
            Assert.AreEqual(0, UnitTest.Delete(sqlParameters));

            DropTable();
        }

        /// <summary>
        /// 有多个IN关键字存在的删除操作
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:18:53
        [TestMethod]
        public void DBTableTest_DeleteWithKeyIns()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string[] texts = { "1", "3", "5" };
            int[] ids = { 1, 2, 3 };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("text", texts),
                                               new SqlParameter("id", ids)
                                           };
            Assert.AreEqual(2, UnitTest.Delete(sqlParameters));

            DropTable();
        }

        /// <summary>
        /// 只有一个IN关键字存在时的删除操作
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:21:01
        [TestMethod]
        public void DBTableTest_DeleteWithKeyIn()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            int[] ids = { 1, 2, 3 };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", ids)
                                           };
            Assert.AreEqual(3, UnitTest.Delete(sqlParameters));

            DropTable();
        }

        /// <summary>
        /// 同时有IN和等于存在时的删除
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 13:22:36
        [TestMethod]
        public void DBTableTest_DeleteWithInAndEqual()
        {
            CreateTableWithValue();

            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            int[] ids = { 1, 2, 3 };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", ids),
                                               new SqlParameter("text", "2")
                                           };
            Assert.AreEqual(1, UnitTest.Delete(sqlParameters));

            DropTable();
        }

        /// <summary>
        /// delete中不传参数是删除所有数据
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 19:20:52
        [TestMethod]
        public void DBTableTest_DeleteAll()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            Assert.AreEqual(10, UnitTest.Delete());
            DropTable();
        }
        #endregion

        #region 改
        /// <summary>
        /// Update操作,以下几个测试都是不抛异常，看Update的各种参数
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 19:42:25
        [TestMethod]
        public void DBTableTest_Update()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100)
                                         };
            SqlParameter[] queryParams = {
                                             new SqlParameter("id", 2)
                                         };
            Assert.AreEqual(1, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update2()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100),
                                             new SqlParameter("text", "yibai")
                                         };
            SqlParameter queryParam = new SqlParameter("id", 2);
            Assert.AreEqual(1, UnitTest.Update(valueParams, queryParam));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update3()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter valueParams = new SqlParameter("id", 100);
            string[] texts = { "2" };
            SqlParameter[] queryParams = {
                                             new SqlParameter("id", 2),
                                             new SqlParameter("text", texts)
                                         };
            Assert.AreEqual(1, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update4()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100)
                                         };
            int[] ids = { 1, 2, 3, 4 };
            string[] texts = { "1", "3", "5" };
            SqlParameter[] queryParams = { 
                                             new SqlParameter("id", ids),
                                             new SqlParameter("text", texts)
                                         };
            Assert.AreEqual(2, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update5()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100)
                                         };
            SqlParameter[] queryParams = {
                                            new SqlParameter("id", 2),
                                            new SqlParameter("text", "2")
                                        };
            Assert.AreEqual(1, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update6()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100)
                                         };
            int[] ids = { 1, 2, 3 };
            SqlParameter queryParam = new SqlParameter("id", ids);
            Assert.AreEqual(3, UnitTest.Update(valueParams, queryParam));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update7()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100),
                                             new SqlParameter("text", "yibai")
                                         };
            string[] texts = { };
            SqlParameter[] queryParams = { 
                                             new SqlParameter("id", 2),
                                             new SqlParameter("text", texts)
                                         };
            Assert.AreEqual(0, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update8()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100),
                                             new SqlParameter("text", "yibai")
                                         };
            int[] ids = { 1, 2, 3, 4 };
            string[] texts = { "yi", "3", "5" };
            SqlParameter[] queryParams = { 
                                             new SqlParameter("id", ids),
                                             new SqlParameter("text", texts)
                                         };
            Assert.AreEqual(1, UnitTest.Update(valueParams, queryParams));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update9()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100),
                                             new SqlParameter("text", "yibai")
                                         };
            SqlParameter[] queryParam = {
                                            new SqlParameter("id", 2),
                                            new SqlParameter("text", "3")
                                        };
            Assert.AreEqual(0, UnitTest.Update(valueParams, queryParam));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_Update10()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] valueParams = {
                                             new SqlParameter("id", 100),
                                             new SqlParameter("text", "yibai")
                                         };
            int[] ids = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0 };
            SqlParameter queryParam = new SqlParameter("id", ids);
            Assert.AreEqual(10, UnitTest.Update(valueParams, queryParam));
            DropTable();
        }

        /// <summary>
        /// 测试只有一个参数的情况
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 20:13:29
        [TestMethod]
        public void DBTableTest_UpdateAll()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter valueParam = new SqlParameter("id", 100);
            Assert.AreEqual(10, UnitTest.Update(valueParam));
            DropTable();
        }

        /// <summary>
        /// 给valueParam赋的值为一个数组时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 20:20:06
        public void DBTableTest_UpdateFail()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            int[] ids = { 1 };
            SqlParameter valueParam = new SqlParameter("id", ids);
            try
            {
                UnitTest.Update(valueParam);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.ParametersWrongWhenUpdate);
            }
            DropTable();
        }
        #endregion

        #region 查
        #region 获取单个的值
        /// <summary>
        /// 通过一个Sqlparameter对象获取单个值
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:37:53
        [TestMethod]
        public void DBTableTest_GetValueByParam()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter sqlParameter = new SqlParameter("id", 1);
            Assert.AreEqual("1", (string)UnitTest.GetValue("text", sqlParameter));
            DropTable();
        }

        /// <summary>
        /// 通过SqlParameter数组获取单个值
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:39:11
        [TestMethod]
        public void DBTableTest_GetValueByParams()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 1),
                                               new SqlParameter("text", "1")
                                           };
            Assert.AreEqual<decimal>((decimal)0.1, (decimal)UnitTest.GetValue("value", sqlParameters));
            DropTable();
        }

        /// <summary>
        /// 不传SqlParameter参数时获取单个值
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:42:21
        [TestMethod]
        public void DBTableTest_GetValue()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            Assert.AreEqual(1, (int)UnitTest.GetValue("id"));
            DropTable();
        }

        /// <summary>
        /// 当要查询的字段不存在时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:46:12
        [TestMethod]
        public void DBTableTest_GetValueWrongWhenFieldNotExist()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetValue("asd");
            }
            catch (Exception e)
            {
                StringAssert.StartsWith(e.Message, "列名");
                StringAssert.EndsWith(e.Message, "无效。");                
            }
            DropTable();
        }

        /// <summary>
        /// 当要查询的字段不存在时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:46:12
        [TestMethod]
        public void DBTableTest_GetValueWrongWhenFieldIsEmpty()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetValue("");
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.FieldNotExist);
            }
            DropTable();
        }

        /// <summary>
        /// 没有值时返回null
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:48:27
        [TestMethod]
        public void DBTableTest_GetValueReturnNullWhenNoValueExist()
        {
            CreateTableUnitTest();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            Assert.AreEqual(null, UnitTest.GetValue("id"));
            DropTable();
        }

        /// <summary>
        /// 没有值时返回null
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 20:09:57
        [TestMethod]
        public void DBTableTest_GetValueReturnNullWhenNoValueExist2()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 1),
                                               new SqlParameter("text", "3")
                                           };
            Assert.AreEqual(null, UnitTest.GetValue("id", sqlParameters));
            DropTable();
        }

        /// <summary>
        /// 查询出多个值时返回第一个
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-14 19:50:28
        [TestMethod]
        public void DBTableTest_GetValueReturnFirstWhenGetValues()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            Assert.AreEqual(0, (int)UnitTest.GetValue("id"));
            DropTable();
        }
        #endregion

        #region GetList获取多个值
        /// <summary>
        /// 测试列名是否经过过滤
        /// </summary>
        /// 创建人：卢君默  创建时间：2015-5-23 01:39:12
        [TestMethod]
        public void DBTableTest_GetListCheckFieldName()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetList<int>("id, value");
            }
            catch (Exception e)
            {
                StringAssert.StartsWith(e.Message, "列名");
                StringAssert.EndsWith(e.Message, "无效。");
            }
        }

        /// <summary>
        /// 通过一个Sqlparameter对象获取一个List
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 12:57:32
        [TestMethod]
        public void DBTableTest_GetListByParam()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter sqlParameter = new SqlParameter("id", 1);      
            Assert.AreEqual(true, ListTheSame<int>(new List<int>(){1}, UnitTest.GetList<int>("id", sqlParameter)));
            DropTable();
        }

        /// <summary>
        /// 通过SqlParameter数组获取一个List
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:00:39
        [TestMethod]
        public void DBTableTest_GetListByParams()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 1),
                                               new SqlParameter("text", "1")
                                           };
            bool result = ListTheSame<decimal>(new List<decimal>() { (decimal)0.1 }, UnitTest.GetList<decimal>("value", sqlParameters));
            Assert.AreEqual(true, result);
            DropTable();
        }

        /// <summary>
        /// 不传SqlParameter参数时获取一个List
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:04:12
        [TestMethod]
        public void DBTableTest_GetList()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            List<string> list = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            Assert.AreEqual(true, ListTheSame<string>(list, UnitTest.GetList<string>("text")));
            DropTable();
        }

        /// <summary>
        /// 当要查询的字段不存在时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:05:38
        [TestMethod]
        public void DBTableTest_GetListWrongWhenFieldNotExist()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetList<object>("一个不存在的列名");
            }
            catch (Exception e)
            {
                StringAssert.StartsWith(e.Message, "列名");
                StringAssert.EndsWith(e.Message, "无效。");                
            }
            DropTable();
        }

        /// <summary>
        /// 当要查询的字段不存在时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:09:12
        [TestMethod]
        public void DBTableTest_GetListWrongWhenFieldIsEmpty()
        {
            CreateTableWithOneValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetList<object>("");
            }
            catch (Exception e)
            {
                StringAssert.Contains(e.Message, DBWrongMessage.FieldNotExist);
            }
            DropTable();
        }

        /// <summary>
        /// 没有值时返回空的List
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:12:23
        [TestMethod]
        public void DBTableTest_GetListReturnNullWhenNoValueExist()
        {
            CreateTableUnitTest();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            Assert.ReferenceEquals(new List<int>(), UnitTest.GetList<int>("id"));
            DropTable();
        }

        /// <summary>
        /// 没有值时返回null
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 13:13:11
        [TestMethod]
        public void DBTableTest_GetListReturnNullWhenNoValueExist2()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 1),
                                               new SqlParameter("text", "3")
                                           };
            Assert.ReferenceEquals(new List<int>(), UnitTest.GetList<int>("id", sqlParameters));
            DropTable();
        }

        /// <summary>
        /// 字段类型不同时抛出异常
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-15 19:00:58
        [TestMethod]
        public void DBTableTest_GetListReturnFirstWhenGetValues()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 1),
                                               new SqlParameter("text", "1")
                                           };
            try
            {
                UnitTest.GetList<string>("id", sqlParameters);
            }
            catch (InvalidCastException e)
            {
                StringAssert.Contains(e.Message, "无法将类型");
                StringAssert.Contains(e.Message, "强制转换为");
            }
            DropTable();
        }
        #endregion

        #region GetDataTable获取DataTable对象
        /// <summary>
        /// 当查询的列名是多个列名的组合时抛出异常
        /// </summary>
        /// 创建人：卢君默  创建时间：2015-5-23 01:33:33
        [TestMethod]
        public void DBTableTest_GetDataTableCheckFieldName()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            try
            {
                UnitTest.GetDataTable("id, value");
            }
            catch (Exception e)
            {
                StringAssert.StartsWith(e.Message, "列名");
                StringAssert.EndsWith(e.Message, "无效。");
            }
        }

        /// <summary>
        /// 没有任何参数时将整张表作为DataTable返回
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-18 13:55:53
        [TestMethod]
        public void DBTableTest_GetDataTable_0_0()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text 
                                FROM dbo.UnitTest
                                ";
            DataTable dt = GetDatatable(sqlCommand);
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable()));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_0_1()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text 
                                FROM dbo.UnitTest
                                WHERE id = 3
                                ";
            DataTable dt = GetDatatable(sqlCommand);
            SqlParameter sqlParameter = new SqlParameter("id", 3);
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(sqlParameter)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_0_N()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text 
                                FROM dbo.UnitTest
                                WHERE id = 3 AND text = '3'
                                ";
            DataTable dt = GetDatatable(sqlCommand);
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("id", 3),
                                               new SqlParameter("text", "3")
                                           };
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(sqlParameters)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_1_0()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  id
                                FROM dbo.UnitTest
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string fieldName = "id";
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldName)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_1_1()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  text
                                FROM dbo.UnitTest
                                WHERE id IN (1,2,3,4,5,6)
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string fieldName = "text";
            int[] ids = { 1, 2, 3, 4, 5, 6 };
            SqlParameter sqlParameter = new SqlParameter("id", ids);
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldName, sqlParameter)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_1_N()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  value
                                FROM dbo.UnitTest
                                WHERE text IN ('1', '2', '2') AND id = 3
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string fieldName = "value";
            string[] texts = { "1", "2", "2" };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("text", texts),
                                               new SqlParameter("id", 3)
                                           };
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldName, sqlParameters)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_N_0()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text
                                FROM dbo.UnitTest
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string[] fieldNames = { "guid", "id", "value", "text" };
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldNames)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_N_1()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text
                                FROM dbo.UnitTest
                                WHERE value IN (0.10, 0.20, 0.30)
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string[] fieldNames = { "guid", "id", "value", "text" };
            decimal[] values = { 0.10M, 0.20M, 0.30M };
            SqlParameter sqlParameter = new SqlParameter("value", values);
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldNames, sqlParameter)));
            DropTable();
        }

        [TestMethod]
        public void DBTableTest_GetDataTable_N_N()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text
                                FROM dbo.UnitTest
                                WHERE value IN (0.10, 0.20, 0.30)
                                AND id IN (1, 2, 3, 4, 5, 6, 7, 8, 9)
                                AND text IN ('1', '2', '2')
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string[] fieldNames = { "guid", "id", "value", "text" };
            decimal[] values = { 0.10M, 0.20M, 0.30M };
            int[] ids = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            string[] texts = { "1", "2", "2" };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("value", values),
                                               new SqlParameter("id", ids),
                                               new SqlParameter("text", texts)
                                           };
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldNames, sqlParameters)));
            DropTable();
        }

        /// <summary>
        /// 查不出结果时返回一个含有表结构的DataTable
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-19 13:58:15
        [TestMethod]
        public void DBTableTest_GetDataTableReturnDataTableEmpty()
        {
            CreateTableWithValue();
            DBTable UnitTest = new DBTable(connstr, "UnitTest");
            string sqlCommand = @"
                                SELECT  guid ,
                                        id ,
                                        value ,
                                        text
                                FROM dbo.UnitTest
                                WHERE value = 0.10 AND id = 2
                                ";
            DataTable dt = GetDatatable(sqlCommand);

            string[] fieldNames = { "guid", "id", "value", "text" };
            SqlParameter[] sqlParameters = {
                                               new SqlParameter("value", 0.10),     /*不确定会不会抛异常*/
                                               new SqlParameter("id", 2)
                                           };
            Assert.AreEqual(true, DataTableTheSame(dt, UnitTest.GetDataTable(fieldNames, sqlParameters)));
            //应该是不相等的，查不出结果但是表的结构还在
            Assert.AreNotEqual<DataTable>(new DataTable(), UnitTest.GetDataTable(fieldNames, sqlParameters));
            DropTable();
        }
        #endregion
        #endregion

        #region 私有函数
        /// <summary>
        /// 通过Sql语句获取DataTable
        /// </summary>
        /// <param name="sqlCommand">执行的查询语句</param>
        /// <returns>查询的DataTable</returns>
        /// 创建人：卢君默    创建时间：2015-5-18 13:48:22
        private DataTable GetDatatable(string sqlCommand)
        {
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlCommand sc = new SqlCommand(sqlCommand, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(sc))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
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
        private bool ListTheSame<T>(List<T> list1, List<T> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].Equals(list2[i]))
                    return false;
            }
            return true;
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
        /// 创建表UnitTest，不包含值
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-12 20:07:27
        private void CreateTableUnitTest()
        {
            string sql = CreateTableSql();
            DBHelper helper = new DBHelper(connstr, sql);
            helper.ExecuteNonQuery();
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
        /// 创建一个带有值的UnitTest表
        /// </summary>
        /// 创建人：卢君默    创建时间：2015-5-13 12:57:41
        private void CreateTableWithValue()
        {
            CreateTableUnitTest();
            DBHelper helper = new DBHelper(connstr);
            for (int i = 0; i < 10; i++)
            {
                helper.sqlCommand = InsertSql(i);
                helper.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 创建带有一个值得UnitTest表
        /// </summary>
        /// <param name="i">值的id</param>
        /// 创建人：卢君默    创建时间：2015-5-14 19:34:25
        private void CreateTableWithOneValue(int i = 1)
        {
            CreateTableUnitTest();
            DBHelper helper = new DBHelper(connstr, InsertSql(i));
            helper.ExecuteNonQuery();
        }
        #endregion
    }
}

/**
 * 2015-5-7 20:50:49：
 *  添加了两个单元测试，现在想到的还有更改table的时候有一个可能抛异常的测试
 *  
 * 2015-5-12 21:04:19：
 *  暂时应该也完了Insert的单元测试，把单元测试分类
 *  
 * 2015-5-13 13:36:30：
 *  增加删除相关的单元测试
 *  
 * 2015-5-13 20:27:29：
 *  感觉上像是写完了Update的单元测试，开始码代码
 *  
 * 2015-5-19 13:59:37：
 *  正常的GetDataTable的测试应该是写完了，想到了再补，
 *  改天每个函数应该补一个过滤fieldName的测试
 *  
*/