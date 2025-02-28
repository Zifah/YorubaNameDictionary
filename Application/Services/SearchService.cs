﻿using Application.Mappers;
using Core.Dto.Response;
using Core.Entities;
using Core.Repositories;
using YorubaOrganization.Core.Dto.Response;
using YorubaOrganization.Core.Enums;

namespace Application.Services
{
    public class SearchService
    {
        private readonly INameEntryRepository _namesRepository;

        public SearchService(INameEntryRepository namesRepository)
        {
            _namesRepository = namesRepository;
        }

        public async Task<HashSet<string>> AutoComplete(string query)
        {
            var namesResult = new HashSet<NameEntry>();

            if(query.Length > 1)
            {
                namesResult = await _namesRepository.FindByTitleStartingWithAndState(query, State.PUBLISHED);
            }

            var namesContainingQuery = await _namesRepository.FindEntryByTitleContainingAndState(query, State.PUBLISHED);
            namesResult.UnionWith(namesContainingQuery);

            return new HashSet<string>(namesResult.Select(n => n.Title));
        }

        public async Task<NameEntryDto?> GetName(string searchTerm)
        {
            var result = await _namesRepository.FindByTitleAndState(searchTerm, State.PUBLISHED);
            return result?.MapToDto();
        }

        /// <summary>
        /// Return number of published names
        /// </summary>
        /// <returns></returns>
        public async Task<SearchMetadataDto> GetNamesMetadata()
        {
            var totalPublishedNames = await _namesRepository.CountByState(State.PUBLISHED);
            return new SearchMetadataDto { TotalPublishedNames = totalPublishedNames };
        }

        public async Task<IEnumerable<NameEntry>> Search(string searchTerm)
        {
            var exactFound = await _namesRepository.FindByTitleAndState(searchTerm, State.PUBLISHED);
            if (exactFound != null)
            {
                return new NameEntry[] { exactFound };
            }

            var startingWithSearchTerm = await _namesRepository.FindByTitleStartingWithAndState(searchTerm, State.PUBLISHED);
            if (startingWithSearchTerm.Any())
            {
                return startingWithSearchTerm;
            }

            var possibleFound = new HashSet<NameEntry>();
            possibleFound.UnionWith(await _namesRepository.FindEntryByTitleContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindEntryByVariantsContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindEntryByMeaningContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindEntryByExtendedMeaningContainingAndState(searchTerm, State.PUBLISHED));

            return possibleFound;

        }

        public async Task<IEnumerable<NameEntry>> SearchByStartsWith(string searchTerm)
        {
            return await _namesRepository.FindByTitleStartingWithAndState(searchTerm, State.PUBLISHED);
        }
    }
}
