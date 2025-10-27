using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using Controllers;
using RegistrationForm.Utils;

namespace RegistrationForm
{
    public class Router
    {
        private readonly AuthController authController = new AuthController();
        private readonly UserController userController = new UserController();

        public string HandleRequest(string request, string sessionId, out string newSessionId)
        {
            newSessionId = null;

            var lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var requestLine = lines[0].Split(' ');
            var method = requestLine[0];
            var path = requestLine[1];

            if (path.Contains("?")) path = path.Substring(0, path.IndexOf("?"));

            if (method == "GET")
            {
                if (path == "/" || path == "/index.html")
                {
                    //return File.ReadAllText("../../../Views/index.html");
                    return File.ReadAllText(GetViewPath("index.html"));
                }

                if (path == "/register.html")
                {
                    //return File.ReadAllText("../../../Views/register.html");
                    return File.ReadAllText(GetViewPath("register.html"));
                }

                if (path == "/login.html")
                {
                    //return File.ReadAllText("../../../Views/login.html");
                    return File.ReadAllText(GetViewPath("login.html"));
                }

                if (path == "/style.css")
                {
                    //return File.ReadAllText("../../../Views/style.css");
                    return File.ReadAllText(GetViewPath("style.css"));
                }

                if (path == "/captcha.png")
                {

                }

                if (path == "/profile.html")
                {
                    var session = SessionManager.GetSession(sessionId);
                    if (session == null)
                    {
                        return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Please login first!</h3><a href='/login.html'>Login</a>";
                    }
                    var user = Database.Database.GetUser(session.Email);
                    //var html = File.ReadAllText("../../../Views/profile.html");
                    var html = File.ReadAllText(GetViewPath("profile.html"));
                    html = html.Replace("{{USER_NAME}}", user.Name);
                    html = html.Replace("{{USER_EMAIL}}", user.Email);

                    return html;
                }

                if (path == "/change.html")
                {
                    var session = SessionManager.GetSession(sessionId);
                    if (session == null)
                    {
                        return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Please login first!</h3><a href='/login.html'>Login</a>";
                    }
                    //return File.ReadAllText("../../../Views/change.html");
                    return File.ReadAllText(GetViewPath("change.html"));
                }

                if (path == "/logout")
                {
                    authController.Logout(sessionId);
                    return "<h2><link rel=\"stylesheet\" href=\"style.css\">Logged out successfully!</h2><a href='/index.html'>Home</a>";
                }
            }

            if (method == "POST")
            {

                var body = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i]) && i + 1 < lines.Length)
                    {
                        body = lines[i + 1];
                        break;
                    }
                }

                Console.WriteLine($"POST body: {body}");
                Console.WriteLine($"Full request: {request}");


                var formData = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(body))
                {
                    var pairs = body.Split('&');
                    foreach (var pair in pairs)
                    {
                        var kv = pair.Split('=');
                        if (kv.Length == 2)
                            formData[kv[0]] = HttpUtility.UrlDecode(kv[1]);
                    }
                }

                if (path == "/register")
                {

                    var enteredCaptcha = formData.GetValueOrDefault("captcha", "");
                    if (enteredCaptcha != CaptchaStorage.CurrentCode)
                    {
                        return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Invalid CAPTCHA code!</h3><a href='/register.html'>Try again</a>";
                    }

                    return authController.Register(
                        formData.GetValueOrDefault("email", ""),
                        formData.GetValueOrDefault("name", ""),
                        formData.GetValueOrDefault("password", "")
                    );
                }

                if (path == "/login")
                {
                    Console.WriteLine("Login route hit");
                    Console.WriteLine($"New session ID: {newSessionId}");
                    return authController.Login(
                        formData.GetValueOrDefault("email", ""),
                        formData.GetValueOrDefault("password", ""),
                        out newSessionId
                    );


                }

                if (path == "/change")
                {
                    var session = SessionManager.GetSession(sessionId);
                    if (session == null)
                    {
                        return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Please, login!</h3><a href='/login.html'>Login</a>";
                    }

                    return userController.Change(
                        session.Email,
                        formData.GetValueOrDefault("newName", ""),
                        formData.GetValueOrDefault("newPassword", "")
                    );
                }

                if (path == "/delete")
                {
                    var session = SessionManager.GetSession(sessionId);
                    if (session == null)
                    {
                        return "<link rel=\"stylesheet\" href=\"style.css\"><h3>Please, login!</h3><a href='/login.html'>Login</a>";
                    }

                    var result = userController.Delete(session.Email);
                    SessionManager.RemoveSession(sessionId);
                    return result;
                }
            }




            return "<h2>404 Not Found</h2>";
        }
    
        private string GetViewPath(string filename)
        {
            string baseDir = AppContext.BaseDirectory;
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, @"../../../Views"));
            return Path.Combine(projectDir, filename);
        }   
    }
}