using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;
using Web.Model.Paging;

namespace RedMan.ViewModel
{
    public class AllTopicViewModel
    {
        public User User { get; set; }
        public PagingModel<IndexTopicsViewModel> Topics { get; set; }
    }
}
