using Backend.Models;

namespace Backend.Controllers;

public interface IDatabaseController{

    Task<int> addData(UserDataPacket packet);

    Task<List<User>> getUsers();
};