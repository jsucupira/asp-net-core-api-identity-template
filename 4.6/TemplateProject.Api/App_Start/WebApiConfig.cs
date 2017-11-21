using System.Net.Http.Formatting;
using System.Web.Http;
using CacheCow.Server;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TemplateProject.Api.Attributes;

namespace TemplateProject.Api
{
    public static class WebApiConfig
    {
        private static void ConfigureApi(HttpConfiguration config)
        {
            int index = config.Formatters.IndexOf(config.Formatters.JsonFormatter);
            config.Formatters[index] = new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore}
            };
        }

        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());

            ConfigureApi(config);
            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new CachingHandler(config));
        }
    }
}