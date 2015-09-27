using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
    public class DBQuery
    {
        public string query; 
        public DBQuery(SqlParameter sqlParameter)
        {

        }

        public DBQuery(SqlParameter[] sqlParameters)
        {

        }

        public string And(DBQuery query)
        {
            return "";
        }
    }
}

/*
 * 2015-4-28 13:47:15：
 *  新增DBQuery类，用来生成where后面的查询条件，可能会重载操作符
 *  可能会把DBHelper类的东西全部用这个类重写一遍，这个还没有考虑清楚。。。
 *  构造函数估计有点多
*/