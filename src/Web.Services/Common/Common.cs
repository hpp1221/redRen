using System;
using System.Security.Cryptography;
using System.Text;

namespace Web.Services.Common
{
    public static class Common
    {
        public static string EncryptMD5(string encryptString)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(encryptString));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        #region 辅助
        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static Result Success(string msg)
        {
            return new Result() { Code = 200, Success = true, Message = msg };
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public static Result Fail(string msg)
        {
            return new Result() { Code = -200, Success = false, Message = msg };
        }
        #endregion
    }
}
