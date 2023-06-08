using MySql.Data.MySqlClient;
using System.Reflection;
using System.Data;

namespace Backend.Helpers;

public class DatabaseReader<T>
{
    /// <summary>
    /// The connection to the database
    /// </summary>
    private MySqlConnection conn;

    /// <summary>
    /// The results of the database query
    /// </summary>
    public List<T> results;

    public DatabaseReader(MySqlConnection conn)
    {
        this.conn = conn;
        results = new List<T>();
    }

    /// <summary>
    /// Reads from the database
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="createResult">A function that takes a DataTable and returns a list of T</param>
    /// <returns>A list of T</returns>
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
                conn.Close();
                Console.WriteLine(e.Message);
                throw(e);
            }
    }

    /// <summary>
    /// Writes to the database
    /// </summary>
    /// <param name="sqlCommand">The SQL command to execute</param>
    /// <param name="packet">The packet to write to the database</param>
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
            conn.Close();
            Console.WriteLine(e.Message);
            throw(e);
        }
        return;
    }

    /// <summary>
    /// Checks if the connection is open, and opens it if it is not
    /// </summary>
    private void checkConnection()
    {
        if (conn.State != System.Data.ConnectionState.Open)
        {
            conn.Open();
        }
    }


}