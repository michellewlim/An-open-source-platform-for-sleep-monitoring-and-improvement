using Backend.Models;
using Backend.Models.Fitbit;
namespace Backend.Controllers;

public interface IDatabaseController{

    Task<int> addUser(UserDataPacket packet);

    Task<int> linkFitbitToUser(FitbitOnboardPacket packet);

    Task<int> addSurvey(UserDailyQuizPacket packet);

    Task<List<User>> getUsers();

    Task<User> getUser(int userID);
};