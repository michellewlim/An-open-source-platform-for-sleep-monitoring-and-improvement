using MySql.Data.MySqlClient;
using System.Reflection;
using System.Data;

namespace Backend.Helpers;

public class DatabaseReader<T>
{
    private MySqlConnection conn;

    public List<T> results;

    public DatabaseReader(MySqlConnection conn)
    {
        this.conn = conn;
        results = new List<T>();
    }
    public async Task<List<T>> databaseRead(string sqlCommand, Func<DataTable, List<T>> createResult)
    {
        this.checkConnection();
        PropertyInfo[] propertyInfos;
        propertyInfos = typeof(T).GetProperties(BindingFlags.Public);

        MySqlCommand myCommand = conn.CreateCommand();

        MySqlTransaction myTrans;
        myTrans = conn.BeginTransaction();
        myCommand.Connection = conn;
        myCommand.Transaction = myTrans;

        myCommand.CommandText = sqlCommand;
        try
            {
            using (var response = await myCommand.ExecuteReaderAsync())
            {
                var dataTable = new DataTable();
                dataTable.Load(response);
                
                results = createResult(dataTable);
                
                response.Close();
                myTrans.Commit();
                return results;

            };
        }
            catch(Exception e)
            {
                myTrans.Rollback();
                throw(e);
            }
    }

    public async Task databaseWrite(Func<MySqlCommand, T, int> createSqlCommand, T packet)
    {
        this.checkConnection();

        MySqlCommand myCommand = conn.CreateCommand();
        MySqlTransaction myTrans;
        myTrans = conn.BeginTransaction();
        myCommand.Connection = conn;
        myCommand.Transaction = myTrans;

        try
        {
            createSqlCommand(myCommand, packet);
            //myCommand.CommandText = sqlCommand;
            await myCommand.ExecuteNonQueryAsync();
            myTrans.Commit();
        }
        catch(Exception e)
        {
            myTrans.Rollback();
            throw(e);
        }
        return;
    }

    private void checkConnection()
    {
        if (conn.State != System.Data.ConnectionState.Open)
        {
            conn.Open();
        }
    }


}