using RegistrationForm.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RegistrationForm.Database;
using static System.Collections.Specialized.BitVector32;
using RegistrationForm;

namespace Controllers
{
    public class AuthController
    {
        public string Register(string email, string name, string password)
        {
            if (!Validation.IsValidEmail(email))
            {
                return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Invalid email</h3>";   
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(password)) 
            {
                return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Name and password cannot be empty</h3>";
            }

            var existing = Database.GetUser(email);
            if (existing != null) 
            {
                return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Email already registered</h3>";
            }

            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = Validation.HashPassword(password)
            };

            Database.InsertUser(user);
            var html = File.ReadAllText(GetViewPath("profile.html"));
            html = html.Replace("{{USER_NAME}}", user.Name);
            html = html.Replace("{{USER_EMAIL}}", user.Email);

            return html;
        }

        public string Login(string email, string password,out string sessionId)
        {

            sessionId = null;

            var user = Database.GetUser(email);
            if (user == null)
            {
                return "<link rel=\"stylesheet\" href=\"style.css\"><h3>User not found!</h3><a href='/login.html'>Try again</a>";
            }

            var hash = Validation.HashPassword(password);
            if (user.PasswordHash != hash)
            {
                return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Wrong password!</h3><a href='/login.html'>Try again</a>";
            }
            sessionId = SessionManager.CreateSession(email);

            var html = File.ReadAllText(GetViewPath("profile.html"));
            html = html.Replace("{{USER_NAME}}", user.Name);
            html = html.Replace("{{USER_EMAIL}}", user.Email);

            return html;
        }

        public string Logout(string sessionId)
        {
            SessionManager.RemoveSession(sessionId);
            return File.ReadAllText(GetViewPath("index.html"));
        }

        private string GetViewPath(string filename)
        {
            string baseDir = AppContext.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, @"../../../Views"));
            return Path.Combine(projectDir, filename);
        }   
    }
}
