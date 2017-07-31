using Microsoft.AspNetCore.Mvc;
using RedMan.DataAccess.IRepository;
using RedMan.DataAccess.Repository;
using System;
using System.Threading.Tasks;
using Web.Model.Context;
using Web.Model.Entities;

namespace RedMan.ViewComponentes {
    /// <summary>
    /// 首页-右侧-无人回复的话题
    /// </summary>
    public class ZeroReplySubject:ViewComponent
    {
        private readonly MyContext _context;
        private readonly IRepository<Topic> _topicRepo;

        public ZeroReplySubject(MyContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._topicRepo = new Repository<Topic>(context);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var zeroReplySubject = await _topicRepo.FindTopDelayAsync(5,p => p.Reply_Count == 0 && !p.Deleted,p => p.CreateDateTime);
            return View(nameof(ZeroReplySubject),zeroReplySubject);
        }
    }
}
