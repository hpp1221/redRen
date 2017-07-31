using Microsoft.AspNetCore.Mvc;
using RedMan.DataAccess.IRepository;
using RedMan.DataAccess.Repository;
using System.Threading.Tasks;
using Web.Model.Context;
using Web.Model.Entities;

namespace RedMan.ViewComponentes {
    /// <summary>
    /// 首页-头部-右侧-导航菜单
    /// </summary>
    public class TopNav:ViewComponent
    {
        private readonly MyContext _context;
        private readonly IRepository<User> _userRepo;

        public TopNav(MyContext context)
        {
            this._context = context;
            this._userRepo = new Repository<User>(context);
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loginUserName = User.Identity.Name;
            if(loginUserName == null)
            {
                return View(nameof(TopNav),new User());
            }
            else
            {
                var loginUser = await _userRepo.FindAsync(p => p.Name == loginUserName);
                if(loginUser != null)
                    return View(nameof(TopNav),loginUser);
                else
                    return View(nameof(TopNav),new User());
            }
        }
    }
}
