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
        DatabaseReader<UserDataPacket> dbReader = new DatabaseReader<UserDataPacket>(conn);
        await dbReader.databaseWrite(generateInsertUserSql, packet);
        return 0;
    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        string CommandText = $"select userID, age, sex, nestID, FitbitID from users";
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        return await dbReader.databaseRead(CommandText, createUserObjects);
    }

    public async Task<int> addSurvey(UserDailyQuizPacket packet){
        DatabaseReader<UserDailyQuizPacket> dbReader = new DatabaseReader<UserDailyQuizPacket>(conn);
        await dbReader.databaseWrite(generateInsertUserDailyQuizSql, packet);
        return 0;
    }

    public async Task<int> submitHeartPacket(HeartDataPacket packet){
        DatabaseReader<HeartDataPacket> dbReader = new DatabaseReader<HeartDataPacket>(conn);
        await dbReader.databaseWrite(generateInsertHeartDataSql, packet);
        return 0;
    }

    private int generateInsertUserSql(MySqlCommand command, UserDataPacket packet){

        command.CommandText = $"INSERT into users values({packet.userID},{packet.age},?sex,?nestID,?fitbitID);";
        command.Parameters.AddWithValue("?sex", packet.sex);
        command.Parameters.AddWithValue("?nestID", packet.nestID);
        command.Parameters.AddWithValue("?fitbitID", packet.fitbitID);
        return 0;

    }

    private int generateInsertUserDailyQuizSql(MySqlCommand command, UserDailyQuizPacket packet){
        command.CommandText = $"INSERT INTO dailyQuizes values ({packet.userID}, {packet.sleepQuality}, {packet.disturbance}, ?disturbanceDetails, ?sleepTime, ?wakeTime);";
        command.Parameters.AddWithValue("?disturbanceDetails", packet.disturbanceDetails);
        command.Parameters.AddWithValue("?sleepTime", packet.sleepTime);
        command.Parameters.AddWithValue("?wakeTime", packet.wakeTime);
        return 0;
    }

    private int generateInsertHeartDataSql(MySqlCommand command, HeartDataPacket packet){
        command.CommandText = $"INSERT into heartdata (userID, fitbitID) VALUES ({packet.userID}, {packet.fitbitID})";
        return 0;
    }

    private List<User> createUserObjects(DataTable dataTable){
        return (from DataRow row in dataTable.Rows
                    select new User{
                        userID = (int)row["userID"],
                        age = -1,
                        sex = string.Empty,
                        nestID = (string)row["nestId"],
                        fitbitID = (string)row["fitbitID"]
                    }).ToList();
    }

    private void checkConnection(){
        if(conn.State != System.Data.ConnectionState.Open){
            conn.Open();
        }
    }

}