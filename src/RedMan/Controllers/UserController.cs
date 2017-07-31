using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RedMan.DataAccess.IRepository;
using RedMan.DataAccess.Repository;
using RedMan.Extensions;
using RedMan.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.DataAccess.Repository;
using Web.Model.Context;
using Web.Model.Entities;
using Web.Model.Paging;
using Web.Services.EntitiesServices;

namespace RedMan.Controllers
{
    [Authorize]
    public class UserController :Controller
    {
        private readonly IHostingEnvironment env;
        private readonly MyContext _context;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Topic> _topicRepo;
        private readonly IRepository<Reply> _replyRepo;
        private readonly IdentityService _identityService;
        private readonly IRepository<TopicCollect> _topicCollectRepo;

        public UserController(IHostingEnvironment env,MyContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            this.env = env;
            this._context = context;
            this._userRepo = new Repository<User>(context);
            this._topicRepo = new Repository<Topic>(context);
            this._replyRepo = new Repository<Reply>(context);
            this._topicCollectRepo = new Repository<TopicCollect>(context);
            this._identityService = new IdentityService(new IdentityRepository<User>(context));
        }

        /// <summary>
        /// 用户主页
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int id)
        {
            var loginUser = await _userRepo.FindAsync(p => p.Name == User.Identity.Name);

            var user = await _userRepo.FindAsync(p => p.UserId == id);
            if(user == null)
                throw new Exception("用户找不到，或者已被删除");
            //获取用户发布过的主题
            var topic_Pub = await _topicRepo.FindTopDelayAsync(10,p => p.Author_Id == user.UserId,p => p.CreateDateTime);
            //获取用户发布过的回复
            var reply_Pub = await _replyRepo.FindTopDelayAsync(10,p => p.Author_Id == user.UserId,p => p.CreateDateTime);
            var topic_Reply = await GetTopicByReply(reply_Pub);
            UserViewModel userViewModel;
            if(topic_Pub != null && topic_Reply != null)
            {
                userViewModel = new UserViewModel()
                {
                    LoginUserIsAdmin = loginUser.IsAdmin,
                    User = user,
                    Topic_Published = topic_Pub,
                    Topic_Join = topic_Reply.Distinct()
                };
            }
            else
            {
                userViewModel = new UserViewModel()
                {
                    LoginUserIsAdmin = loginUser.IsAdmin,
                    User = user
                };
            }

            return View(userViewModel);
        }

        /// <summary>
        /// 用户所有相关话题
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="type">相关话题类型：发布/参与</param>
        /// <param name="pageIndex">页码</param>
        /// <returns></returns>
        public async Task<IActionResult> AllTopic(int id,string type,int pageIndex = 1)
        {
            var user = await _userRepo.FindAsync(p => p.UserId == id);
            if(user == null)
                throw new Exception("用户找不到，或者已被删除");
            var pageSize =(await GetPageSize("User/AllTopic")) ?? 40;
            //数据源
            var pagingModel = new PagingModel<Topic>()
            {
                ModelList = new List<Topic>(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageIndex,
                    ItemsPerPage = pageSize
                }
            };
            //话题相关用户
            var topicUsers = new List<User>();
            if(type == "pub")
            {
                //获取用户发布过的主题
                var topic_Pub = await _topicRepo.FindPagingOrderByDescendingAsync(p => p.Author_Id == user.UserId,p => p.CreateDateTime,pagingModel);
                topicUsers.Add(user);
            }
            else
            {
                #region 回复分页模型
                var replyPagingModel = new PagingModel<Reply>()
                {
                    ModelList = new List<Reply>(),
                    PagingInfo = new PagingInfo()
                    {
                        CurrentPage = pageIndex,
                        ItemsPerPage = pageSize
                    }
                };
                #endregion

                //获取用户发布过的回复
                var reply_Pub = await _replyRepo.FindPagingOrderByDescendingAsync(p => p.Author_Id == user.UserId,p => p.CreateDateTime,replyPagingModel);
                var topic_Reply = await GetTopicByReply(reply_Pub.ModelList);
                pagingModel.ModelList = topic_Reply.Distinct().ToList();
                //查找相关用户
                var topicAuthors = await _userRepo.JoinAsync(pagingModel.ModelList,author => author.UserId,topic => topic.Author_Id,(author,topic) => author);
                var topicLastReplyUsers = await _userRepo.JoinAsync(pagingModel.ModelList,replyUser => replyUser.UserId,topic => topic.Last_Reply_UserId,(replyUser,topic) => replyUser);
                topicUsers = topicAuthors.Concat(topicLastReplyUsers).ToList();
            }

            //分页视图模型
            var pagingViewModel = new PagingModel<IndexTopicsViewModel>();
            pagingViewModel.ModelList = new List<IndexTopicsViewModel>();
            pagingViewModel.PagingInfo = pagingModel.PagingInfo;

            pagingModel.ModelList.ForEach(item =>
            {
                pagingViewModel.ModelList.Add(new IndexTopicsViewModel()
                {
                    Type = (TopicTypeViewModel)item.Type,
                    UserAvatarUrl = topicUsers.Where(p => p.UserId == item.Author_Id).FirstOrDefault().Avatar,
                    UserId = item.Author_Id,
                    UserName = topicUsers.Where(p => p.UserId == item.Author_Id).FirstOrDefault().Name,
                    RepliesCount = item.Reply_Count,
                    VisitsCount = item.Visit_Count,
                    LastReplyUrl = item.Last_Reply_Id == null ? null : Url.Content($"/Topic/Index/{item.TopicId}/#{item.Last_Reply_Id}"),
                    LastReplyUserAvatarUrl = item.Last_Reply_UserId == null ? null : topicUsers.Where(p => p.UserId == item.Last_Reply_UserId).FirstOrDefault().Avatar,
                    LastReplyDateTime = item.Last_ReplyDateTime.ToString(),
                    TopicId = item.TopicId,
                    Title = item.Title
                });
            });

            ViewData["Type"] = type;
            return View(new AllTopicViewModel() { User = user,Topics = pagingViewModel });
        }


        /// <summary>
        /// 话题收藏
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<IActionResult> Collections(int id,int pageIndex = 1)
        {
            var user = await _userRepo.FindAsync(p => p.UserId == id);
            if(user == null)
                throw new Exception("用户找不到，或者已被删除");
            var pageSize = (await GetPageSize("User/AllTopic")) ?? 40;
            //数据源
            var pagingModel = new PagingModel<Topic>()
            {
                ModelList = new List<Topic>(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageIndex,
                    ItemsPerPage = pageSize
                }
            };
            //话题相关用户
            var topicUsers = new List<User>();

            #region 收藏分页模型
            var collectPagingModel = new PagingModel<Reply>()
            {
                ModelList = new List<Reply>(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageIndex,
                    ItemsPerPage = pageSize
                }
            };
            #endregion

            //获取用户收藏的话题
            var collectTopicIdPagingModel = new PagingModel<TopicCollect>()
            {
                ModelList = new List<TopicCollect>(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageIndex,
                    ItemsPerPage = pageSize
                }
            };
            var collectTopicId = await _topicCollectRepo.FindPagingAsync(p => p.UserId == user.UserId,collectTopicIdPagingModel);

            pagingModel.ModelList = (await _topicRepo.JoinAsync(collectTopicId.ModelList,topic => topic.TopicId,topicCollect => topicCollect.TopicId,(topic,topicCollect) => topic)).ToList();

            //查找相关用户
            var topicAuthors = await _userRepo.JoinAsync(pagingModel.ModelList,author => author.UserId,topic => topic.Author_Id,(author,topic) => author);
            var topicLastReplyUsers = await _userRepo.JoinAsync(pagingModel.ModelList,replyUser => replyUser.UserId,topic => topic.Last_Reply_UserId,(replyUser,topic) => replyUser);
            topicUsers = topicAuthors.Concat(topicLastReplyUsers).ToList();

            //分页视图模型
            var pagingViewModel = new PagingModel<IndexTopicsViewModel>();
            pagingViewModel.ModelList = new List<IndexTopicsViewModel>();
            pagingViewModel.PagingInfo = pagingModel.PagingInfo;

            pagingModel.ModelList.ForEach(item =>
            {
                pagingViewModel.ModelList.Add(new IndexTopicsViewModel()
                {
                    Type = (TopicTypeViewModel)item.Type,
                    UserAvatarUrl = topicUsers.Where(p => p.UserId == item.Author_Id).FirstOrDefault().Avatar,
                    UserId = item.Author_Id,
                    UserName = topicUsers.Where(p => p.UserId == item.Author_Id).FirstOrDefault().Name,
                    RepliesCount = item.Reply_Count,
                    VisitsCount = item.Visit_Count,
                    LastReplyUrl = item.Last_Reply_Id == null ? null : Url.Content($"/Topic/Index/{item.TopicId}/#{item.Last_Reply_Id}"),
                    LastReplyUserAvatarUrl = item.Last_Reply_UserId == null ? null : topicUsers.Where(p => p.UserId == item.Last_Reply_UserId).FirstOrDefault().Avatar,
                    LastReplyDateTime = item.Last_ReplyDateTime.ToString(),
                    TopicId = item.TopicId,
                    Title = item.Title
                });
            });
            return View(new AllTopicViewModel() { User = user,Topics = pagingViewModel });
        }

        /// <summary>
        /// 用户设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Settings()
        {
            var user = await _userRepo.FindAsync(p => p.Name == User.Identity.Name);
            if(user == null)
                throw new Exception("用户找不到，或已被删除");
            var userSettingViewModel = new UserSettingsViewModel()
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Avatar = user.Avatar,
                Signature = user.Signature
            };
            ViewData["Error1"] = false;
            return View(userSettingViewModel);
        }

        /// <summary>
        /// 用户设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Settings(UserSettingsViewModel model)
        {
            ViewData["Error1"] = true;
            if(!ModelState.IsValid)
                return View(model);
            var loginUserName = User.Identity.Name;
            if(loginUserName == null)
                return RedirectToAction("Login","Account",new { ReturnUrl = "/User/Setting" });

            var user = await _userRepo.FindAsync(p => p.UserId == model.UserId);
            if(user == null)
                throw new Exception("用户找不到，或已被删除");
            if(user.Name != loginUserName)
                return RedirectToAction("Index","Home");

            var nameIsExist = await _userRepo.IsExistAsync(p => p.Name == model.Name);
            if(nameIsExist && (model.Name != user.Name))
            {
                ModelState.AddModelError("","此名称已存在");
                return View(model);
            }
            var emailIsExist = await _userRepo.IsExistAsync(p => p.Email == model.Email);
            if(emailIsExist && model.Email != user.Email)
            {
                ModelState.AddModelError("","此邮箱地址已存在");
                return View(model);
            }

            if(model.AvatarFile != null)
            {
                string fileExtensions = string.Empty;
                try
                {
                    var fileNameSplit = model.AvatarFile.FileName.Split('.');
                    fileExtensions = fileNameSplit[fileNameSplit.Length - 1];
                }
                catch(IndexOutOfRangeException)
                {
                    ModelState.AddModelError("","未知文件格式");
                    return View(model);
                }
                if(fileExtensions.ToLower() != "jpg" && fileExtensions.ToLower() != "png")
                {
                    ModelState.AddModelError("","请上传 .JPG 或者 .PNG 格式的图片");
                    return View(model);
                }
                var fileName = new StringBuilder().AppendFormat($"{model.UserId}_{model.Name}.{fileExtensions}").ToString();
                var uploadFile = new UploadFile(env);
                model.Avatar = await uploadFile.UploadImage(model.AvatarFile,fileName);
            }

            user.Name = model.Name;
            user.Email = model.Email;
            if(!string.IsNullOrEmpty(model.Avatar))
                user.Avatar = model.Avatar;
            user.Signature = model.Signature;
            var success = await _userRepo.UpdateAsync(user);
            if(success)
                return new RedirectResult(Url.Content("/User/Settings/"));
            else
            {
                ModelState.AddModelError("","保存失败，请稍后重试");
                return View(model);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewData["Error2"] = true;
            var user = await _userRepo.FindAsync(p => p.UserId == model.UserId);
            if(user == null)
            {
                ModelState.AddModelError("","用户不存在，或已被删除");
                return PartialView("_PartialChangePassword",model);
            }
            if(!ModelState.IsValid)
                return PartialView("_PartialChangePassword",model);
            if(user.Password != model.OldPassword)
            {
                ModelState.AddModelError("","原始密码不正确");
                return PartialView("_PartialChangePassword",model);
            }
            user.Password = model.Password;
            var success = await _userRepo.UpdateAsync(user);
            if(success)
            {
                ViewData["Error2"] = false;
                return Content("<script>alert('修改成功!');location.href='/Account/Login'</script>");
            }
            else
            {
                ModelState.AddModelError("","修改失败");
                return PartialView("_PartialChangePassword",new ChangePasswordViewModel());
            }
        }

        /// <summary>
        /// 获取积分排行前100名用户
        /// </summary>
        /// <returns></returns>
        [Route("/user/top100")]
        public IActionResult Top100()
        {
            return View();
        }

        /// <summary>
        /// 获取用户头像链接
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<FileContentResult> GetUserAvatarUrl(int id)
        {
            var user = await _userRepo.FindAsync(p => p.UserId == id);
            if(user == null)
                return null;
            var path = $"{Directory.GetCurrentDirectory()}/wwwroot/{user.Avatar}";
            try
            {
                using(FileStream fs = new FileStream(path,FileMode.Open))
                {
                    var bytes = new byte[fs.Length];
                    fs.Read(bytes,0,bytes.Length);
                    return File(bytes,"application/x-png");
                }
            }
            catch(Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 设置管理员
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SetAdmin(int id)
        {
            var user = await _userRepo.FindAsync(p => p.UserId == id);
            if(user == null)
                return Json(new { status = "error" });
            if(user.IsAdmin)
            {
                user.IsAdmin = false;
                if(await _userRepo.UpdateAsync(user))
                    return Json(new { status = "cancel_admin" });
                else
                    return Json(new { status = "error" });
            }
            else
            {
                user.IsAdmin = true;
                if(await _userRepo.UpdateAsync(user))
                    return Json(new { status = "admin" });
                else
                    return Json(new { status = "error" });
            }
        }


        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            ViewData["Error"] = false;
            return View();
        }

        [AllowAnonymous]
        public async Task<JsonResult> CheckUserNameIsExist(string Name)
        {
            var user = await _userRepo.FindAsync(p => p.Name == Name);
            if(user != null)
                return Json("此用户名已存在");
            else
                return Json(true);
        }

        [AllowAnonymous]
        public async Task<JsonResult> CheckEmailIsExist(string Email)
        {
            var user = await _userRepo.FindAsync(p => p.Email == Email);
            if(user != null)
                return Json("此邮箱已存在");
            else
                return Json(true);
        }

        #region 辅助方法

        /// <summary>
        /// 根据回复列表，获取对应的话题
        /// </summary>
        /// <param name="replies">回复列表</param>
        /// <returns></returns>
        public async Task<IEnumerable<Topic>> GetTopicByReply(IEnumerable<Reply> replies)
        {
            return await _topicRepo.JoinAsync(replies,topic => topic.TopicId,reply => reply.Topic_Id,(topic,reply) => topic);
        }

        /// <summary>
        /// 获取分页大小
        /// </summary>
        /// <param name="wherePageSize">分页位置</param>
        /// <returns></returns>
        private static async Task<int?> GetPageSize(string wherePageSize)
        {
            return await Task.Factory.StartNew(() =>
            {
                var directory = Directory.GetCurrentDirectory();
                IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile($"{directory}/appsettings.json",true,true).Build();
                var pagingConfig = configuration.GetSection("Paging");
                var pageSize = pagingConfig.GetValue(typeof(int),wherePageSize);
                return (int?)pageSize;
            });
        }
        #endregion
    }
}
