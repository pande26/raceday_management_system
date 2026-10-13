using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class auth_connection : Iauth_connection
    {//start of class

        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //method to store a new user
        public string store_user_info(string email, string password_hash, string firstname, string surname, string role, DateOnly? date_of_birth, string? phone_number)
        {//start of store_user_info()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"insert into users 
                                     (email, password_hash, firstname, surname, role, date_of_birth, phone_number, created_at, is_active) 
                                     values 
                                     (@Email, @PasswordHash, @Firstname, @Surname, @Role, @DateOfBirth, @PhoneNumber, GETDATE(), 1);";

                    using (SqlCommand run_query = new SqlCommand(query, connect))
                    {//start of using

                        run_query.Parameters.AddWithValue("@Email", email);
                        run_query.Parameters.AddWithValue("@PasswordHash", password_hash);
                        run_query.Parameters.AddWithValue("@Firstname", firstname);
                        run_query.Parameters.AddWithValue("@Surname", surname);
                        run_query.Parameters.AddWithValue("@Role", role);
                        run_query.Parameters.AddWithValue("@DateOfBirth",
                            date_of_birth.HasValue
                                ? date_of_birth.Value.ToDateTime(TimeOnly.MinValue)
                                : (object)DBNull.Value);
                        run_query.Parameters.AddWithValue("@PhoneNumber",
                            string.IsNullOrWhiteSpace(phone_number)
                                ? (object)DBNull.Value
                                : phone_number);

                        run_query.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "User registered successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of store_user_info()

        //method to check if an email exists
        public bool email_exists(string email)
        {//start of email_exists()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from users where email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Email", email);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count > 0;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                return false;

            }//end of catch

        }//end of email_exists()

        //method to fetch a user by email
        public Users? get_user_by_email(string email)
        {//start of get_user_by_email()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select user_id, email, password_hash, firstname, surname, role, 
                                            profile_picture_url, date_of_birth, phone_number, 
                                            created_at, updated_at, is_active 
                                     from users 
                                     where email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var user = new Users
                                {
                                    User_id = Convert.ToInt32(reader["user_id"]),
                                    Email = reader["email"].ToString() ?? "",
                                    Password_hash = reader["password_hash"].ToString() ?? "",
                                    Firstname = reader["firstname"].ToString() ?? "",
                                    Surname = reader["surname"].ToString() ?? "",
                                    Role = reader["role"].ToString() ?? "",
                                    Profile_picture_url = reader["profile_picture_url"] == DBNull.Value ? null : reader["profile_picture_url"].ToString(),
                                    Date_of_birth = reader["date_of_birth"] == DBNull.Value
                                        ? null
                                        : DateOnly.FromDateTime(Convert.ToDateTime(reader["date_of_birth"])),
                                    Phone_number = reader["phone_number"] == DBNull.Value ? null : reader["phone_number"].ToString(),
                                    Created_at = Convert.ToDateTime(reader["created_at"]),
                                    Updated_at = reader["updated_at"] == DBNull.Value
                                        ? null
                                        : Convert.ToDateTime(reader["updated_at"]),
                                    Is_active = Convert.ToBoolean(reader["is_active"])
                                };

                                connect.Close();
                                return user;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                return null;

            }//end of catch

            return null;

        }//end of get_user_by_email()

        //method to update the last login timestamp
        public void update_last_login(int user_id)
        {//start of update_last_login()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "update users set updated_at = GETDATE() where user_id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

            }//end of catch

        }//end of update_last_login()

    }//end of class

}//end of namespace