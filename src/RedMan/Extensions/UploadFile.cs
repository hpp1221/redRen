using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RedMan.Extensions
{
    public class UploadFile
    {
        private readonly IHostingEnvironment env;

        public UploadFile() { }

        public UploadFile(IHostingEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> UploadImage(IFormFile image,string fileName)
        {
            var path = GetDefaultFilePath();
            var filePath = path + fileName;
            var fullPath = $"{env.WebRootPath}{filePath.Replace("/",@"\")}";
            using(var fs=new FileStream(fullPath,FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }
            return filePath;
        }

        public async Task<string> UploadImageReturnFullPath(IFormFile image,string fileName)
        {
            var path = GetDefaultFilePath();
            var filePath = path + fileName;
            var fullPath = $"{env.WebRootPath}{filePath.Replace("/",@"\")}";
            using(var fs = new FileStream(fullPath,FileMode.Create))
            {
                await image.CopyToAsync(fs);
            }
            return fullPath;
        }

        /// <summary>
        /// 获取文件默认保存路径
        /// </summary>
        /// <returns></returns>
        private string GetDefaultFilePath()
        {
            var directory = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder().AddJsonFile($"{directory}/appsettings.json",true,true).Build();
            var filePath = configuration.GetSection("DefaultFilePath");
            var avatarPath = filePath.GetValue(typeof(string),"AvatarPath");
            return (string)avatarPath;
        }

        /// <summary>
        /// 日期转换成unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var start = new DateTime(1970,1,1,0,0,0,dateTime.Kind);
            return Convert.ToInt64((dateTime - start).TotalSeconds);
        }
    }
}
