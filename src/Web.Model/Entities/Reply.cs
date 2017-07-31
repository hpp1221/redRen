using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Model.Entities
{
    public class Reply
    {
        private MarkdownSharp.Markdown markdown = new MarkdownSharp.Markdown();
        [Key]
        public Int64 ReplyId { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string Html { get { return markdown.Transform(Content?.Trim()); } }
        public Int64 Author_Id { get; set; }
        public string Author_Name { get; set; }
        public Int64 Topic_Id { get; set; }
        public Int64? Reply_Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public bool Content_Is_Html { get; set; }
        public Int64 Ups { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
