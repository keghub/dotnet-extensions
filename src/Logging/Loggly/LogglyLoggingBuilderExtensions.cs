// ReSharper disable CheckNamespace

using System;
using System.Linq;
using System.Net.Http;
using EMG.Extensions.Logging.Loggly;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Logging
{
    public static class LogglyLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddLoggly(this ILoggingBuilder builder, IConfiguration configuration = null, Action<LogglyOptions> configureOptions = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddLoggly(builder, () =>
            {
                var options = new LogglyOptions();

                configuration?.Bind(options);
                configureOptions?.Invoke(options);

                return options;
            });
        }

        public static ILoggingBuilder AddLoggly(this ILoggingBuilder builder, Func<LogglyOptions> optionsFactory)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (optionsFactory == null)
            {
                throw new ArgumentNullException(nameof(optionsFactory));
            }

            var options = optionsFactory();

            if (options.ApiKey == null)
            {
                throw new ArgumentNullException(nameof(options.ApiKey));
            }

            builder.Services.AddSingleton(options);

            builder.Services.AddSingleton<ILogglyProcessor, LogglyProcessor>();

            builder.Services.AddSingleton(CreateLogglyClient);

            builder.Services.AddSingleton<ILoggerProvider, LogglyLoggerProvider>();

            return builder;
        }

        private static ILogglyClient CreateLogglyClient(IServiceProvider serviceProvider)
        {
            var o = serviceProvider.GetRequiredService<LogglyOptions>();
            var http = CreateHttpClient(o);

            return new LogglyHttpClient(http, o);
        }

        private static HttpClient CreateHttpClient(LogglyOptions options)
        {
            var http = new HttpClient
            {
                BaseAddress = new Uri($"{options.LogglyScheme}://{options.LogglyHost}")
            };

            if (options.Tags.Any())
            {
                http.DefaultRequestHeaders.Add("X-LOGGLY-TAG", string.Join(",", options.Tags));
            }

            return http;
        }
    }
}