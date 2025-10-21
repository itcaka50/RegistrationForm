using Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationForm
{
    public class Router
    {
        private readonly AuthController auth = new AuthController();
        private readonly UserController user = new UserController();

        public string HandleRequest(string requestT)
        {
            if (string.IsNullOrEmpty(requestT))
            {
                return "Empty request";
            }

            string firstLine = requestT.Split('\n')[0];
            var parts = firstLine.Split(' ');

            if (parts.Length < 2)
            {
                return "Invalid request";
            }

            string method = parts[0];
            string path = parts[1];

            if (path == "/" || path == "/index.html") return File.ReadAllText("../../../Views/index.html");
            else if (path == "/register.html") return File.ReadAllText("../../../Views/register.html");
            else if (path == "/login.html") return File.ReadAllText("../../../Views/login.html");
            else if (path.StartsWith("/register")) return auth.Register();
            else if (path.StartsWith("/login")) return auth.Login();
            else if (path.StartsWith("/logout")) return auth.Logout();
            else if (path.StartsWith("/profile")) return File.ReadAllText("../../../Views/profile.html");
            else if (path.StartsWith("/change")) return user.Change();
            else return "<h1>404 Not Found</h1>";
        }
    }
}
