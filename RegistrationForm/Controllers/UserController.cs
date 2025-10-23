using RegistrationForm.Database;
using RegistrationForm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Controllers
{
    public class UserController
    {
        public string Change(string email, string newName, string newPassword)
        {
            var user = Database.GetUser(email);
            if (user == null) return "<link rel=\"stylesheet\" href=\"style.css\"><h3>User not found!</h3>";

            if (!string.IsNullOrWhiteSpace(newName)) user.Name = newName;

            if (!string.IsNullOrWhiteSpace(newPassword)) user.PasswordHash = Validation.HashPassword(newPassword);

            Database.UpdateUser(user);
            var html = File.ReadAllText("../../../Views/profile.html");
            html = html.Replace("{{USER_NAME}}", user.Name);
            html = html.Replace("{{USER_EMAIL}}", user.Email);

            return html;
        }

        public string Delete(string email)
        {
            var user = Database.GetUser(email);
            if (user == null) return "<link rel=\"stylesheet\" href=\"style.css\"><h3>User not found!</h3>";

            Database.DeleteUser(user);
            return File.ReadAllText("../../../Views/index.html");
        }
    }
}

