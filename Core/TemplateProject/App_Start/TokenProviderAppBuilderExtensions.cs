using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace TemplateProject.Api
{
    public static class TokenProviderAppBuilderExtensions
    {
        /// <summary>
        /// Uses the simple token provider.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="options">The options.</param>
        /// <returns>IApplicationBuilder.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// app
        /// or
        /// options
        /// </exception>
        public static IApplicationBuilder UseSimpleTokenProvider(this IApplicationBuilder app, TokenProviderOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));
        }
    }
}