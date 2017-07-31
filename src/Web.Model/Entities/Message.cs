using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Enums;

namespace Web.Model.Entities
{
    public class Message
    {
        public Int64 MessageId { get; set; }
        public Int64 ToUserId { get; set; }
        public Int64 FromUserId { get; set; }
        public string FromUserName { get; set; }
        public Int64? Topic_Id { get; set; }
        public Int64? Reply_Id { get; set; }
        public string Tilte { get; set; }
        public Int64? FromReplyId { get; set; }
        public string Content { get; set; }
        public bool Has_Read { get; set; } = false;
        public DateTime CreateDateTime { get; set; }
    }
}
