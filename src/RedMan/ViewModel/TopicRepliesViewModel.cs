using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace RedMan.ViewModel
{
    public class TopicRepliesViewModel
    {
        public Int64 TopicId { get; set; }
        public Topic Topic { get; set; }
        public User LoginUser { get; set; }
        public IQueryable<Reply> Replies { get; set; }
    }
}
