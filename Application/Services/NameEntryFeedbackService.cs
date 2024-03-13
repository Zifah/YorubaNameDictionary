﻿using Core.Entities.NameEntry.Collections;
using Core.Repositories;

namespace Application.Services
{
    public class NameEntryFeedbackService
    {
        private readonly INameEntryFeedbackRepository _nameEntryFeedbackRepository;

        public NameEntryFeedbackService(INameEntryFeedbackRepository nameEntryFeedbackRepository)
        {
            _nameEntryFeedbackRepository = nameEntryFeedbackRepository;
        }

        public async Task<List<Feedback>> FindAllAsync(string sort)
        {
            return await _nameEntryFeedbackRepository.FindAllAsync(sort);
        }

        public async Task<List<Feedback>> FindByNameAsync(string name, string sortOrder)
        {
            return await _nameEntryFeedbackRepository.FindByNameAsync(name, sortOrder);
        }

        public async Task<bool> AddFeedbackByNameAsync(string name, string feedbackContent)
        {
            return await _nameEntryFeedbackRepository.AddFeedbackByNameAsync(name, feedbackContent);
        }

        public async Task<bool> DeleteAllFeedbackForNameAsync(string name)
        {
            return await _nameEntryFeedbackRepository.DeleteAllFeedbackForNameAsync(name);
        }

        public async Task<Feedback> GetFeedbackByIdAsync(string feedbackId)
        {
            return await _nameEntryFeedbackRepository.GetFeedbackByIdAsync(feedbackId);
        }
    }
}
