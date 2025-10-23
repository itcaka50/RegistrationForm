using NUnit.Framework;
using Controllers;
using RegistrationForm.Utils;
using System.IO;
using System;

namespace RegistrationForm.Tests
{
    [TestFixture]
    public class AuthTests
    {
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _authController = new AuthController();
           
            CreateTestHtmlFiles();
        }

        private void CreateTestHtmlFiles()
        {
            var viewsDir = "../../../../RegistrationForm/Views/";
            Directory.CreateDirectory(viewsDir);

            File.WriteAllText(viewsDir + "profile.html",
                "<html><body>Profile: {{USER_NAME}} - {{USER_EMAIL}}</body></html>");

            File.WriteAllText(viewsDir + "index.html",
                "<html><body>Home Page</body></html>");
        }

        [Test]
        public void Register_InvalidEmail_ReturnsErrorMessage()
        {
            // Arrange
            string email = "invalid-email";
            string name = "Test User";
            string password = "password123";

            // Act
            var result = _authController.Register(email, name, password);

            // Assert
            Assert.That(result, Contains.Substring("Invalid email"));
        }

        [Test]
        public void Register_EmptyName_ReturnsErrorMessage()
        {
            // Arrange
            string email = "test@test.com";
            string name = "";
            string password = "password123";

            // Act
            var result = _authController.Register(email, name, password);

            // Assert
            Assert.That(result, Contains.Substring("cannot be empty"));
        }

        [Test]
        public void Register_EmptyPassword_ReturnsErrorMessage()
        {
            // Arrange
            string email = "test@test.com";
            string name = "Test User";
            string password = "";

            // Act
            var result = _authController.Register(email, name, password);

            // Assert
            Assert.That(result, Contains.Substring("cannot be empty"));
        }

        [Test]
        public void Register_ValidData_ReturnsProfilePage()
        {
            // Arrange
            string email = "newuser@test.com";
            string name = "New User";
            string password = "password123";

            // Act
            var result = _authController.Register(email, name, password);

            // Assert
            Assert.That(result, Contains.Substring("Profile:"));
            Assert.That(result, Contains.Substring(name));
            Assert.That(result, Contains.Substring(email));
        }

        [Test]
        public void Login_UserNotFound_ReturnsErrorMessage()
        {
            // Arrange
            string email = "nonexistent@test.com";
            string password = "password123";

            // Act
            var result = _authController.Login(email, password, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("User not found"));
            Assert.That(sessionId, Is.Null);
        }

        [Test]
        public void Login_WrongPassword_ReturnsErrorMessage()
        {
            // Arrange
            string email = "test@test.com";
            string password = "wrongpassword";

            // Act
            var result = _authController.Login(email, password, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Wrong password"));
            Assert.That(sessionId, Is.Null);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsProfilePageAndSession()
        {
            // Arrange
            string email = "test@test.com";
            string password = "correctpassword";

            // Act
            var result = _authController.Login(email, password, out string sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Profile:"));
            Assert.That(sessionId, Is.Not.Null);
            Assert.That(sessionId, Is.Not.Empty);
        }

        [Test]
        public void Logout_ValidSession_ReturnsHomePage()
        {
            // Arrange
            string sessionId = "test-session";

            // Act
            var result = _authController.Logout(sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Home Page"));
        }

        [Test]
        public void Logout_AnySession_ReturnsHomePage()
        {
            // Arrange
            string sessionId = "any-session-id";

            // Act
            var result = _authController.Logout(sessionId);

            // Assert
            Assert.That(result, Contains.Substring("Home Page"));
        }
    }

    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void IsValidEmail_ValidEmails_ReturnsTrue()
        {
            // Arrange
            string[] validEmails = {
                "test@test.com",
                "user.name@domain.com",
                "user123@test.org",
                "test@sub.domain.com"
            };

            foreach (var email in validEmails)
            {
                // Act & Assert
                Assert.That(Validation.IsValidEmail(email), Is.True, $"Failed for: {email}");
            }
        }

        [Test]
        public void IsValidEmail_InvalidEmails_ReturnsFalse()
        {
            // Arrange
            string[] invalidEmails = {
                "invalid-email",
                "missing-at.com",
                "@nodomain.com",
                "spaces in@email.com",
                ""
            };

            foreach (var email in invalidEmails)
            {
                // Act & Assert
                Assert.That(Validation.IsValidEmail(email), Is.False, $"Failed for: {email}");
            }
        }

        [Test]
        public void HashPassword_SamePassword_SameHash()
        {
            // Arrange
            string password = "testpassword123";

            // Act
            var hash1 = Validation.HashPassword(password);
            var hash2 = Validation.HashPassword(password);

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2));
        }

        [Test]
        public void HashPassword_DifferentPasswords_DifferentHashes()
        {
            // Arrange
            string password1 = "password1";
            string password2 = "password2";

            // Act
            var hash1 = Validation.HashPassword(password1);
            var hash2 = Validation.HashPassword(password2);

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2));
        }

        [Test]
        public void HashPassword_EmptyPassword_ReturnsEmptyString()
        {
            // Act
            var result = Validation.HashPassword("");

            // Assert
            Assert.That(result, Is.EqualTo(""));
        }
    }
}
