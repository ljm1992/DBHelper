using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServer
{
    public static class DBWrongMessage
    {
        public const string ConnstrWrong = "连接字符串错误";
        public const string TableNotExist = "表不存在";
        public const string FieldNotExist = "字段不存在";
        public const string KeyNotExist = "表主键不存在";
        public const string ParametersWrong = "参数不合法";
        public const string ParametersWrongWhenInsert = "执行INSERT操作时参数不合法";
        public const string ParametersWrongWhenUpdate = "执行UPDATE操作时参数不合法";
    }
}
