using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services
{
    public class Result
    {
        /// <summary>
        /// 代码
        /// </summary>
        public Int32 Code { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }
    }
}
