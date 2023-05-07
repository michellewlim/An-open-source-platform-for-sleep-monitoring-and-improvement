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
    
    ///<summary>
    ///Add a user to the database
    ///</summary>
    public async Task<int> addUser(UserDataPacket packet){
        DatabaseReader<UserDataPacket> dbReader = new DatabaseReader<UserDataPacket>(conn);
        await dbReader.databaseWrite(generateInsertUserSql, packet);
        return 0;
    }
    
    /// <summary>
    /// Link a user's fitbit account to their user account in the database
    /// </summary>
    public async Task<int> linkFitbitToUser(FitbitOnboardPacket packet){
        await linkFitbitAccountToUser(packet);
        await createFitbitUser(packet);
        return 0;
    }

    /// <summary>
    /// Update the users table to include that user's fitbit ID
    /// </summary>
    private async Task linkFitbitAccountToUser(FitbitOnboardPacket packet){
        DatabaseReader<FitbitOnboardPacket> dbReader = new DatabaseReader<FitbitOnboardPacket>(conn);
        await dbReader.databaseWrite(generateLinkFitbitAccountSql, packet); 
        return;
    }

    /// <summary>
    /// Create a new entry in the fitbitUsers table for the user's fitbit credentials
    /// </summary>
    private async Task createFitbitUser(FitbitOnboardPacket packet){
        DatabaseReader<FitbitOnboardPacket> dbReader = new DatabaseReader<FitbitOnboardPacket>(conn);
        await dbReader.databaseWrite(generateInsertFitbitUserSql, packet);
        return;

    /// <summary>
    /// Link a user's nest account to their user account in the database
    /// </summary>
    }
    public async Task<int> linkNestToUser(NestOnboardPacket packet){
        await createNestUser(packet);
        return 0;
    }

    /// <summary>
    /// Create a new entry in the nestUsers table for the user's nest credentials
    /// </summary>
    private async Task createNestUser(NestOnboardPacket packet){
        DatabaseReader<NestOnboardPacket> dbReader = new DatabaseReader<NestOnboardPacket>(conn);
        await dbReader.databaseWrite(generateInsertNestUserSql, packet);
        return;
    }

    /// <summary>
    /// update a user's fitbit credentials in the database
    /// </summary>
    public async Task<int> refreshFitbitUserAuth(FitbitRefreshAuthResponse packet){
        DatabaseReader<FitbitRefreshAuthResponse> dbReader = new DatabaseReader<FitbitRefreshAuthResponse>(conn);
        await dbReader.databaseWrite(generateRefreshFitbitUserSql, packet);
        return 0;
    }

    /// <summary>
    /// update a user's nest credentials in the database
    /// </summary>
    public async Task<int> refreshNestUserAuth(NestRefreshAuthResponse packet){
        DatabaseReader<NestRefreshAuthResponse> dbReader = new DatabaseReader<NestRefreshAuthResponse>(conn);
        await dbReader.databaseWrite(generateRefreshNestUserSql, packet);
        return 0;
    }

    /// <summary>
    /// Get a list of all users from the database
    /// </summary>
    public async Task<List<User>> getUsers(){
        //Get Users
        string CommandText = $"select users.userID, age, sex, users.fitbitID, fitbitUsers.accessToken as fitbitAccess, fitbitUsers.refreshToken as fitbitRefresh, fitbitUsers.expires as fitbitExpires, nestUsers.accessToken as nestAccess, nestUsers.refreshToken as nestRefresh, nestUsers.expires as nestExpires from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID left join nestUsers on users.userID = nestUsers.userID;";
        DatabaseReader<User> dbReader = new DatabaseReader<User>(conn);
        return await dbReader.databaseRead(CommandText, createUserObjects);
    }

    /// <summary>
    /// Retrieves a specific user from the database
    /// </summary>
    public async Task<User> getUser(int userID){
        string CommandText = $"select users.userID, age, sex, users.fitbitID, fitbitUsers.accessToken as fitbitAccess, fitbitUsers.refreshToken as fitbitRefresh, fitbitUsers.expires as fitbitExpires, nestUsers.accessToken as nestAccess, nestUsers.refreshToken as nestRefresh, nestUsers.expires as nestExpires from users left join fitbitUsers on users.fitbitID = fitbitUsers.userID left join nestUsers on users.userID = nestUsers.userID where users.userID = {userID};";
        DatabaseReader<User> dbUserReader = new DatabaseReader<User>(conn);
        List<User> user = await dbUserReader.databaseRead(CommandText, createUserObjects);
        if(user.Count() == 0){
            throw new RowNotInTableException("User Not Found");
        }
        return user[0];
    }

    public async Task<int> getSleepSession(User user){
        var surveys = await getSurveys(user);
        if(surveys.Count() == 0){
            return 0;
        }
        //Returns the highest sleepSession Number
        var session = surveys.Max(survey => survey.sleepSession);
        return session;
    }

    public async Task<List<UserDailyQuizPacket>> getSurveys(User user){
        string CommandText = $"select * from dailyQuizes where userID = {user.userID};";
        DatabaseReader<UserDailyQuizPacket> dbReader = new DatabaseReader<UserDailyQuizPacket>(conn);
        List<UserDailyQuizPacket> surveys = await dbReader.databaseRead(CommandText, createUserDailyQuizPacketObjects);
        return surveys;
    }

    /// <summary>
    /// Stores a user's sleep survey in the database
    /// <summary>
    public async Task<int> addSurvey(UserDailyQuizPacket packet){
        DatabaseReader<UserDailyQuizPacket> dbReader = new DatabaseReader<UserDailyQuizPacket>(conn);
        await dbReader.databaseWrite(generateInsertUserDailyQuizSql, packet);
        return 0;
    }

    /// <summary>
    /// Store a user's heart data packet in the database
    /// <summary>
    public async Task<int> submitHeartPacket(HeartDataPacket packet){
        DatabaseReader<HeartDataPacket> dbReader = new DatabaseReader<HeartDataPacket>(conn);
        await dbReader.databaseWrite(generateInsertHeartDataSql, packet);
        return 0;
    }

    public async Task<int> submitSleepPacket(UserSleepStatePacket packet){
        DatabaseReader<UserSleepStatePacket> dbReader = new DatabaseReader<UserSleepStatePacket>(conn);
        await dbReader.databaseWrite(generateInsertUserSleepDataSql, packet);
        return 0;
    }

    /// <summary>
    /// Generates the SL command to insert a user into the database
    /// <summary>
    private int generateInsertUserSql(MySqlCommand command, UserDataPacket packet){
        command.CommandText = $"INSERT into users values({packet.userID},{packet.age},?sex,?fitbitID);";
        command.Parameters.AddWithValue("?sex", packet.sex);
        command.Parameters.AddWithValue("?fitbitID", null);
        return 0;
    }

    /// <summary>
    /// Generates the SQL command to insert a user's daily quize into the database
    /// <summary>
    private int generateInsertUserDailyQuizSql(MySqlCommand command, UserDailyQuizPacket packet){
        command.CommandText = $"INSERT INTO dailyQuizes values ({packet.userID}, {packet.sleepSession}, ?q1, ?q2, ?q3, ?q4, ?q5, ?q6, ?q7, ?sleepTime);";
        command.Parameters.AddWithValue("?q1", packet.q1);
        command.Parameters.AddWithValue("?q2", packet.q2);
        command.Parameters.AddWithValue("?q3", packet.q3);
        command.Parameters.AddWithValue("?q4", packet.q4);
        command.Parameters.AddWithValue("?q5", packet.q5);
        command.Parameters.AddWithValue("?q6", packet.q6);
        command.Parameters.AddWithValue("?q7", packet.q7);
        command.Parameters.AddWithValue("?sleepTime", packet.wakeTime);
        return 0;
    }

    /// <summary>
    /// Generates the SQL command to update the user's table to include a user's fitbit ID
    /// <summary>
    private int generateLinkFitbitAccountSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"UPDATE users SET fitbitID = ?fitbitID WHERE users.userID = ?userID;";
        command.Parameters.AddWithValue("?fitbitID", packet.fitbitID);
        command.Parameters.AddWithValue("?userID", packet.userID);
        return 0;
    }

    /// <summary>
    /// Generates the SQL command to insert a new set of fitbit credentials into the database
    /// <summary>
    private int generateInsertFitbitUserSql(MySqlCommand command, FitbitOnboardPacket packet){
        command.CommandText = $"INSERT INTO fitbitUsers values(?fitbitID,?accessToken,?refreshToken, NULL);";
        command.Parameters.AddWithValue("?fitbitID",packet.fitbitID);
        command.Parameters.AddWithValue("?accessToken",packet.accessToken);
        command.Parameters.AddWithValue("?refreshToken",packet.refreshToken);
        return 0;
    }

    /// <summary>
    /// Generates the SQL command to insert a user's nest credentials into the database
    /// <summary>
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

    /// <summary>
    /// Generates the SQL command to update a user's fitbit credentials in the database
    /// <summary>
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

    /// <summary>
    /// Generates the SQL command to update a user's nest credentials in the database
    /// <summary>
    private int generateRefreshNestUserSql(MySqlCommand command, NestRefreshAuthResponse packet){
        command.CommandText = $"UPDATE nestUsers SET accessToken = ?accessToken, expires = ?expires WHERE userID = ?userID;";
        command.Parameters.AddWithValue("?accessToken", packet.access_token);
        command.Parameters.AddWithValue("?userID", packet.user_id);
        var now = DateTime.Now;
        var offset = new TimeSpan(0,0,packet.expires_in);
        
        command.Parameters.AddWithValue("?expires", now + offset);
        return 0;
    }

    /// <summary>
    /// Generates the SQL command to insert a user's heart data into the database
    /// <summary>
    private int generateInsertHeartDataSql(MySqlCommand command, HeartDataPacket packet){
        command.CommandText = $"INSERT into heartdata (userID, fitbitID) VALUES ({packet.userID}, {packet.fitbitID})";
        return 0;
    }

    private int generateInsertUserSleepDataSql(MySqlCommand command, UserSleepStatePacket packet){
        //command.CommandText = $"INSERT into sleepData (userID, sleepSession, logTime, heartBeat, heartBeatLag, currentTemp, targetTemp) VALUES ({packet.userID}, {packet.sleepSession}, {packet.logTime}, {packet.heartBeat}, {packet.heartBeatLag}, {packet.currentTemp}, {packet.targetTemp})";
        command.CommandText = $"INSERT into sleepData values(?userID, ?sleepSession, ?logTime, ?heartBeat, ?heartBeatLag, ?currentTemp, ?targetTemp);";
        command.Parameters.AddWithValue("?userID", packet.userID);
        command.Parameters.AddWithValue("?sleepSession", packet.sleepSession);
        command.Parameters.AddWithValue("?logTime", packet.logTime.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("?heartBeat", packet.heartBeat);
        command.Parameters.AddWithValue("?heartBeatLag", packet.heartBeatLag);
        command.Parameters.AddWithValue("?currentTemp", packet.currentTemp);
        command.Parameters.AddWithValue("?targetTemp", packet.targetTemp);
        return 0;
    }

    /// <summary>
    /// Creates a list of user objects from a datatable
    /// <summary>
    /// <param name="dataTable">DataTable dataTable: The datatable containing data from the users, fitbitUsers, and nestUsers tables to convert to a list of user objects</param>
    /// <returns>List<User>: A list of user objects</returns>
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

    private List<UserDailyQuizPacket> createUserDailyQuizPacketObjects(DataTable dataTable){
        return (from DataRow row in dataTable.Rows
                    select new UserDailyQuizPacket{
                        userID = (int)row["userID"],
                        sleepSession = (int)row["sleepSession"],
                        q1 = (int)row["q1"],
                        q2 = (int)row["q2"],
                        q3 = (int)row["q3"],
                        q4 = (int)row["q4"],
                        q5 = (int)row["q5"],
                        q6 = (int)row["q6"],
                        q7 = (int)row["q7"],
                        wakeTime = ConvertFromDBVal<DateTime>(row["wakeTime"]),
                    }
        ).ToList();
    }

    /// <summary>
    /// Check to see if the connection is open, and if not, open it
    /// <summary>
    private void checkConnection(){
        if(conn.State != System.Data.ConnectionState.Open){
            conn.Open();
        }
    }

    /// <summary>
    /// Converts a database value to a nullable type
    /// <summary>
    public static T? ConvertFromDBVal<T>(object obj){
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