using System.Collections;

namespace Comm.Model
{
    /// <summary>
    /// Data Repository 
    /// Implement Select, Insert, Update, Delete 
    /// </summary>
    public interface IRepository : IDisposable
    {

        IEnumerable GetItem(dynamic param = null);
        IEnumerable GetItem(string sql);
        int AddItem(string sql, dynamic param = null);
        int Update(string sql, dynamic param = null);
        int Delete(string sql, dynamic param = null);

    }
}
