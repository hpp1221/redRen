using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedMan.ViewModel
{
    public class IndexTopicsViewModel {
        public TopicTapViewModel Tab { get; set; }
        public TopicTypeViewModel Type { get; set; }
        public string UserAvatarUrl { get; set; }
        public Int64 UserId { get; set; }
        public string UserName { get; set; }
        public int RepliesCount { get; set; }
        public int VisitsCount { get; set; }
        public string LastReplyUrl { get; set; }
        public string LastReplyUserAvatarUrl { get; set; }
        public string LastReplyDateTime { get; set; }
        public Int64 TopicId { get; set; }
        public string Title { get; set; }
        public bool Top { get; set; }
        public bool Good { get; set; }
        public string CreateDateTime { get; set; }
    }

    /// <summary>
    /// 所有话题类别
    /// </summary>
    public enum TopicTapViewModel {
        all,
        good,
        share,
        ask,
        job
    }

    /// <summary>
    /// 话题分类
    /// </summary>
    public enum TopicTypeViewModel {
        置顶,
        精华,
        分享,
        问答,
        二手
    }
}
