using RegistrationForm.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RegistrationForm.Database;

namespace Controllers
{
    public class AuthController
    {
        public string Register(string email, string name, string password)
        {
            if (!Validation.IsValidEmail(email))
            {
                return "<h2>Invalid email</h2>";   
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(password)) 
            {
                return "<h2>Name and password cannot be empty</h2>";
            }

            var existing = Database.GetUser(email);
            if (existing != null) 
            {
                return "<h2>Email already registered";
            }


            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = Validation.HashPassword(password)
            };

            Database.InsertUser(user);
            return "<h2>Registration successful!</h2>";
        }

        public string Login(string email, string password)
        {
            return "<h2>Login successful (placeholder)</h2>";
        }

        public string Logout()
        {
            return "<h2>Logged out successfully</h2>";
        }
    }
}
