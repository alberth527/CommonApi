using Dapper;

using System.Text;

namespace Comm.WebUtil
{
    public class SqlQuery
    {
        public SqlQuery()
        {
            Builder = new StringBuilder();
        }

        /// <summary>
        /// SQL string builder
        /// </summary>
        public StringBuilder Builder { get; }

        /// <summary>
        /// SQL 指令
        /// </summary>
        public string Sql => Builder.ToString().TrimEnd();

        public object Param { get; set; }

        /// <summary>
        /// SQL 指令中的參數
        /// </summary>
        public DynamicParameters Params { get; set; } = new DynamicParameters();

    }
}
