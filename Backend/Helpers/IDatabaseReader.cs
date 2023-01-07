using System.Data;

namespace Backend.Helpers;
public interface IDatabaseReader<T>
{
    Task<List<T>> databaseRead(string sqlCommand, Func<DataTable, List<T>> createResult);
}