using Microsoft.Data.SqlClient;

using System.Data;

namespace Comm.WebUtil
{
    public class DbContext : IDisposable
    {
        private IDbConnection _connection;
        static IConfigurationRoot Configuration { get; set; }
        private MyAppConfig config;
        public DbContext()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())  // 設定根目錄
           .AddJsonFile("appsettings.json", true);   // 讀取appsettings.json檔案
            Configuration = builder.Build();
            config = Configuration.Get<MyAppConfig>();
        }

        /// <summary>
        /// SQLConnection
        /// </summary>
        public IDbConnection SQLConnection
        {
            get
            {

                return new SqlConnection(config.ConnectionStrings.Connsql);
            }
            private set
            {
                _connection = value;
            }
        }


















        public void Dispose()
        {
            if (_connection?.State != ConnectionState.Closed)
            {
                _connection?.Close();
            }
        }
    }
}
