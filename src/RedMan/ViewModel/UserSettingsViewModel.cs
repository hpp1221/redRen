using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace RedMan.ViewModel
{
    public class UserSettingsViewModel
    {
        public Int64 UserId { get; set; }

        [Required(ErrorMessage = "请输入姓名")]
        [MinLength(2,ErrorMessage = "请输入至少两个字")]
        [MaxLength(10,ErrorMessage = "请输入少于10个字")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请输入邮箱")]
        [DataType(DataType.EmailAddress,ErrorMessage = "请输入正确的邮箱地址")]
        public string Email { get; set; }



        [MinLength(6,ErrorMessage = "请至少输入6个字")]
        [MaxLength(120,ErrorMessage = "请输入少于120个字")]
        public string Signature { get; set; }
        public string Avatar { get; set; }

        public IFormFile AvatarFile { get; set; }
    }

    public class ChangePasswordViewModel
    {
        public Int64 UserId { get; set; }

        public string OldPassword { get; set; }

        [MinLength(6,ErrorMessage = "密码长度不得少于6个字")]
        [MaxLength(16,ErrorMessage = "密码上都不得超过16个字")]
        public string Password { get; set; }

        [Compare(nameof(Password),ErrorMessage = "两次输入的密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}
