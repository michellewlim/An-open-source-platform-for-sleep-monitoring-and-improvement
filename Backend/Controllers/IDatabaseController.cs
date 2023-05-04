using Backend.Models;
using Backend.Models.Fitbit;
using Backend.Models.Nest;
namespace Backend.Controllers;

public interface IDatabaseController{

    Task<int> addUser(UserDataPacket packet);

    Task<int> linkFitbitToUser(FitbitOnboardPacket packet);

    Task<int> linkNestToUser(NestOnboardPacket packet);

    Task<int> refreshFitbitUserAuth(FitbitRefreshAuthResponse packet);

    Task<int> refreshNestUserAuth(NestRefreshAuthResponse packet);

    Task<int> addSurvey(UserDailyQuizPacket packet);

    Task<List<User>> getUsers();

    Task<User> getUser(int userID);
};