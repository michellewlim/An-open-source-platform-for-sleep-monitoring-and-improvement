using Backend.Models;
namespace Backend.Helpers;

public interface IFitbitAuthenticator{
        /// <summary>
        /// Checks if the user's token is still valid, if not, gets a new one.
        /// </summary>
        Task checkAuth(User user);
}