using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Model.Entities;

namespace RedMan.ViewModel
{
    public class UserViewModel
    {
        public bool LoginUserIsAdmin { get; set; }
        public User User { get; set; }
        public IEnumerable<Topic> Topic_Published { get; set; }
        public IEnumerable<Topic> Topic_Join { get; set; }
    }
}
