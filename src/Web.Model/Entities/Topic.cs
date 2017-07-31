using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Enums;

namespace Web.Model.Entities
{
    public class Topic
    {
        private MarkdownSharp.Markdown markdown = new MarkdownSharp.Markdown();

        [Key]
        public Int64 TopicId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string Html { get { return markdown.Transform(Content?.Trim()); } }
        public Int64 Author_Id { get; set; }
        public bool Top { get; set; } = false;
        public bool Good { get; set; } = false;
        public bool Lock { get; set; } = false;
        public Int32 Reply_Count { get; set; }
        public Int32 Visit_Count { get; set; }
        public Int32 Collect_Count { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public Int64? Last_Reply_Id { get; set; }
        public Int64? Last_Reply_UserId { get; set; }
        public DateTime? Last_ReplyDateTime { get; set; }
        public bool Content_Is_Html { get; set; } = false;
        public int Type { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
