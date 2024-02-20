using Comm.WebUtil;

using Dapper;

using Microsoft.Data.SqlClient;

using NLog;

using System.Collections;
using System.Data;

namespace Comm.Model
{
    public class BaseRepository<TEntity> : IRepository where TEntity : class
    {
        public string DBName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public SqlBuilder SqlBuilder { get; set; }
        public Type EntityType => typeof(TEntity);
        public IDbConnection Connection => CreateConnectionByDBName(DBName);
        public MyAppConfig _memberConfig { get; set; } = new MyAppConfig();
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public DbContext DB { get; private set; }
        public BaseRepository()
        {
            DBName = DBName;
            TableName = TableName;
            SqlBuilder = new SqlBuilder(TableName, this);
            if (DB == null)
            {
                DB = new DbContext();
            }
            SqlMapper.AddTypeMap(typeof(string), DbType.AnsiString);
        }
        static IConfigurationRoot Configuration { get; set; }
        IDbConnection conn = null;
        public IDbConnection CreateConnectionByDBName(string DBName)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // 設定根目錄
            .AddJsonFile("appsettings.json", true);   // 讀取appsettings.json檔案
            Configuration = builder.Build();
            var setting = Configuration.Get<MyAppConfig>();

            //conn = new SqlConnection(setting.ConnectionStrings.SQLComm);

            IDbConnection conn = null;
            if (DBName.Contains("SQL"))
            {
                // conn = setting.ConnectionStrings.CDM;
                conn = new SqlConnection(setting.ConnectionStrings.Connsql);

            }
            return conn;



        }
        /// <summary>
        /// 取得Entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetItem(dynamic param = null)
        {
            SqlQuery sqlQuery = SqlBuilder.BuildSelect(TableName, param);
            return Connection.Query<TEntity>(sqlQuery.Sql, sqlQuery.Param);

        }

        /// <summary>
        /// 自組SQL & param
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetItem(string sql, DynamicParameters dp = null)
        {
            using (var Connection = CreateConnectionByDBName(DBName))
            {
                return Connection.Query<TEntity>(sql, dp);
            }

        }

        public IEnumerable<TEntity> GetItem(string sql)
        {
            using (var Connection = CreateConnectionByDBName(DBName))
            {
                return Connection.Query<TEntity>(sql);
            }


        }
        public IEnumerable<TEntity> GetAllItems()
        {
            SqlQuery sqlQuery = SqlBuilder.BuildSelect(TableName, null);
            return Connection.Query<TEntity>(sqlQuery.Sql).ToList();
        }


        public int AddItem(string sql, dynamic param = null)
        {
            // log.Info($"Command: {sql}");
            return Connection.Execute(sql, param as object);
        }

        public int AddItem(TEntity TEntity)
        {
            SqlQuery sqlQuery = SqlBuilder.BuildInsert(TableName, TEntity, EntityType);
            _logger.Info($"Command: {sqlQuery.Sql} ");
            return Connection.Execute(sqlQuery.Sql, sqlQuery.Param);
        }

        public int Update(string sql, dynamic param = null)
        {

            return Connection.Execute(sql, param as object);
        }
        public int Update(TEntity TEntity)
        {
            SqlQuery sqlQuery = SqlBuilder.BuildUpdate(TableName, TEntity, EntityType);

            return Connection.Execute(sqlQuery.Sql, sqlQuery.Param);
        }

        public int Delete(TEntity TEntity)
        {
            SqlQuery sqlQuery = SqlBuilder.BuildDelete(TableName, TEntity, EntityType);

            return Connection.Execute(sqlQuery.Sql, sqlQuery.Param);
        }

        public int Delete(string sql, dynamic param = null)
        {
            //log.Info($"Command: {sql}");
            return Connection.Execute(sql, param as object);
        }
        public void Dispose()
        {
            if (Connection?.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }


        #region IRepository


        IEnumerable IRepository.GetItem(dynamic param)
        {
            throw new NotImplementedException();
        }

        IEnumerable IRepository.GetItem(string sql)
        {
            throw new NotImplementedException();
        }






        #endregion

    }
}
