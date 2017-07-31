using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace RedMan.ViewModel
{
    public class TopicViewModel
    {
        [Required(ErrorMessage ="请选择板块")]
        public int Tab { get; set; }
        [Required(ErrorMessage ="请输入标题")]
        [MinLength(2,ErrorMessage ="标题长度应输入 2 字以上")]
        public string Title { get; set; }

        [Required(ErrorMessage = "请输入内容")]
        [MinLength(5,ErrorMessage ="内容字数必须在 5 字以上")]
        [MaxLength(500,ErrorMessage ="内容字数在 500 字以内")]
        public string Content { get; set; }

        /// <summary>
        /// 话题ID
        /// </summary>
        public Int64 TopicId { get; set; }
        /// <summary>
        /// 话题
        /// </summary>
        public Topic Topic { get; set; }
        /// <summary>
        /// 话题作者
        /// </summary>
        public User Author { get; set; }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User LoginUser { get; set; }
        /// <summary>
        /// 是否已被当前登录用户收藏
        /// </summary>
        public bool Collected { get; set; }

        /// <summary>
        /// Markdown转换为HTML
        /// </summary>
        public string Html { get; set; }
    }
}
