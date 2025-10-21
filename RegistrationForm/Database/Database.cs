using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RegistrationForm.Database
{
    public static class Database
    {
        private static string connectionString = "Server=localhost;Database=REGISTRATIONDB;Trusted_Connection=True;";

        public static void InsertUser(User user)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("INSERT INTO Users (Email, Name, PasswordHash) VALUES (@e, @n, @p)", conn);

            cmd.Parameters.AddWithValue("@e", user.Email);
            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }

        public static User? GetUser(string email)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT Id, Email, Name, Password FROM Users WHERE Email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);
            using var reader = cmd.ExecuteReader();
            if (reader.Read()) 
            {
                return new User
                {
                    Id = reader.GetInt32(0),
                    Email = reader.GetString(1),
                    Name = reader.GetString(2),
                    PasswordHash = reader.GetString(3)
                };
            }
            return null;
        }

        public static void UpdateUser(User user) 
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            var cmd = new SqlCommand("UPDATE Users SET Name=@n, PasswordHash=@p WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteUser(User user) 
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM Users WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", user);
            cmd.ExecuteNonQuery();
        }

    }
}
