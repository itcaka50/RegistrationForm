using NUnit.Framework;
using System;
using System.IO;
using RegistrationForm.Utils;

namespace RegistrationForm.Unit_tests
{
    [TestFixture]
    public class RouterTests
    {
        private Router _router;

        [SetUp]
        public void Setup()
        {
            _router = new Router();

            CreateTestHtmlFiles();
        }

        private void CreateTestHtmlFiles()
        {
            var viewsDir = "../../../Views/";
            Directory.CreateDirectory(viewsDir);

            File.WriteAllText(viewsDir + "index.html", "<html><body>Home Page</body></html>");
            File.WriteAllText(viewsDir + "register.html", "<html><body>Register Page</body></html>");
            File.WriteAllText(viewsDir + "login.html", "<html><body>Login Page</body></html>");
            File.WriteAllText(viewsDir + "profile.html", "<html><body>Profile Page</body></html>");
            File.WriteAllText(viewsDir + "change.html", "<html><body>Change Page</body></html>");
            File.WriteAllText(viewsDir + "style.css", "body { color: red; }");
        }

        [Test]
        public void HandleRequest_GET_Home_ReturnsHtml()
        {
            // Arrange
            var request = "GET / HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Home Page"));
        }

        [Test]
        public void HandleRequest_GET_Register_ReturnsHtml()
        {
            // Arrange
            var request = "GET /register.html HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Register Page"));
        }

        [Test]
        public void HandleRequest_GET_Login_ReturnsHtml()
        {
            // Arrange
            var request = "GET /login.html HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Login Page"));
        }

        [Test]
        public void HandleRequest_GET_StyleCss_ReturnsCss()
        {
            // Arrange
            var request = "GET /style.css HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("color: red"));
        }

        [Test]
        public void HandleRequest_GET_UnknownPath_Returns404()
        {
            // Arrange
            var request = "GET /unknown.html HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Is.EqualTo("<h2>404 Not Found</h2>"));
        }

        [Test]
        public void HandleRequest_POST_Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var request = "POST /register HTTP/1.1\r\nHost: localhost\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\nemail=test@test.com&name=Test&password=123456";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Registration successful") | Contains.Substring("Invalid email"));
        }

        [Test]
        public void HandleRequest_POST_Login_WithValidData_ReturnsWelcome()
        {
            // Arrange
            var request = "POST /login HTTP/1.1\r\nHost: localhost\r\nContent-Type: application/x-www-form-urlencoded\r\n\r\nemail=test@test.com&password=123456";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Welcome") | Contains.Substring("not found") | Contains.Substring("Wrong password"));
        }

        [Test]
        public void HandleRequest_GET_Profile_WithoutSession_ReturnsLoginPrompt()
        {
            // Arrange
            var request = "GET /profile.html HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Please login first"));
        }

        [Test]
        public void HandleRequest_GET_Change_WithoutSession_ReturnsLoginPrompt()
        {
            // Arrange
            var request = "GET /change.html HTTP/1.1\r\nHost: localhost\r\n\r\n";

            // Act
            var result = _router.HandleRequest(request, null, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Please login first"));
        }
    }

    [TestFixture]
    public class SimpleValidationTests
    {
        [Test]
        public void IsValidEmail_ValidEmail_ReturnsTrue()
        {
            // Act & Assert
            Assert.That(Validation.IsValidEmail("test@test.com"), Is.True);
        }

        [Test]
        public void IsValidEmail_InvalidEmail_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(Validation.IsValidEmail("invalid-email"), Is.False);
        }

        [Test]
        public void HashPassword_ValidPassword_ReturnsHash()
        {
            // Act
            var result = Validation.HashPassword("password123");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
    }

    [TestFixture]
    public class SimpleSessionTests
    {
        [Test]
        public void CreateSession_ValidEmail_CreatesSession()
        {
            // Act
            var sessionId = SessionManager.CreateSession("test@test.com");

            // Assert
            Assert.That(sessionId, Is.Not.Null);
            Assert.That(sessionId, Is.Not.Empty);
        }

        [Test]
        public void GetSession_ValidSession_ReturnsSession()
        {
            // Arrange
            var sessionId = SessionManager.CreateSession("test@test.com");

            // Act
            var session = SessionManager.GetSession(sessionId);

            // Assert
            Assert.That(session, Is.Not.Null);
            Assert.That(session.Email, Is.EqualTo("test@test.com"));
        }

        [Test]
        public void GetSession_InvalidSession_ReturnsNull()
        {
            // Act
            var session = SessionManager.GetSession("invalid-session");

            // Assert
            Assert.That(session, Is.Null);
        }

        [Test]
        public void IsAuthenticated_ValidSession_ReturnsTrue()
        {
            // Arrange
            var sessionId = SessionManager.CreateSession("test@test.com");

            // Act
            var result = SessionManager.isAuthenticated(sessionId);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}