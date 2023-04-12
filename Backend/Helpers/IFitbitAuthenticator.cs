using Backend.Models;
namespace Backend.Helpers;

public interface IFitbitAuthenticator{
        Task checkAuth(User user);

}