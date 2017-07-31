using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Model.Paging;

namespace RedMan.TagHelpers
{
    public class PageTagHelper:TagHelper
    {
        public PagingInfo pagingInfo { get; set; }
        public Func<long,string> pageUrl { get; set; }

        public override void Process(TagHelperContext context,TagHelperOutput output)
        {
            base.Process(context,output);

            if(pagingInfo == null)
            {
                return;
            }

            Int64 i = 1;
            Int32 showPage = 7;     //显示的页数
            Int32 midelPage = 4;    //中间页数
            var builder = new StringBuilder();

            #region 上一页
            if(!(pagingInfo.CurrentPage - 1 >= 1))
            {
                builder.Append("<li class=\"disabled\"><a>«</a></li>");
            }
            else
            {
                builder.Append($"<li><a href=\"{pageUrl(pagingInfo.CurrentPage - 1)}\">«</a></li>");
            }
            #endregion

            #region 中间页码

            #region 总页数小于显示的页数
            //总页数小于等于显示页数
            if(pagingInfo.TotalPages <= showPage)
            {
                i = 1;
                var endIndex = pagingInfo.TotalPages;
                var tagBuilder = string.Empty;
                for(; i <= endIndex; i++)
                {
                    if(i != pagingInfo.CurrentPage)
                        tagBuilder = $"<li><a href=\"{pageUrl(i)}\">{i}</a></li>";
                    else
                        tagBuilder = $"<li class=\"disabled active\"><a>{i}</a></li>";
                    builder.Append(tagBuilder);
                }
            }
            #endregion
             
            #region 总页数大于显示页数
            //总页数大于显示页数
            else if(pagingInfo.TotalPages > showPage)
            {
                //当前页小于中间页
                if(pagingInfo.CurrentPage <= midelPage)
                {
                    i = 1;
                    var endIndex = showPage;
                    string tagBuilder = string.Empty;
                    for(; i < endIndex; i++)
                    {
                        if(i != pagingInfo.CurrentPage)
                            tagBuilder = $"<li><a href=\"{pageUrl(i)}\">{i}</a></li>";
                        else
                            tagBuilder = $"<li class=\"disabled active\"><a>{i}</a></li>";
                        builder.Append(tagBuilder);
                    }
                    builder.Append("<li><a>...</a></li>");
                }
                //当前页大于中间页
                else if(pagingInfo.CurrentPage > midelPage && pagingInfo.TotalPages - pagingInfo.CurrentPage >= midelPage-1)
                {
                    i = pagingInfo.CurrentPage - midelPage + 2;
                    var endIndex = pagingInfo.CurrentPage + midelPage - 1;
                    string tagBuilder = string.Empty;
                    builder.Append("<li><a>...</a></li>");
                    for(; i < endIndex; i++)
                    {
                        if(i != pagingInfo.CurrentPage)
                            tagBuilder = $"<li><a href=\"{pageUrl(i)}\">{i}</a></li>";
                        else
                            tagBuilder = $"<li class=\"disabled active\"><a>{i}</a></li>";
                        builder.Append(tagBuilder);
                    }
                    builder.Append("<li><a>...</a></li>");
                }
                else if(pagingInfo.TotalPages - pagingInfo.CurrentPage < midelPage)
                {
                    i = pagingInfo.CurrentPage - midelPage + 2;
                    var endIndex = pagingInfo.TotalPages;
                    string tagBuilder = string.Empty;
                    builder.Append("<li><a>...</a></li>");
                    for(; i <= endIndex; i++)
                    {
                        if(i != pagingInfo.CurrentPage)
                            tagBuilder = $"<li><a href=\"{pageUrl(i)}\">{i}</a></li>";
                        else
                            tagBuilder = $"<li class=\"disabled active\"><a>{i}</a></li>";
                        builder.Append(tagBuilder);
                    }
                }
            }
            #endregion

            #endregion

            #region 下一页
            if(!(pagingInfo.CurrentPage + 1 <= pagingInfo.TotalPages))
            {
                builder.Append("<li class=\"disable\"><a>»</a></li>");
            }
            else
            {
                builder.Append($"<li><a href=\"{pageUrl(pagingInfo.CurrentPage + 1)}\">»</a></li>");
            }
            #endregion

            output.TagName = "ul";
            output.Content.SetHtmlContent(builder.ToString());
        }
    }
}
