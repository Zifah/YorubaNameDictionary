﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Website.Resources;

namespace Website.Pages.Shared
{
    public class BasePageModel : PageModel
    {
        protected readonly IStringLocalizer<Messages> _localizer;
        public BasePageModel(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);

            // Some of the strings below should be internationalized.
            ViewData["Description"] = "YorubaNames";
            ViewData["SocialURL"] = "http://www.yorubaname.com";
            ViewData["SocialTitle"] = "YorubaNames";
            ViewData["SocialDescription"] = "Over 10,000 Yoruba names and still growing...";
        }
    }
}
