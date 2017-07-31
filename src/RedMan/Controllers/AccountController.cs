using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Services.EntitiesServices;
using Microsoft.AspNetCore.Authorization;
using RedMan.ViewModel;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RedMan.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IdentityService _identityService;

        public AccountController(IdentityService identityService)
        {
            this._identityService = identityService;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl=null)
        {
            ViewData["Error"] = false;
            ViewData["ReturnUrl"] = returnUrl;
            if(!string.IsNullOrEmpty(returnUrl)) {
                ViewData["Error"] = true;
                ModelState.AddModelError("","此操作需要登录，请登陆后继续!");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl=null)
        {
            ViewData["Error"] = true;
            ViewData["ReturnUrl"] = returnUrl;
            model.Email = model.Email?.Trim();
            model.Password = model.Password?.Trim();
            if(!ModelState.IsValid) {
                return View(model);
            }
            var user = await _identityService.CheckUserAsync(model.Email, model.Password);
            if(user==null)
            {
                ModelState.AddModelError(string.Empty, "用户名或密码错误");
                model.Password = string.Empty;
                return View(model);
            }

            ViewData["Error"] = false;
            //登录
            await HttpContext.Authentication.SignInAsync(IdentityService.AuthenticationScheme, user);
            //重定向
            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["Error"] = false;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model,string returnUrl=null)
        {
            ViewData["Error"] = true;
            ViewData["ReturnUrl"] = returnUrl;
            model.Name = model.Name?.Trim();
            model.Email = model.Email?.Trim();
            model.Password = model.Password?.Trim();
            model.ConfirmPassword = model.ConfirmPassword?.Trim();
            if(!ModelState.IsValid) {
                return View(model);
            }
            var result = await _identityService.RegisterAsync(model.Email, model.Name, model.Password);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.ErrorString);
                return View(model);
            }
            ViewData["Error"] = false;
            //登录
            await HttpContext.Authentication.SignInAsync(IdentityService.AuthenticationScheme, result.User);
            return RedirectToLocal(returnUrl);
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await HttpContext.Authentication.SignOutAsync(IdentityService.AuthenticationScheme);
            return RedirectToLocal(null);
        }

        
        #region 辅助
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        #endregion
    }
}
