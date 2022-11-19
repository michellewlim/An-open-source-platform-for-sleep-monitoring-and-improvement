using Backend.Models;

namespace Backend.Controllers;

public class DatabaseController : IDatabaseController{

    public DatabaseController(){
        
    }

    //Submit data to the database
    public async Task<int> addData(UserDataPacket packet){
        
        return 0;
    }

    //Get a list of all users from the database
    public async Task<List<User>> getUsers(){
        
        List<User> users = new List<User>();

        User testUser = new User{
            id = 1
        };

        users.Append(testUser);

        return users;
    }

}