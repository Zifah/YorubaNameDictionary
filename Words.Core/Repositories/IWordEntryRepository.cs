using Words.Core.Entities;
using Words.Core.Dto.Response;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Repositories;

namespace Words.Core.Repositories
{
    public interface IWordEntryRepository : IDictionaryEntryRepository<WordEntry>
    {
        Task<HashSet<WordEntry>> FindEntryByDefinitionsContentContainingAndState(string title, State state);
        Task<IDictionary<string, string[]>> GetEnglishDefinitionsOf(IEnumerable<string> words);
        Task<IDictionary<string, string>> AddEnglishDefinitionsAsync(IDictionary<string, string> definitionsByWord);
        Task<List<WordDefinitionNeedsReviewDto>> GetWordsWithDefinitionsNeedingReviewAsync(int page, int count);
        Task<int> CountByStateAsync(State state);
        Task<List<WordEntry>> FindByStateAsync(State state);
        Task<WordEntry?> GetByIdAsync(string id);
        Task<WordEntry?> AcceptSuggestionAsync(string id);
        Task<bool> DeleteAsync(string id);
        Task<string[]> DeleteSuggestedWordsBatchAsync(IEnumerable<string> words);
        Task<int> DeleteByStateAsync(State state);
    }
}
