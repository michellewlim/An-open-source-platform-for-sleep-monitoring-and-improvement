using System.Data;
using Backend.Helpers;
using Backend.Models;
using MySql.Data.MySqlClient;
namespace Backend.Controllers;

public class DatabaseController : IDatabaseController{

    private readonly IConfiguration _config;
    private MySqlConnection conn; 

    public DatabaseController(IConfiguration config){
        _config = config;
        conn = new MySqlConnection(_config["Databases:SeniorDesignConnectionString"]); 
    }

    //Submit user data to the database
    public async Task<int> addUser(UserDataPacket packet){
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        await dbReader.databaseWrite(generateInsertUserSql(packet));
        return 0;
    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        string CommandText = $"select Id from users";
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        return await dbReader.databaseRead(CommandText, createUserObjects);
    }

    public async Task<int> submitHeartPacket(HeartDataPacket packet){
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        await dbReader.databaseWrite(generateInsertHeartDataSql(packet));
        return 0;
    }

    private string generateInsertUserSql(UserDataPacket packet){
        return $"INSERT into users (Id) VALUES ({packet.userID});";
    }

    private string generateInsertHeartDataSql(HeartDataPacket packet){
        return $"INSERT into heartdata (Id, FitbitId) VALUES ({packet.userID}, {packet.fitbitID})";
    }

    private List<User> createUserObjects(DataTable dataTable){
        return (from DataRow row in dataTable.Rows
                    select new User{
                        userID = (int)row["id"]
                    }).ToList();
    }

    private void checkConnection(){
        if(conn.State != System.Data.ConnectionState.Open){
            conn.Open();
        }
    }

}