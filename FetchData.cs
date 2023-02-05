using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

namespace Fetcher
{
    public class FetchData
    {
        private readonly ILogger _logger;

        public FetchData(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FetchData>();
        }

        [Function("fetchBlob")]
        public async Task<HttpResponseData> FetchBlob
        (
            [HttpTrigger(AuthorizationLevel.Function, Http.Get)] HttpRequestData req,
            [BlobInput("static-files/index.html", Connection = "FetcherConnection")] string indexHtml,
            string name
        )
        {
            _logger.LogInformation("C# HTTP trigger function {FncName} processed a request.", "fetchBlob");

            return await PrepareResponseAsync(req.CreateResponse(HttpStatusCode.OK), indexHtml, name);
        }

        [Function("fetchLocal")]
        public async Task<HttpResponseData> FetchLocal
        (
            [HttpTrigger(AuthorizationLevel.Function, Http.Get)] HttpRequestData req,
            string name
        )
        {
            _logger.LogInformation("C# HTTP trigger function {FncName} processed a request.", "fetchLocal");

            var path = System.IO.Path.Join(Environment.CurrentDirectory, "static/index.html");
            var indexHtml = await System.IO.File.ReadAllTextAsync(path);

            return await PrepareResponseAsync(req.CreateResponse(HttpStatusCode.OK), indexHtml, name);
        }

        private static async Task<HttpResponseData> PrepareResponseAsync(HttpResponseData httpResponseData, string indexHtml, string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                indexHtml = indexHtml.Replace("Hi!", $"Hi, {name}!");

            httpResponseData.Headers.Add("Content-Type", MediaTypeNames.Text.Html);
            await httpResponseData.WriteStringAsync(indexHtml, Encoding.UTF8);
            return httpResponseData;
        }
    }
}
