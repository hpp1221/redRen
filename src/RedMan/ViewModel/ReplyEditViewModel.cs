using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace RedMan.ViewModel
{
    public class ReplyEditViewModel
    {
        public Int64 ReplyId { get; set; }

        [Required(ErrorMessage ="请输入内容")]
        [MinLength(5,ErrorMessage ="请至少输入5个字")]
        [MaxLength(2048,ErrorMessage ="不得超过2048个字")]
        public string Content { get; set; }
    }
}
