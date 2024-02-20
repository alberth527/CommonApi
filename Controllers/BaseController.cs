using Comm.WebUtil;



using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

using NLog;
using NLog.Web;

using System.Net;

namespace CommonApi.Controllers
{
    [EnableCors("CorsPolicy")]
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase, IDisposable
    {
        protected static Logger _logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            DB = new DbContext();
            _httpContextAccessor = httpContextAccessor;
            _userid = new JwtHelper(configuration).Getuserid(_httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?.Replace("Bearer ", string.Empty));
            _IP = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            _hostname = Dns.GetHostName();
        }
        public string _userid { get; private set; }
        public string _IP { get; private set; }
        public string _hostname { get; private set; }

        public DbContext DB { get; private set; }
        public const string actionadd = "add";
        public const string actionupd = "upd";
        public const string actiondel = "del";

        void IDisposable.Dispose()
        {
            DB.Dispose();
        }
    }
}
