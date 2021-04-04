using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhonesAndPeople.Models;
using System.Text;

namespace PhonesAndPeople.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
            PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            int i = 1;
            int j = 0;
            if (pageInfo.PageNumber != 1)
            {
                result = DoPoints(pageInfo.PageNumber - 1, pageInfo, pageUrl, result, "<<<");
                result = DoMarkUp(1, pageInfo, pageUrl, result);
            }
            while (j < 1 && i <= pageInfo.TotalPages)
            {
                if (i == pageInfo.PageNumber)
                {
                    result = DoMarkUp(i, pageInfo, pageUrl, result);
                    j++;
                }
                i++;
            }
            if (pageInfo.PageNumber < pageInfo.TotalPages)
            {
                result = DoMarkUp(pageInfo.TotalPages, pageInfo, pageUrl, result);
                result = DoPoints(pageInfo.PageNumber + 1, pageInfo, pageUrl, result, ">>>");
            }
            return MvcHtmlString.Create(result.ToString());
        }
        public static StringBuilder DoMarkUp(int i, PageInfo pageInfo, Func<int, string> pageUrl, StringBuilder result)
        {
            TagBuilder tag = new TagBuilder("a");
            tag.MergeAttribute("href", pageUrl(i));
            tag.InnerHtml = i.ToString();
            if (i == pageInfo.PageNumber)
            {
                tag.AddCssClass("selected");
                tag.AddCssClass("btn-primary");
            }
            tag.AddCssClass("btn btn-default");
            result.Append(tag.ToString());

            return result;
        }
        public static StringBuilder DoPoints(int i, PageInfo pageInfo, Func<int, string> pageUrl, StringBuilder result, string direction)
        {
            TagBuilder tag = new TagBuilder("a");
            tag.MergeAttribute("href", pageUrl(i));
            tag.InnerHtml = direction;
            if (i == pageInfo.PageNumber)
            {
                tag.AddCssClass("selected");
                tag.AddCssClass("btn-primary");
            }
            tag.AddCssClass("btn btn-default");
            result.Append(tag.ToString());

            return result;
        }
    }
}