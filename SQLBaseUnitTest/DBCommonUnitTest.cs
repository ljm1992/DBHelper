using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLServer;

namespace SQLBaseUnitTest
{
    [TestClass]
    public class DBCommonUnitTest
    {
        #region ConvertToString_Temp
        /// <summary>
        /// 对ConvertToString_Temp函数的一些测试，完整的有
        /// public static string ConvertToString_Temp(Array arr, string splitToken,
        ///    string prefix, string suffix,
        ///    Func<string, string> replaceArr)
        /// <param name="arr">需加工的数组</param>
        /// <param name="splitToken">数组中对象之间的分隔符</param>
        /// <param name="prefix">前缀</param>
        /// <param name="suffix">后缀</param>
        /// <param name="replaceArr">数组中元素的变换规则</param>
        /// arr和splitToken不能缺少,前缀和后缀必须同时存在或同时不存在
        /// 创建人：卢君默  创建时间：2015-6-10 13:57:32
        /// </summary>
        [TestMethod]
        public void ConvertToString_Temp_WithAll()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            string splitToken = @", ";
            string prefix = @"[";
            string suffix = @"]";
            string result = DBCommon.ConvertToString(ints, splitToken, prefix, suffix, replaceArr);
            Assert.AreEqual(@"[1.1, 2.2, 3.3, 4.4, 5.5]", result);
        }

        [TestMethod]
        public void ConvertToString_Temp_WithFix()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            string splitToken = @", ";
            string prefix = @"[";
            string suffix = @"]";
            string result = DBCommon.ConvertToString(ints, splitToken, prefix, suffix);
            Assert.AreEqual(@"[1, 2, 3, 4, 5]", result);
        }

        [TestMethod]
        public void ConvertToString_Temp_WithFunc()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            string splitToken = @", ";
            string result = DBCommon.ConvertToString(ints, splitToken, replaceArr);
            Assert.AreEqual(@"1.1, 2.2, 3.3, 4.4, 5.5", result);
        }

        [TestMethod]
        public void ConvertToString_Temp_WithNothing()
        {
            int[] ints = { 1, 2, 3, 4, 5 };
            string splitToken = @", ";
            string result = DBCommon.ConvertToString(ints, splitToken);
            Assert.AreEqual(@"1, 2, 3, 4, 5", result);
        }

        private string replaceArr(object oldStr)
        {
            return oldStr.ToString() + "." + oldStr.ToString();
        }
        #endregion
    }
}
