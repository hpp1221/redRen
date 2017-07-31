using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RedMan.ViewModel
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "邮箱不能为空!")]
        //[DataType(DataType.EmailAddress,ErrorMessage ="请输入正确的邮箱地址")]
        [RegularExpression(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*\w\w)",ErrorMessage = "输入的邮箱地址不合法")]
        [Remote("CheckEmailIsExist","User")]
        public string Email { get; set; }

        [Required(ErrorMessage ="用户名不能为空")]
        [MaxLength(10,ErrorMessage ="用户名不得超过10个字符")]
        [Remote("CheckUserNameIsExist","User")]
        public string Name { get; set; }

        [Required(ErrorMessage = "密码不能为空!")]
        [MinLength(6,ErrorMessage = "密码长度过短")]
        [MaxLength(16,ErrorMessage = "密码长度过长")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="请输入确认密码")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "两次输入密码不一致!")]
        public string ConfirmPassword { get; set; }
    }
}
