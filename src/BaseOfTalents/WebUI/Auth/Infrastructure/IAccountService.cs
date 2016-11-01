using System.Threading.Tasks;
using DAL.DTO;

namespace WebUI.Infrastructure.Auth
{
    /// <summary>
    /// Service of controlling user actions, such as login (signin), registration and logout
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Performs a logout - the action opposite to login
        /// </summary>
        /// <returns>True if action finished successfully. Else false.</returns>
        bool LogOut(string token);

        UserDTO GetUser(string token);

        UserDTO Authentificate(string login, string password);
        bool CkeckAuthority(string login, string password);

        Task<bool> CkeckAuthorityAsync(string login, string password);
        Task<UserDTO> AuthentificateAsync(string login, string password);
        void ChangePassword(string token, string oldPassword, string newPassword);
        void RecoverAccount(string loginOrEmail);
    }
}
