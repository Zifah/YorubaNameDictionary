using Application.Services.MultiLanguage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Words.Core.Dto.Request;
using Words.Core.Dto.Response;
using Words.Website.Pages.Shared;
using Words.Website.Resources;
using Words.Website.Services;
using YorubaOrganization.Core.Dto.Request;
using YorubaOrganization.Core.Dto.Response;

namespace Words.Website.Pages
{
    public class SubmitWordModel(
        IStringLocalizer<Messages> localizer,
        ILanguageService languageService,
        ApiService apiService) : BasePageModel(localizer, languageService)
    {
        private readonly ApiService _apiService = apiService;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "missing")]
        public string MissingWord { get; set; } = string.Empty;

        public GeoLocationDto[] GeoLocations { get; private set; } = [];

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task OnGet()
        {
            GeoLocations = await _apiService.GetGeoLocations() ?? [];
        }

        public async Task<IActionResult> OnPost(
            string word,
            string suggestedMeaning,
            string suggestedEmail,
            string[] suggestedGeoLocation)
        {
            if (!ModelState.IsValid)
            {
                GeoLocations = await _apiService.GetGeoLocations() ?? [];
                return Page();
            }

            try
            {
                var dto = new CreateWordDto
                {
                    Word = word,
                    SubmittedBy = suggestedEmail,
                };

                if (!string.IsNullOrWhiteSpace(suggestedMeaning))
                {
                    dto.Definitions.Add(new DefinitionDto(suggestedMeaning, null, [], DateTime.UtcNow));
                }

                foreach (var geo in suggestedGeoLocation)
                {
                    var parts = geo.Split('.', 2);
                    var place = parts.Length > 1 ? parts[1] : geo;
                    var region = parts.Length > 0 ? parts[0] : geo;
                    dto.GeoLocation.Add(new CreateGeoLocationDto(place, region));
                }

                await _apiService.SuggestWordAsync(dto);
                SuccessMessage = "Thank you! Your word has been submitted for review.";
                return RedirectToPage(new { missing = string.Empty });
            }
            catch
            {
                GeoLocations = await _apiService.GetGeoLocations() ?? [];
                ErrorMessage = "Something went wrong submitting your word. Please try again.";
                return Page();
            }
        }
    }
}
