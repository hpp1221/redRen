using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Web.Model.Context;
using RedMan.DataAccess.IRepository;
using Web.Model.Entities;
using RedMan.DataAccess.Repository;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860
namespace RedMan.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly MyContext _context;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Message> _msgRepo;
        public MessageController(MyContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._userRepo = new Repository<User>(context);
            this._msgRepo = new Repository<Message>(context);
        }
        public async Task<IActionResult> Index()
        {
            var loginUser = await _userRepo.FindAsync(p => p.Name == User.Identity.Name);
            if(loginUser == null)
                throw new Exception("找不到用户，或已被删除");
            var unreadMsg = await _msgRepo.FindAllDelayAsync(p => p.ToUserId == loginUser.UserId && !p.Has_Read);
            return View(unreadMsg);
        }

        [HttpPost]
        public async Task<IActionResult> ReadMessage()
        {
            var loginUser = await _userRepo.FindAsync(p => p.Name == User.Identity.Name);
            if(loginUser == null)
                throw new Exception("找不到用户，或已被删除");
            var unreadMsg = await _msgRepo.FindAllDelayAsync(p => p.ToUserId == loginUser.UserId && p.Has_Read);
            return PartialView("_PartialReadMessage",unreadMsg);
        }

        /// <summary>
        /// 将消息标记为已读
        /// </summary>
        /// <param name="id">消息ID</param>
        /// <returns></returns>
        public async Task<IActionResult> MarkRead(int id)
        {
            var msg = await _msgRepo.FindAsync(p=>p.MessageId==id);
            if(msg!=null)
            {
                msg.Has_Read = true;
                await _msgRepo.UpdateAsync(msg,false);
                var user = await _userRepo.FindAsync(p => p.UserId == msg.ToUserId);
                if(user!=null)
                {
                    user.UnreadMsg_Count -= 1;
                    await _userRepo.UpdateAsync(user);
                }
                return new RedirectResult($"/Topic/Index/{msg.Topic_Id}#{msg.FromReplyId}");
            }
            throw new Exception("消息找不到，或已被删除");
        }
    }
}
