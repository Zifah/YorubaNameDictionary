﻿using Application.Events;
using Application.Services;
using Core.Cache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EventHandlers
{

    public class NameDeletedEventHandler : INotificationHandler<NameDeletedAdapter>
    {
        private IRecentIndexesCache _recentIndexesCache;
        private IRecentSearchesCache _recentSearchesCache;

        public NameDeletedEventHandler(IRecentIndexesCache recentIndexesCache, IRecentSearchesCache recentSearchesCache)
        {
            _recentIndexesCache = recentIndexesCache;
            _recentSearchesCache = recentSearchesCache;
        }

        public async Task Handle(NameDeletedAdapter notification, CancellationToken cancellationToken)
        {
            try
            {
                await _recentIndexesCache.Remove(notification.Name);
                await _recentSearchesCache.Remove(notification.Name);              
            }
            catch (Exception ex)
            {
                //TODO log this
            }
        }
    }
}