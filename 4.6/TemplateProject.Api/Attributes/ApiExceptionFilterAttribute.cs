using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

namespace TemplateProject.Api.Attributes
{
    /// <summary>
    /// Class ApiExceptionFilterAttribute.
    /// </summary>
    /// <seealso cref="System.Web.Http.Filters.ExceptionFilterAttribute" />
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Called when [exception].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="HttpResponseException"></exception>
        /// <exception cref="HttpResponseMessage"></exception>
        /// <exception cref="StringContent">There was an issue processing your request.  Please try again!</exception>
        public override void OnException(HttpActionExecutedContext context)
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("There was an issue processing your request.  Please try again!"),
                ReasonPhrase = "Server Error"
            });
        }
    }
}