﻿using Microsoft.Extensions.Localization;
using Website.Resources;
using Website.Services;

namespace Website.Pages.Shared
{
    public abstract class StaticPageModel(IStringLocalizer<Messages> localizer, ApiService apiService) : BasePageModel(localizer)
    {
        protected readonly ApiService _apiService = apiService;

        public int NameCount { get; private set; }

        public virtual async Task OnGet()
        {
            var indexedNameCount = await _apiService.GetIndexedNameCount();
            NameCount = indexedNameCount.TotalPublishedNames;
        }
    }
}
