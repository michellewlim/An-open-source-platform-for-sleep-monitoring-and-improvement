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

    //Submit data to the database
    public async Task<int> addUser(UserDataPacket packet){
        this.checkConnection();

        MySqlCommand myCommand = conn.CreateCommand();
        MySqlTransaction myTrans;
        myTrans = conn.BeginTransaction();
        myCommand.Connection = conn;
        myCommand.Transaction = myTrans;

        try
        {
            myCommand.CommandText = $"insert into users (Id) VALUES ({packet.userID});";
            myCommand.ExecuteNonQuery();
            myTrans.Commit();
        }
        catch(Exception e)
        {
            myTrans.Rollback();
            throw(e);
        }
        return 0;
    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        
        this.checkConnection();

        MySqlCommand myCommand = conn.CreateCommand();

        List<User> users = new List<User>();

        MySqlTransaction myTrans;
        myTrans = conn.BeginTransaction();
        myCommand.Connection = conn;
        myCommand.Transaction = myTrans;

        myCommand.CommandText = $"select Id from users";
        using(var response = myCommand.ExecuteReader()){
            while (response.Read())
            {
                var Id = response.GetInt32(0);
                users.Add(new User{
                    id=Id
                });
            }
        response.Close();
        };
        myTrans.Commit();
        return users;
    }

    private void checkConnection(){
        if(conn.State != System.Data.ConnectionState.Open){
            conn.Open();
        }
    }

}