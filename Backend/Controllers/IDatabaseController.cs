using Backend.Models;
using Backend.Models.Fitbit;
using Backend.Models.Nest;
namespace Backend.Controllers;

public interface IDatabaseController{

    ///<summary>
    ///Add a user to the database
    ///</summary>
    Task<int> addUser(UserDataPacket packet);

    /// <summary>
    /// Link a user's fitbit account to their user account in the database
    /// </summary>
    Task<int> linkFitbitToUser(FitbitOnboardPacket packet);

    /// <summary>
    /// Link a user's nest account to their user account in the database
    /// </summary>
    Task<int> linkNestToUser(NestOnboardPacket packet);

    /// <summary>
    /// update a user's fitbit credentials in the database
    /// </summary>
    Task<int> refreshFitbitUserAuth(FitbitRefreshAuthResponse packet);

    /// <summary>
    /// update a user's nest credentials in the database
    /// </summary>
    Task<int> refreshNestUserAuth(NestRefreshAuthResponse packet);

    /// <summary>
    /// Stores a user's sleep survey in the database
    /// <summary>
    Task<int> addSurvey(UserDailyQuizPacket packet);

    /// <summary>
    /// Get a list of all users from the database
    /// </summary>
    Task<List<User>> getUsers();

    /// <summary>
    /// Retrieves a specific user from the database
    /// </summary>
    Task<User> getUser(int userID);

    /// <summary>
    /// Returns a user's most recent completed sleep Session
    /// </summary>
    Task<int> getSleepSession(User user);

    /// <summary>
    /// Submit a user's sleep state data to the database
    /// </summary>
    Task<int> submitSleepPacket(UserSleepStatePacket packet);
};