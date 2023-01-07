using Backend.Models;

namespace Backend.Controllers;

public interface IDatabaseController{

    Task<int> addUser(UserDataPacket packet);

    Task<int> addSurvey(UserDailyQuizPacket packet);

    Task<List<User>> getUsers();
};