using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.SqlClient;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RegistrationForm.Database
{
    public static class Database
    {
        //private static string connectionString = "Server=.\\DEFAULTSQLSERVER;Database=REGISTRATIONDB;Trusted_Connection=True;";
        private static readonly string connectionString = "Server=localhost;Port=3306;User=root;Password=11122111;Database=registrationdb;";

        public static void InitializeDatabase()
        {
            using (var conn = new MySqlConnection("Server=localhost;User=root;Password=11122111;"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS registrationdb;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            using (var conn = new MySqlConnection("Server=localhost;User=root;Password=11122111;Database=registrationdb;"))
            {
                conn.Open();
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Email VARCHAR(255) NOT NULL UNIQUE,
                Name VARCHAR(255) NOT NULL,
                PasswordHash VARCHAR(255) NOT NULL
            );";
                using (var cmd = new MySqlCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Database opened.");
        }

        public static void InsertUser(User user)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("INSERT INTO Users (Email, Name, PasswordHash) VALUES (@e, @n, @p)", conn);

            cmd.Parameters.AddWithValue("@e", user.Email);
            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }

        public static User? GetUser(string email)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT Id, Email, Name, PasswordHash FROM Users WHERE Email=@e", conn);
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
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            var cmd = new MySqlCommand("UPDATE Users SET Name=@n, PasswordHash=@p WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@n", user.Name);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteUser(User user) 
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM Users WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.ExecuteNonQuery();
        }

    }
}
