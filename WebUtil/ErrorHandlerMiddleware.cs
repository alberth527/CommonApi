using NLog;

using System.Net;
using System.Text;

namespace Comm.WebUtil
{
    public class ErrorHandlerMiddleware
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly RequestDelegate _next;

        public string _hostname { get; private set; }
        public string _IP { get; private set; }

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _hostname = Dns.GetHostName();
        }

        public async Task Invoke(HttpContext context)
        {
            _IP = context.Connection.RemoteIpAddress.ToString();

            //First, get the incoming request
            var request = await FormatRequest(context.Request);

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using var responseBody = new MemoryStream();


            //...and use that for the temporary response body
            context.Response.Body = responseBody;
            //讀取完內容後，Stream Position 又會被指到結尾，為了回原本的 Response.Body，所以要把 Stream Position 指回起始位置。
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            //Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

            //Format the response from the server
            var response = await FormatResponse(context.Response);

            //TODO: Save log to chosen datastore
            if (context.Response.StatusCode != 200 && context.Response.StatusCode != 204 && context.Response.StatusCode != 404 && context.Response.StatusCode != 101)
            {
                try
                {
                    _logger.Info($"request=>{request},response=>{response}");
                }
                catch (Exception ex)
                {


                }

            }

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);



        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = request.Body;

            //This line allows us to set the reader for the request back at the beginning of its stream.
            request.EnableBuffering();

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var bodyAsText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()
            request.Body = body;

            return $"{bodyAsText}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}

