using National4HSatrusLive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace National4HSatrusLive.Services
{
    public class UserService
    {
        public UserModel GetUserByCredentials(string email, string password)
        {
            UserModel user = new UserModel();
            if (email == "email@domain.com" || password == "password")
            {
                user = new UserModel()
                {
                    Id = "1",
                    Email = "email@domain.com",
                    Password = "password",
                    Name = "John"
                };
            }
            else if (email == "email@domain1.com" || password == "password1")
            {
                user = new UserModel()
                {
                    Id = "1",
                    Email = "email@domain1.com",
                    Password = "password1",
                    Name = "John1"
                };
            }
            else
            {
                return null;
            }
            if (null != user)
            {
                user.Password = string.Empty;
            }
            return user;
        }
    }
}