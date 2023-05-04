using System.Data;
using Backend.Helpers;
using Backend.Models;
using Backend.Models.Fitbit;
using Backend.Models.Nest;
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
    public async Task<int> linkNestToUser(NestOnboardPacket packet){
        await createNestUser(packet);
        return 0;
    }

    private async Task createNestUser(NestOnboardPacket packet){
        DatabaseReader<NestOnboardPacket> dbReader = new DatabaseReader<NestOnboardPacket>(conn);
        await dbReader.databaseWrite(generateInsertNestUserSql, packet);
        return;
    }

    public async Task<int> refreshFitbitUserAuth(FitbitRefreshAuthResponse packet){
        DatabaseReader<FitbitRefreshAuthResponse> dbReader = new DatabaseReader<FitbitRefreshAuthResponse>(conn);
        await dbReader.databaseWrite(generateRefreshFitbitUserSql, packet);
        return 0;
    }

    public async Task<int> refreshNestUserAuth(NestRefreshAuthResponse packet){
        DatabaseReader<NestRefreshAuthResponse> dbReader = new DatabaseReader<NestRefreshAuthResponse>(conn);
        await dbReader.databaseWrite(generateRefreshNestUserSql, packet);
        return 0;
    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        //Get Users
        string CommandText = $"select users.userID, age, sex, users.fitbitID, fitbitUsers.accessToken as fitbitAccess, fitbitUsers.refreshToken as fitbitRefresh, fitbitUsers.expires as fitbitExpires, nestUsers.accessToken as nestAccess, nestUsers.refreshToken as nestRefresh, nestUsers.expires as nestExpires from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID left join nestUsers on users.userID = nestUsers.userID;";
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        return await dbReader.databaseRead(CommandText, createUserObjects);
    }

    public async Task<User> getUser(int userID){
        string CommandText = $"select users.userID, age, sex, users.fitbitID, fitbitUsers.accessToken as fitbitAccess, fitbitUsers.refreshToken as fitbitRefresh, fitbitUsers.expires as fitbitExpires, nestUsers.accessToken as nestAccess, nestUsers.refreshToken as nestRefresh, nestUsers.expires as nestExpires from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID left join nestUsers on users.userID = nestUsers.userID where users.userID = {userID};";
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
        command.CommandText = $"INSERT into users values({packet.userID},{packet.age},?sex,?fitbitID);";
        command.Parameters.AddWithValue("?sex", packet.sex);
        command.Parameters.AddWithValue("?fitbitID", null);
        return 0;
    }

    private int generateInsertUserDailyQuizSql(MySqlCommand command, UserDailyQuizPacket packet){
        command.CommandText = $"INSERT INTO dailyQuizes values ({packet.userID}, {packet.sleepSession}, ?q1, ?q2, ?q3, ?q4, ?q5, ?q6, ?q7, ?sleepTime);";
        command.Parameters.AddWithValue("?q1", packet.q1);
        command.Parameters.AddWithValue("?q2", packet.q2);
        command.Parameters.AddWithValue("?q3", packet.q3);
        command.Parameters.AddWithValue("?q4", packet.q4);
        command.Parameters.AddWithValue("?q5", packet.q5);
        command.Parameters.AddWithValue("?q6", packet.q6);
        command.Parameters.AddWithValue("?q7", packet.q7);
        command.Parameters.AddWithValue("?sleepTime", packet.sleepTime);
        return 0;
    }

    private int generateLinkFitbitAccountSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"UPDATE users SET fitbitID = ?fitbitID WHERE users.userID = ?userID;";
        command.Parameters.AddWithValue("?fitbitID", packet.fitbitID);
        command.Parameters.AddWithValue("?userID", packet.userID);
        return 0;
    }

    private int generateInsertFitbitUserSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"INSERT INTO fitbitUsers values(?fitbitID,?accessToken,?refreshToken, NULL);";
        command.Parameters.AddWithValue("?fitbitID",packet.fitbitID);
        command.Parameters.AddWithValue("?accessToken",packet.accessToken);
        command.Parameters.AddWithValue("?refreshToken",packet.refreshToken);
        return 0;
    }

    private int generateInsertNestUserSql(MySqlCommand command, NestOnboardPacket packet){
        command.CommandText = $"INSERT INTO nestUsers values(?nestID,?accessToken,?refreshToken,?expires);";
        command.Parameters.AddWithValue("?nestID",packet.userID);
        command.Parameters.AddWithValue("?accessToken",packet.accessToken);
        command.Parameters.AddWithValue("?refreshToken",packet.refreshToken);
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,packet.expires_in);
        command.Parameters.AddWithValue("?expires", now + offset);
        return 0;
    }

    private int generateRefreshFitbitUserSql(MySqlCommand command, FitbitRefreshAuthResponse packet){
        command.CommandText = $"UPDATE fitbitUsers SET accessToken = ?accessToken, refreshToken = ?refreshToken, expires = ?expires WHERE userID = ?userID;";
        command.Parameters.AddWithValue("?accessToken", packet.access_token);
        command.Parameters.AddWithValue("?refreshToken", packet.refresh_token);
        command.Parameters.AddWithValue("?userID", packet.user_id);
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,packet.expires_in);
        
        command.Parameters.AddWithValue("?expires", now + offset);
        return 0;
    }

    private int generateRefreshNestUserSql(MySqlCommand command, NestRefreshAuthResponse packet){
        command.CommandText = $"UPDATE nestUsers SET accessToken = ?accessToken, expires = ?expires WHERE userID = ?userID;";
        command.Parameters.AddWithValue("?accessToken", packet.access_token);
        command.Parameters.AddWithValue("?userID", packet.user_id);
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,packet.expires_in);
        
        command.Parameters.AddWithValue("?expires", now + offset);
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
                        fitbitData = new FitbitAuth{
                            fitbitID = ConvertFromDBVal<string>(row["fitbitID"]),
                            accessToken = ConvertFromDBVal<string>(row["fitbitAccess"]),
                            refreshToken = ConvertFromDBVal<string>(row["fitbitRefresh"]),
                            expires = ConvertFromDBVal<DateTime>(row["fitbitExpires"])
                        },
                        nestData = new NestAuth{
                            accessToken = ConvertFromDBVal<string>(row["nestAccess"]),
                            refreshToken = ConvertFromDBVal<string>(row["nestRefresh"]),
                            expires = ConvertFromDBVal<DateTime>(row["nestExpires"])
                        }
                    }).ToList();
    }

    private List<FitbitAuth> createFitbitAuthObjects(DataTable dataTable){
        return (from DataRow row in dataTable.Rows
                    select new FitbitAuth{
                        fitbitID = (string)row["fitbitID"],
                        accessToken = (string)row["accessToken"],
                        refreshToken = (string)row["refreshToken"],
                        expires = ConvertFromDBVal<DateTime>(row["expires"])
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