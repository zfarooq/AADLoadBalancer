using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SP.LoadBalancer.WebAPI
{
    public class ContentActionResult : IHttpActionResult
    {
        private readonly HttpStatusCode statusCode;
        private readonly string responseMessage;
        private readonly object data;
        private readonly HttpRequestMessage request;
        private readonly bool reducedMode;

        public ContentActionResult(HttpStatusCode statusCode, string responseMessage, object data, HttpRequestMessage request)
        {
            this.statusCode = statusCode;
            this.responseMessage = responseMessage;
            this.data = data;
            this.request = request;
        }

        public ContentActionResult(HttpStatusCode statusCode, string responseMessage, object data, HttpRequestMessage request, bool reducedMode)
            : this(statusCode, responseMessage, data, request)
        {
            this.reducedMode = reducedMode;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = this.request.CreateResponse(this.statusCode);
            var formatter = request.GetConfiguration().Formatters.JsonFormatter;

            if (reducedMode == true)
            {
                formatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate;
                formatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            }
            else
            {
                formatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
                formatter.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
            }

            response.Content = new ObjectContent<object>(new
            {
                message = this.responseMessage,
                data = this.data
            }, formatter);


            return Task.FromResult(response);
        }
    }
}