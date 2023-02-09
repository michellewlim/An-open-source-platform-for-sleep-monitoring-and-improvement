using System.Data;
using Backend.Helpers;
using Backend.Models;
using MySql.Data.MySqlClient;
using Backend.Models.Fitbit;
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

    public async Task<int> linkFitbitToUser(FitbitOnboardPacket packet){
        await linkFitbitAccountToUser(packet);
        await createFitbitUser(packet);
        return 0;
    }

    private async Task linkFitbitAccountToUser(FitbitOnboardPacket packet){
        DatabaseReader<FitbitOnboardPacket> dbReader = new DatabaseReader<FitbitOnboardPacket>(conn);
        await dbReader.databaseWrite(generateLinkFitbitAccountSql, packet); 
        return;
    }

    private async Task createFitbitUser(FitbitOnboardPacket packet){
        DatabaseReader<FitbitOnboardPacket> dbReader = new DatabaseReader<FitbitOnboardPacket>(conn);
        await dbReader.databaseWrite(generateInsertFitbitUserSql, packet);
        return;

    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        //Get Users
        string CommandText = $"select users.userID, age, sex, nestID, users.fitbitID, fitbitUsers.accessToken, fitbitUsers.refreshToken from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID;";
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        return await dbReader.databaseRead(CommandText, createUserObjects);
    }

    public async Task<User> getUser(int userID){
        string CommandText = $"select users.userID, age, sex, nestID, users.fitbitID, fitbitUsers.accessToken, fitbitUsers.refreshToken from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID where users.userID = {userID};";
        DatabaseReader<User> dbUserReader = new DatabaseReader<User>(conn);
        List<User> user = await dbUserReader.databaseRead(CommandText, createUserObjects);

        if(user.Count() == 0){
            throw new RowNotInTableException("User Not Found");
        }
        return user[0];
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
        command.Parameters.AddWithValue("?fitbitID", null);
        return 0;
    }

    private int generateInsertUserDailyQuizSql(MySqlCommand command, UserDailyQuizPacket packet){
        command.CommandText = $"INSERT INTO dailyQuizes values ({packet.userID}, {packet.sleepQuality}, ?disturbance, ?disturbanceDetails, ?sleepTime, ?wakeTime);";
        command.Parameters.AddWithValue("?disturbance", packet.disturbance);
        command.Parameters.AddWithValue("?disturbanceDetails", packet.disturbanceDetails);
        command.Parameters.AddWithValue("?sleepTime", packet.sleepTime);
        command.Parameters.AddWithValue("?wakeTime", packet.wakeTime);
        return 0;
    }

    private int generateLinkFitbitAccountSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"UPDATE users SET fitbitID = ?fitbitID WHERE users.userID = ?userID;";
        command.Parameters.AddWithValue("?fitbitID", packet.fitbitID);
        command.Parameters.AddWithValue("?userID", packet.userID);
        return 0;
    }

    private int generateInsertFitbitUserSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"INSERT INTO fitbitUsers values(?fitbitID,?accessToken,?refreshToken);";
        command.Parameters.AddWithValue("?fitbitID",packet.fitbitID);
        command.Parameters.AddWithValue("?accessToken",packet.accessToken);
        command.Parameters.AddWithValue("?refreshToken",packet.refreshToken);
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
                        age = (int)row["age"],
                        sex = (string)row["sex"],
                        nestID = (string)row["nestId"],
                        fitbitData = new FitbitAuth{
                            fitbitID = ConvertFromDBVal<string>(row["fitbitID"]),
                            accessToken = ConvertFromDBVal<string>(row["accessToken"]),
                            refreshToken = ConvertFromDBVal<string>(row["refreshToken"])
                        }
                    }).ToList();
    }

    private List<FitbitAuth> createFitbitAuthObjects(DataTable dataTable){
        return (from DataRow row in dataTable.Rows
                    select new FitbitAuth{
                        fitbitID = (string)row["fitbitID"],
                        accessToken = (string)row["accessToken"],
                        refreshToken = (string)row["refreshToken"]
                    }
        ).ToList();
    }

    private void checkConnection(){
        if(conn.State != System.Data.ConnectionState.Open){
            conn.Open();
        }
    }

    public static T? ConvertFromDBVal<T>(object obj)
{
    if (obj == null || obj == DBNull.Value)
    {
        return default(T); // returns the default value for the type
    }
    else
    {
        return (T)obj;
    }
}

}