using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RedMan.ViewModel
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage ="请输入邮箱地址")]
        [DataType(DataType.EmailAddress,ErrorMessage ="请输入正确的邮箱地址")]
        public string Email { get; set; }
    }
}
