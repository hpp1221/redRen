using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RedMan.DataAccess.IRepository;
using RedMan.DataAccess.Repository;
using RedMan.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Web.Model.Context;
using Web.Model.Entities;
using Web.Model.Paging;
using Web.DataAccess.ExpressionExtend;

namespace RedMan.Controllers
{
    public class HomeController :Controller
    {

        private readonly MyContext _context;
        private readonly IRepository<Topic> _topicRepo;
        private readonly IRepository<User> _userRepo;

        public HomeController(MyContext context)
        {
            if(context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._topicRepo = new Repository<Topic>(context);
            this._userRepo = new Repository<User>(context);
        }

        [Route("")]
        [Route("Home/Index/{tab}")]
        [Route("Home/Index/{tab}/{q}")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration =10,VaryByQueryKeys = new string[] { "pageIndex"})]
        public async Task<IActionResult> Index(int tab,string q = null,int pageIndex = 1)
        {
            var pageSize = (await GetPageSize("Home/Index")) ?? 40;
            var pagingModel = new PagingModel<Topic>()
            {
                ModelList = new List<Topic>(),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageIndex,
                    ItemsPerPage = pageSize
                }
            };

            Expression<Func<Topic,bool>> predicate = p => !p.Deleted;
            switch(tab)
            {
                case 0:
                break;
                case 1:
                predicate = predicate.And(p => p.Good);
                break;
                default:
                predicate = predicate.And(p => p.Type == tab);
                break;
            }
            if(!string.IsNullOrEmpty(q))
            {
                ViewData["q"] = q;
                if(tab == 0)
                    predicate = predicate.And(p => (p.Title.Contains(q) || p.Content.Contains(q)));
            }
            pagingModel = await _topicRepo.FindPagingOrderByDescendingAsync(predicate,p => p.CreateDateTime, pagingModel);
            pagingModel.ModelList = pagingModel.ModelList.OrderByDescending(p => p.Top).ToList();
            var pagingViewModel = await GetViewModel(pagingModel,tab);
            ViewData["tab"] = tab;
            return View(pagingViewModel);
        }

        [Route("/Error")]
        public IActionResult Error()
        {
            return Content("<h1>Sorry,服务器内部出错了!</h1>","text/html");
        }

        public async Task<PagingModel<IndexTopicsViewModel>> GetViewModel(PagingModel<Topic> pagingModel,int tab = 0)
        {
            var pagingViewModel = new PagingModel<IndexTopicsViewModel>();
            pagingViewModel.ModelList = new List<IndexTopicsViewModel>();
            pagingViewModel.PagingInfo = pagingModel.PagingInfo;

            //根据话题,查找相关用户
            var topicAuthors = (await _userRepo.JoinAsync(pagingModel.ModelList,user => user.UserId,topic => topic.Author_Id,(user,topic) => user)).ToList();
            var topicLastReplyUsers = (await _userRepo.JoinAsync(pagingModel.ModelList,user => user.UserId,topic => topic.Last_Reply_UserId,(user,topic) => user)).ToList();
            var topicUsers = topicAuthors.Concat(topicLastReplyUsers);

            pagingModel.ModelList.ForEach(item =>
            {
                pagingViewModel.ModelList.Add(new IndexTopicsViewModel()
                {
                    Tab = (TopicTapViewModel)tab,
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
                    Title = item.Title,
                    Top = item.Top,
                    Good = item.Good,
                    CreateDateTime=item.CreateDateTime.ToString()
                });
            });
            return pagingViewModel;
        }

        #region 附加方法

        /// <summary>
        /// 获取分页大小
        /// </summary>
        /// <param name="wherePageSize">分页位置</param>
        /// <returns></returns>
        private async Task<int?> GetPageSize(string wherePageSize)
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
