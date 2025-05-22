
using Microsoft.Data.SqlClient;
using MVP_LEARNING.Repositories;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rapha_LIS.Repositories
{
    public class UserRepository : BaseRepository, IUserControlRepository, ISigninRepository
    {
        public UserRepository(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }

        public void DeleteUser(int Id)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "DELETE FROM Users2 WHERE Id=@Id";
                command.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                command.ExecuteNonQuery();
            }
        }



        public List<UserModel> GetAll()
        {
            var userList = new List<UserModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Users2 ORDER BY DateCreated DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userModel = new UserModel();

                        userModel.Id = Convert.ToInt32(reader["Id"]);
                        userModel.Name = reader["Name"].ToString();
                        userModel.Age = reader["Age"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Age"]);
                        userModel.Sex = reader["Sex"].ToString();
                        userModel.Username = reader["Username"].ToString();
                        userModel.Password = reader["Password"].ToString();
                        userModel.DateCreated = Convert.ToDateTime(reader["DateCreated"]);
                        userList.Add(userModel);
                    }
                }
            }
            return userList;
        }

        public List<FilteredUserModel> GetFilteredName()
        {
            var filteredUserList = new List<FilteredUserModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Users2 ORDER BY DateCreated DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userModel = new FilteredUserModel();

                        userModel.Id = Convert.ToInt32(reader["Id"]);
                        userModel.Name = reader["Name"].ToString();
                        userModel.Age = reader["Age"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Age"]);
                        userModel.Sex = reader["Sex"].ToString();
                        userModel.DateCreated = Convert.ToDateTime(reader["DateCreated"]);
                        filteredUserList.Add(userModel);
                    }
                }
            }
            return filteredUserList;
        }

        public List<FilteredUserModel> GetByFilteredName(string value)
        {
            var filteredUserList = new List<FilteredUserModel>();
            string userName = value;
            int Id = int.TryParse(value, out _) ? Convert.ToInt32(value) : 0;
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"    
                                        SELECT * FROM Users2
                                        WHERE Id = @Id or Name LIKE @Name + '%'
                                        ORDER BY DateCreated DESC";
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = userName;
                command.Parameters.Add("@Id", SqlDbType.NVarChar).Value = Id;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userModel = new FilteredUserModel();
                        userModel.Id = Convert.ToInt32(reader["Id"]);
                        userModel.Name = reader["Name"].ToString();
                        userModel.Age = Convert.ToInt32(reader["Age"]);
                        userModel.Sex = reader["Sex"].ToString();
                        filteredUserList.Add(userModel);
                    }
                }
            }
            return filteredUserList;
        }

        // Validate user login
        public string? ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Name, Password FROM Users2 WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string? storedPassword = reader["Password"].ToString().Trim();
                            string? fullName = reader["Name"].ToString();

                            if (storedPassword == password.Trim()) // Verify password match
                            {
                                return fullName; // Return full name on success
                            }
                        }
                    }
                }
            }
            return null; // Return null if login fails
        }

        public int InsertEmptyUser()
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
            INSERT INTO Users2 
            (Name, Age, Sex, Username, Password)
            OUTPUT INSERTED.Id
            VALUES ('', NULL, '', '', '')", conn);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void SaveOrUpdateUser(UserModel user)
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
        IF EXISTS (SELECT 1 FROM Users2 WHERE Id = @Id)
        BEGIN
            UPDATE Users2 SET 
                Name=@Name, Age=@Age, Sex=@Sex, Username=@Username, Password=@Password, DateCreated=GETDATE()
            WHERE Id=@Id
        END
        ELSE
        BEGIN
            INSERT INTO Users2 (Id, Name, Age, Sex, Username, Password, DateCreated)
            VALUES (@Id, @Name, @Age, @Sex, @Username, @Password, GETDATE())
        END", conn);

            conn.Open();

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name ?? "");
            cmd.Parameters.AddWithValue("@Age", (object?)user.Age ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sex", user.Sex ?? "");
            cmd.Parameters.AddWithValue("@Username", user.Username ?? "");
            cmd.Parameters.AddWithValue("@Password", user.Password ?? "");

            cmd.ExecuteNonQuery();
        }
    }
}
