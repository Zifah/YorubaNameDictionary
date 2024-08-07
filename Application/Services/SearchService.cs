﻿using Application.Mappers;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;

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
                namesResult = await _namesRepository.FindByNameStartingWithAndState(query, State.PUBLISHED);
            }

            var namesContainingQuery = await _namesRepository.FindNameEntryByNameContainingAndState(query, State.PUBLISHED);
            namesResult.UnionWith(namesContainingQuery);

            return new HashSet<string>(namesResult.Select(n => n.Name));
        }

        public async Task<NameEntryDto?> GetName(string searchTerm)
        {
            var result = await _namesRepository.FindByNameAndState(searchTerm, State.PUBLISHED);
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
            var exactFound = await _namesRepository.FindByNameAndState(searchTerm, State.PUBLISHED);
            if (exactFound != null)
            {
                return new NameEntry[] { exactFound };
            }

            var startingWithSearchTerm = await _namesRepository.FindByNameStartingWithAndState(searchTerm, State.PUBLISHED);
            if (startingWithSearchTerm.Any())
            {
                return startingWithSearchTerm;
            }

            var possibleFound = new HashSet<NameEntry>();
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByNameContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByVariantsContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByMeaningContainingAndState(searchTerm, State.PUBLISHED));
            possibleFound.UnionWith(await _namesRepository.FindNameEntryByExtendedMeaningContainingAndState(searchTerm, State.PUBLISHED));

            return possibleFound;

        }

        public async Task<IEnumerable<NameEntry>> SearchByStartsWith(string searchTerm)
        {
            return await _namesRepository.FindByNameStartingWithAndState(searchTerm, State.PUBLISHED);
        }
    }
}
