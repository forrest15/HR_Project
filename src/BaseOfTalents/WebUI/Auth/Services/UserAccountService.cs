﻿using System;
using System.Threading.Tasks;
using DAL.DTO;
using DAL.Services;
using WebUI.Auth.Infrastructure;
using WebUI.Globals.Validators;

namespace WebUI.Auth.Services
{
    /// <summary>
    /// Service for managing user and its session
    /// </summary>
    public class UserAccountService : IAccountService
    {
        UserService _userService;
        RoleService _roleService;

        public UserAccountService(UserService userService, RoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        /// <summary>
        /// Logs user out of application, clearing its session.
        /// </summary>
        /// <returns>True if action finished succesfully</returns>
        public bool LogOut(string token)
        {
            throw new NotImplementedException();
            try
            {

                //_authContainer.Delete(token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Changing password for authenticated user by his token
        /// </summary>
        /// <param name="token">User's token</param>
        /// <param name="oldPassword">Old password for check user identity</param>
        /// <param name="newPassword">New password will be set to user's data if old password is match </param>
        public void ChangePassword(string token, string oldPassword, string newPassword)
        {
            int id = PayloadDecoder.TryGetId(token);
            var user = _userService.Get(id);
            var validator = new PasswordValidator();
            var result = validator.Validate(oldPassword, user.Password);
            if (!result.IsValid)
            {
                throw new ArgumentException(result.Message);
            }
            user.Password = newPassword;
            _userService.Update(user);
        }

        /// <summary>
        /// Perfoms accessing to user of specified login and password
        /// </summary>
        /// <param name="login">Application user login</param>
        /// <param name="password">User password (hashed)</param>
        /// <returns>Corresponting user dto object</returns>
        /// <exception cref="ArgumentException">Is thrown, when there is no user with such a login and password</exception>
        public UserDTO Authentificate(string login, string password)
        {
            var user = _userService.Get(login);
            if (user == null && user.Password == password)
            {
                throw new ArgumentException("Wrong login or password");
                //TODO: Extract message to external source
                //TODO: new exception type
            }
            return user;
        }

        public bool CkeckAuthority(string login, string password)
        {
            try
            {
                Authentificate(login, password);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Perfoms accessing to user of specified login and password async
        /// </summary>
        /// <param name="login">Application user login</param>
        /// <param name="password">User password (hashed)</param>
        /// <returns>Corresponting user dto object</returns>
        /// <exception cref="ArgumentException">Is thrown, when there is no user with such a login and password</exception>
        public async Task<UserDTO> AuthentificateAsync(string login, string password)
        {
            var user = await _userService.GetAsync(login);
            if (user == null && user.Password == password)
            {
                throw new ArgumentException("Wrong login or password");
                //TODO: Extract message to external source
                //TODO: new exception type
            }
            return user;
        }

        public async Task<bool> CkeckAuthorityAsync(string login, string password)
        {
            try
            {
                await AuthentificateAsync(login, password);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}