using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationForm.Utils
{
    public class SessionManager
    {
        private static Dictionary<string, Session> sessions = new Dictionary<string, Session>();

        public static string CreateSession(string email)
        {
            var sessionId = Guid.NewGuid().ToString();
            sessions[sessionId] = new Session
            {
                Email = email,
                CreatedAt = DateTime.Now
            };
            return sessionId;
        }

        public static Session GetSession(string sessionId) 
        {
            if (string.IsNullOrEmpty(sessionId)) return null;

            if (sessions.ContainsKey(sessionId))
            {
                var session = sessions[sessionId];

                if ((DateTime.Now - session.CreatedAt).TotalMinutes < 15)
                {
                    return session;
                }

                else
                {
                    sessions.Remove(sessionId);
                }    
            }
            return null;
        }

        public static void RemoveSession(string sessionId) 
        {
            if (sessions.ContainsKey(sessionId))
            {
                sessions.Remove(sessionId);
            }
        }

        public static bool isAuthenticated(string sessionId)
        {
            return GetSession(sessionId) != null;
        }
    }

    public class Session
    {
        public string Email {  get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
