using Comm.Model;

using NLog;

using System.ComponentModel.DataAnnotations;

namespace Comm.WebUtil
{
    public class SqlBuilder
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();
        public SqlBuilder(string tableName, IRepository repository)
        {
            Repository = repository;
            TableName = tableName;
        }

        /// <summary>
        /// SQL 指令的 Table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Entity's Repository
        /// </summary>
        private IRepository Repository { get; set; }

        /// <summary>
        /// SQL 指令的參數的符號
        /// </summary>
        private string Sign
        {
            get
            {
                //return DbProvider == DbProvider.Oracle ? "@" : ":";
                return "@";
            }
        }

        /// <summary>
        /// Build Select SQL
        /// </summary>
        /// <param name="param">Query condtion</param>
        public virtual SqlQuery BuildSelect(string TableName, dynamic param = null)
        {
            SqlQuery sqlQuery = new SqlQuery();
            Dictionary<string, object> dicParams = new Dictionary<string, object>();  //存放 colName = value 的value (因為SQL要參數化, 所以value的部份要另外存起來)
            sqlQuery.Builder.Append($"SELECT * FROM {TableName}  ");
            if (param != null)
            {
                string where = string.Empty;
                foreach (var prop in param.GetType().GetProperties())
                {
                    string propName = prop.Name;
                    object value = prop.GetValue(param, null);
                    if (value != null && !string.IsNullOrEmpty(value.ToString()) && value.ToString() != "0")
                    {
                        dicParams.Add(propName, value);
                        where += string.IsNullOrEmpty(where) ? $" WHERE {propName} = @{propName}" : $" AND {propName} = @{propName}";
                    }
                }
                sqlQuery.Builder.Append(where);
                sqlQuery.Param = dicParams;
            }
            //_logger.Info(sqlQuery.Builder);
            return sqlQuery;
        }
        /// <summary>
        /// Build Update SQL
        /// </summary>
        /// <param name="item">Entity</param>
        public virtual SqlQuery BuildInsert(string TableName, dynamic item, Type type)
        {
            SqlQuery sqlQuery = new SqlQuery();
            IEnumerable<string> columnNames, parameters;
            var hashSet = new HashSet<string>();
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                object value = prop.GetValue(item);

                if (!string.IsNullOrEmpty(Convert.ToString(value)))
                {
                    if (value.GetType().Name.Contains("List")) continue;
                    hashSet.Add(prop.Name);
                }
            }
            columnNames = props.Where(prop => hashSet.Contains(prop.Name)).Select(prop => prop.Name);
            parameters = props.Where(prop => hashSet.Contains(prop.Name)).Select(prop => "@" + prop.Name);

            sqlQuery.Builder.Append($"INSERT INTO  {TableName} ( {string.Join(",", columnNames.ToArray())} ) VALUES ( {string.Join(",", parameters.ToArray())} ) ");
            sqlQuery.Param = item;
            _logger.Info(sqlQuery.Builder);
            return sqlQuery;
        }


        /// <summary>
        /// Build Update SQL
        /// </summary>
        /// <param name="item">Entity</param>
        /// <returns></returns>
        public virtual SqlQuery BuildUpdate(string TableName, dynamic item, Type type)
        {
            SqlQuery sqlQuery = new SqlQuery();
            var props = type.GetProperties();
            var wheres = props.Where(p => p.GetCustomAttributes(false).Any(key => key.GetType() == typeof(KeyAttribute)))
            .Where(p => p.GetValue(item) != null).Select(p => p.Name + " = @" + p.Name);
            _logger.Info(wheres);
            var sets = props.Where(p => !string.Join(",", wheres.ToArray()).Contains(p.Name))
                .Where(p => p.GetValue(item) != null && !p.GetValue(item).GetType().Name.Contains("List"))
                .Select(p => p.Name + " = @" + p.Name);

            //不能沒有PK
            if (wheres == null)
            {
                throw new SystemException("Primary key is empty.");
            }

            sqlQuery.Builder.Append($"UPDATE {TableName} SET  {string.Join(", ", sets.ToArray())} WHERE {string.Join(" AND ", wheres.ToArray())} ");
            sqlQuery.Param = item;
            _logger.Info(sqlQuery.Builder);
            return sqlQuery;
        }
        /// <summary>
        /// Build Delete SQL
        /// </summary>
        /// <param name="item">Entity</param>
        /// <returns></returns>
        public virtual SqlQuery BuildDelete(string TableName, dynamic item, Type type)
        {

            SqlQuery sqlQuery = new SqlQuery();
            var props = type.GetProperties();
            var wheres = props.Where(p => p.GetCustomAttributes(false).Any(key => key.GetType() == typeof(KeyAttribute)))
            .Where(p => p.GetValue(item) != null).Select(p => p.Name + " = @" + p.Name);
            sqlQuery.Builder.Append($"DELETE FROM {TableName} WHERE {string.Join(" AND ", wheres.ToArray())} ");

            //不能沒有PK
            if (wheres == null)
            {
                throw new SystemException("Primary key is empty.");
            }
            _logger.Info(sqlQuery.Builder);
            sqlQuery.Param = item;
            return sqlQuery;
        }
    }
}
