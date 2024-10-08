﻿using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;

namespace Infrastructure.Twitter
{
    public static partial class DependencyInjection
    {
        private const string ConfigSectionName = "Twitter";

        public static IServiceCollection AddTwitterClient(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetRequiredSection(ConfigSectionName);

            services.Configure<TwitterConfig>(config);

            services.AddSingleton<ITwitterClient>(provider =>
            {
                var twitterConfig = config.Get<TwitterConfig>()!;
                return new TwitterClient(
                    twitterConfig.ConsumerKey,
                    twitterConfig.ConsumerSecret,
                    twitterConfig.AccessToken,
                    twitterConfig.AccessTokenSecret
                );
            });

            services.AddSingleton<ITwitterClientV2, TwitterClientV2>();
            return services;
        }
    }
}
