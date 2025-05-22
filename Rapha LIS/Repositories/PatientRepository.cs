
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using MVP_LEARNING.Repositories;
using Rapha_LIS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Document.NET;

namespace Rapha_LIS.Repositories
{
    public class PatientRepository : BaseRepository, IPatientControlRepository, IPatientResultRepository, IAnalyticsRepository, ITestListRepository
    {
        public PatientRepository(string ConnectionString)
        {
            this.connectionString = ConnectionString;
        }



        //Analytics

        public void UpdateExaminer(string barcodeID, string examinerName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "UPDATE Patients5 SET MedTech = @MedTech WHERE BarcodeID = @BarcodeID";
                    command.Parameters.AddWithValue("@MedTech", examinerName);
                    command.Parameters.AddWithValue("@BarcodeID", barcodeID);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Patient record updated. MedTech: {examinerName}");
                    }
                    else
                    {
                        MessageBox.Show("Failed to update patient record.");
                    }
                }
            }
        }

        public PatientModel? GetPatientByHRI(string id)
        {
            PatientModel? patientList = null;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Patients5 WHERE BarcodeID = @BarcodeID";
                    command.Parameters.Add("@BarcodeID", SqlDbType.NVarChar).Value = id;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            patientList = new PatientModel()
                            {

                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Age = Convert.ToInt32(reader["Age"]),
                                Sex = reader["Sex"].ToString(),
                                Physician = reader["Physician"].ToString(),
                                MedTech = reader["MedTech"].ToString(),
                                Test = reader["Test"].ToString(),
                                TestResult = reader["TestResult"].ToString(),
                                NormalValue = reader["NormalValue"].ToString(),
                                Leukocytes = reader["Leukocytes"].ToString(),
                                LeukocytesResult = reader["LeukocytesResult"].ToString(),
                                LeukocytesNormalValue = reader["LeukocytesNormalValue"].ToString(),
                                DateCreated = Convert.ToDateTime(reader["DateCreated"])
                            };
                        }

                    }

                }
            }
            MessageBox.Show(patientList != null ? $"User Found: {patientList.Name}" : "User not found in GetUserById!");

            return patientList;
        }

        public List<PatientModel> GetPatientHRI(string examinerName)
        {
            var patientHRI = new List<PatientModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Patients5 WHERE MedTech = @MedTech";
                command.Parameters.AddWithValue("@MedTech", examinerName);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var patientModel = new PatientModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Age = Convert.ToInt32(reader["Age"]),
                            Sex = reader["Sex"].ToString(),
                            Physician = reader["Physician"].ToString(),
                            MedTech = reader["MedTech"].ToString(),
                            Test = reader["Test"].ToString(),
                            TestResult = reader["TestResult"].ToString(),
                            NormalValue = reader["NormalValue"].ToString(),
                            Leukocytes = reader["Leukocytes"].ToString(),
                            LeukocytesResult = reader["LeukocytesResult"].ToString(),
                            LeukocytesNormalValue = reader["LeukocytesNormalValue"].ToString(),
                            DateCreated = Convert.ToDateTime(reader["DateCreated"]),
                        };

                        patientHRI.Add(patientModel); // FIX: Now actually adding to the list
                    }
                }
            }
            return patientHRI;
        }

        public void AddPatient(PatientModel patientModel)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO Patients5 (Name, Age, Sex, Physician, MedTech, Test, TestResult, NormalValue, " +
                                           "Leukocytes, LeukocytesResult, LeukocytesNormalValue, DateCreated) " +
                                          "VALUES (@Name, @Age, @Sex, @Physician, @MedTech, @Test, @TestResult, @NormalValue, " +
                                           "@Leukocytes, @LeukocytesResult, @LeukocytesNormalValue, @DateCreated)";

                    command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = patientModel.Name;
                    command.Parameters.Add("@Age", SqlDbType.Int).Value = patientModel.Age;
                    command.Parameters.Add("@Sex", SqlDbType.NVarChar).Value = patientModel.Sex;
                    command.Parameters.Add("@Physician", SqlDbType.NVarChar).Value = patientModel.Physician;
                    command.Parameters.Add("@MedTech", SqlDbType.NVarChar).Value = patientModel.MedTech;
                    command.Parameters.Add("@Test", SqlDbType.NVarChar).Value = patientModel.Test;
                    command.Parameters.Add("@TestResult", SqlDbType.NVarChar).Value = patientModel.TestResult;
                    command.Parameters.Add("@NormalValue", SqlDbType.NVarChar).Value = patientModel.NormalValue;
                    command.Parameters.Add("@Leukocytes", SqlDbType.NVarChar).Value = patientModel.Leukocytes;
                    command.Parameters.Add("@LeukocytesResult", SqlDbType.NVarChar).Value = patientModel.LeukocytesResult;
                    command.Parameters.Add("@LeukocytesNormalValue", SqlDbType.NVarChar).Value = patientModel.LeukocytesNormalValue;

                    command.ExecuteNonQuery();

                }
            }
        }

        public void EditPatient(PatientModel patient)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
                                        UPDATE Patients5
                                        SET Name = @Name,
                                            Age = @Age,
                                            Sex = @Sex,
                                            Physician = @Physician,
                                            MedTech = @MedTech,
                                            Test = @Test,
                                            TestResult = @TestResult,
                                            NormalValue = @NormalValue,
                                            Leukocytes = @Leukocytes,
                                            LeukocytesResult = @LeukocytesResult,
                                            LeukocytesNormalValue = @LeukocytesNormalValue,
                                            DateCreated = @DateCreated,
                                        WHERE Id = @Id";

                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = patient.Name;
                command.Parameters.Add("@Age", SqlDbType.Int).Value = patient.Age;
                command.Parameters.Add("@Sex", SqlDbType.NVarChar).Value = patient.Sex;
                command.Parameters.Add("@Physician", SqlDbType.NVarChar).Value = patient.Physician;
                command.Parameters.Add("@MedTech", SqlDbType.NVarChar).Value = patient.MedTech;
                command.Parameters.Add("@Test", SqlDbType.NVarChar).Value = patient.Test;
                command.Parameters.Add("@TestResult", SqlDbType.NVarChar).Value = patient.TestResult;
                command.Parameters.Add("@NormalValue", SqlDbType.NVarChar).Value = patient.NormalValue;
                command.Parameters.Add("@Leukocytes", SqlDbType.NVarChar).Value = patient.Leukocytes;
                command.Parameters.Add("@LeukocytesResult", SqlDbType.NVarChar).Value = patient.LeukocytesResult;
                command.Parameters.Add("@LeukocytesNormalValue", SqlDbType.NVarChar).Value = patient.LeukocytesNormalValue;
                command.Parameters.Add("@DateCreated", SqlDbType.DateTime).Value = patient.DateCreated;
                command.Parameters.Add("@Id", SqlDbType.Int).Value = patient.Id;

                command.ExecuteNonQuery();




            }
        }

        public List<FilteredPatientModel> GetFilteredName()
        {
            var list = new List<FilteredPatientModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Patients5 ORDER BY DateCreated DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(FilteredReadPatient(reader));
                    }
                }
            }
            return list;
        }

        public List<FilteredPatientModel> GetByFilteredName(string value)
        {
            var filteredPatientList = new List<FilteredPatientModel>();
            string userName = value;
            int Id = int.TryParse(value, out _) ? Convert.ToInt32(value) : 0;
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"    
                                        SELECT * FROM Patients5
                                        WHERE Id = @Id or Name LIKE @Name + '%'
                                        ORDER BY DateCreated DESC";
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = userName;
                command.Parameters.Add("@Id", SqlDbType.NVarChar).Value = Id;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        filteredPatientList.Add(FilteredReadPatient(reader));
                    }
                }
            }
            return filteredPatientList;
        }

        public List<PatientModel> GetAll()
        {
            var list = new List<PatientModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"Select *FROM Patients5 Order by DateCreated desc";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(ReadPatient(reader));
                    }
                }
            }
            return list;
        }

        private PatientModel ReadPatient(SqlDataReader reader) => new PatientModel
        {
            Id = reader.GetInt32("Id"),
            Name = reader.GetStringOrEmpty("Name"),
            Age = reader.GetNullableInt("Age"),
            Sex = reader.GetStringOrEmpty("Sex"),
            Physician = reader.GetStringOrEmpty("Physician"),
            MedTech = reader.GetStringOrEmpty("MedTech"),
            Test = reader.GetStringOrEmpty("Test"),
            TestResult = reader.GetStringOrEmpty("TestResult"),
            NormalValue = reader.GetStringOrEmpty("NormalValue"),
            Leukocytes = reader.GetStringOrEmpty("Leukocytes"),
            LeukocytesResult = reader.GetStringOrEmpty("LeukocytesResult"),
            LeukocytesNormalValue = reader.GetStringOrEmpty("LeukocytesNormalValue"),
            DateCreated = reader.GetDateTime("DateCreated")
        };

        private FilteredPatientModel FilteredReadPatient(SqlDataReader reader) => new FilteredPatientModel
        {
            Id = reader.GetInt32("Id"),
            Name = reader.GetStringOrEmpty("Name"),
            Age = reader.GetNullableInt("Age"),
            Sex = reader.GetStringOrEmpty("Sex"),
            Physician = reader.GetStringOrEmpty("Physician"),
            MedTech = reader.GetStringOrEmpty("MedTech"),
            Test = reader.GetStringOrEmpty("Test"),
            TestResult = reader.GetStringOrEmpty("TestResult"),
            NormalValue = reader.GetStringOrEmpty("NormalValue"),
            Leukocytes = reader.GetStringOrEmpty("Leukocytes"),
            LeukocytesResult = reader.GetStringOrEmpty("LeukocytesResult"),
            LeukocytesNormalValue = reader.GetStringOrEmpty("LeukocytesNormalValue"),
            DateCreated = reader.GetDateTime("DateCreated")
        };

        //Result

        public List<PatientModel> GetAllPatientResult()
        {
            var resultList = new List<PatientModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT * FROM Patients5  WHERE Result <> 'Pending'  ORDER BY DateCreated DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var patientModel = new PatientModel();


                        patientModel.Id = Convert.ToInt32(reader["Id"]);
                        patientModel.Name = reader["Name"].ToString();
                        patientModel.Age = Convert.ToInt32(reader["Age"]);
                        patientModel.Sex = reader["Sex"].ToString();
                        patientModel.Physician = reader["Physician"].ToString();
                        patientModel.MedTech = reader["MedTech"].ToString();
                        patientModel.Test = reader["Test"].ToString();
                        patientModel.TestResult = reader["TestResult"].ToString();
                        patientModel.NormalValue = reader["NormalValue"].ToString();
                        patientModel.Leukocytes = reader["Leukocytes"].ToString();
                        patientModel.LeukocytesResult = reader["LeukocytesResult"].ToString();
                        patientModel.LeukocytesNormalValue = reader["LeukocytesNormalValue"].ToString();
                        patientModel.DateCreated = Convert.ToDateTime(reader["DateCreated"]);
                        resultList.Add(patientModel);
                    }
                }
            }
            return resultList;
        }

        public List<FilteredPatientModel> GetResultFilteredName()
        {
            var filteredPatientList = new List<FilteredPatientModel>();
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM Patients5  WHERE TestResult <> 'Pending'  ORDER BY DateCreated DESC";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        filteredPatientList.Add(FilteredReadPatient(reader));
                    }
                }
            }
            return filteredPatientList;
        }

        public List<FilteredPatientModel> GetResultByFilteredName(string value)
        {
            var filteredPatientList = new List<FilteredPatientModel>();
            string userName = value;
            int Id = int.TryParse(value, out _) ? Convert.ToInt32(value) : 0;
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT * FROM Patients5 WHERE Name LIKE @Name + '%'
                                        AND TestResult <> 'Pending' ORDER BY DateCreated DESC";
                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = userName;
                command.Parameters.Add("@Id", SqlDbType.NVarChar).Value = Id;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var userModel = new FilteredPatientModel();
                        userModel.Id = Convert.ToInt32(reader["Id"]);
                        userModel.Name = reader["Name"].ToString();
                        userModel.Age = Convert.ToInt32(reader["Age"]);
                        userModel.Sex = reader["Sex"].ToString();
                        userModel.Physician = reader["Physician"].ToString();
                        userModel.MedTech = reader["MedTech"].ToString();
                        userModel.Test = reader["Test"].ToString();
                        userModel.TestResult = reader["TestResult"].ToString();
                        userModel.NormalValue = reader["NormalValue"].ToString();
                        userModel.Leukocytes = reader["Leukocytes"].ToString();
                        userModel.LeukocytesResult = reader["LeukocytesResult"].ToString();
                        userModel.LeukocytesNormalValue = reader["LeukocytesNormalValue"].ToString();
                        userModel.DateCreated = Convert.ToDateTime(reader["DateCreated"]);
                        filteredPatientList.Add(userModel);
                    }
                }
            }
            return filteredPatientList;
        }

        public void EditResult(PatientModel patientModel)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"
            UPDATE Patients5
            SET Result = @Result
            WHERE Id = @Id";
                command.Parameters.Add("@Id", SqlDbType.Int).Value = patientModel.Id;
                command.Parameters.Add("@Result", SqlDbType.NVarChar).Value = patientModel.TestResult;
                command.ExecuteNonQuery();
            }
        }

        public int InsertEmptyPatient()
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Patients5 
                (Name, Age, Sex, Physician, MedTech, Test, TestResult,
                 NormalValue, Leukocytes, LeukocytesResult, LeukocytesNormalValue, DateCreated)
                OUTPUT INSERTED.Id
                VALUES ('', NULL, '', '', '', '', '', '', '', '', '', GETDATE())", conn);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void SaveOrUpdatePatient(PatientModel patient)
        {
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM Patients5 WHERE Id = @Id)
                BEGIN
                    UPDATE Patients5 SET 
                        Name=@Name, Age=@Age, Sex=@Sex, Physician=@Physician,
                        MedTech=@MedTech, Test=@Test, TestResult=@TestResult,
                        NormalValue=@NormalValue, Leukocytes=@Leukocytes,
                        LeukocytesResult=@LeukocytesResult, LeukocytesNormalValue=@LeukocytesNormalValue,
                        DateCreated = GETDATE()
                    WHERE Id=@Id
                END
                ELSE
                BEGIN
                    INSERT INTO Patients5 
                    (Id, Name, Age, Sex, Physician, MedTech, Test, TestResult,
                     NormalValue, Leukocytes, LeukocytesResult, LeukocytesNormalValue, DateCreated)
                    VALUES
                    (@Id, @Name, @Age, @Sex, @Physician, @MedTech, @Test, @TestResult,
                     @NormalValue, @Leukocytes, @LeukocytesResult, @LeukocytesNormalValue, GETDATE())
                END", conn);
            conn.Open();

            cmd.Parameters.AddWithValue("@Id", patient.Id);
            cmd.Parameters.AddWithValue("@Name", patient.Name ?? "");
            cmd.Parameters.AddWithValue("@Age", (object?)patient.Age ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sex", patient.Sex ?? "");
            cmd.Parameters.AddWithValue("@Physician", patient.Physician ?? "");
            cmd.Parameters.AddWithValue("@MedTech", patient.MedTech ?? "");
            cmd.Parameters.AddWithValue("@Test", patient.Test ?? "");
            cmd.Parameters.AddWithValue("@TestResult", patient.TestResult ?? "");
            cmd.Parameters.AddWithValue("@NormalValue", patient.NormalValue ?? "");
            cmd.Parameters.AddWithValue("@Leukocytes", patient.Leukocytes ?? "");
            cmd.Parameters.AddWithValue("@LeukocytesResult", patient.LeukocytesResult ?? "");
            cmd.Parameters.AddWithValue("@LeukocytesNormalValue", patient.LeukocytesNormalValue ?? "");

            cmd.ExecuteNonQuery();
        }

        public void DeletePatient(List<int> ids)
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            foreach (var id in ids)
            {
                using var cmd = new SqlCommand("DELETE FROM Patients5 WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<TestModel> GetAllTests()
        {
            var list = new List<TestModel>();
            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand("SELECT Test, NormalValue FROM Tests", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new TestModel
                {
                    Test = reader.GetStringOrEmpty("Test"),
                    NormalValue = reader.GetNullableString("NormalValue")
                });
            }
            return list;
        }
    }



    static class SqlDataReaderExtensions
    {
        public static string GetStringOrEmpty(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? "" : reader[columnName].ToString();

        public static string? GetNullableString(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? null : reader[columnName].ToString();

        public static int? GetNullableInt(this SqlDataReader reader, string columnName)
            => reader[columnName] == DBNull.Value ? (int?)null : Convert.ToInt32(reader[columnName]);
    }



}

