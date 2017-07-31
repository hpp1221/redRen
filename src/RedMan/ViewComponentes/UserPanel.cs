using Microsoft.AspNetCore.Mvc;
using RedMan.DataAccess.IRepository;
using RedMan.DataAccess.Repository;
using System;
using System.Threading.Tasks;
using Web.DataAccess.Repository;
using Web.Model.Context;
using Web.Model.Entities;
using Web.Services.EntitiesServices;
using Web.Services.IEntitiesServices;

namespace RedMan.ViewComponentes
{
    /// <summary>
    /// 首页-右侧-用户面板
    /// </summary>
    public class UserPanel :ViewComponent
    {
        private readonly MyContext _context;
        private readonly IIdentityService _identitySer;
        private readonly IRepository<User> _userRepo;
        public UserPanel(MyContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._userRepo = new Repository<User>(context);
            this._identitySer = new IdentityService(new IdentityRepository<User>(context));
        }

        public async Task<IViewComponentResult> InvokeAsync(String username)
        {
            //当前登录用户名
            if(username == "loginUserName")
            {
                var loginUserName = User.Identity.Name;
                //已登录
                if(loginUserName != null)
                {
                    var loginUser = await _userRepo.FindAsync(p => p.Name == loginUserName);
                    if(loginUser != null)
                        return View(nameof(UserPanel),loginUser);
                    else
                        return View(nameof(UserPanel),new User());
                }
                //未登录
                else
                {
                    return View(nameof(UserPanel),new User());
                }
            }
            //话题作者，或其他
            else
            {
                var showUser = await _userRepo.FindAsync(p => p.Name == username);
                if(showUser != null)
                {
                    return View(nameof(UserPanel),showUser);
                }
                else
                {
                    return View(nameof(UserPanel),new User());
                }
            }
        }
    }
}
