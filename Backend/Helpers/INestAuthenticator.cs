using Backend.Models;
namespace Backend.Helpers;

public interface INestAuthenticator{
    ///<Summary>
    ///Checks if the user has a valid Nest token, if not, refreshes the token
    ///</Summary>
    public Task checkAuth(User user);
}