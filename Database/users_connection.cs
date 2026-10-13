using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class users_connection : Iusers_connection
    {//start of class

        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //method to fetch a user's full profile
        public Users? get_user_profile(int user_id)
        {//start of get_user_profile()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select user_id, email, password_hash, firstname, surname, role, 
                                            profile_picture_url, date_of_birth, phone_number, 
                                            created_at, updated_at, is_active 
                                     from users 
                                     where user_id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);

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

        }//end of get_user_profile()

        //method to fetch another user's public profile
        public Users? get_user_public_profile(int user_id)
        {//start of get_user_public_profile()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select user_id, firstname, surname, role, profile_picture_url 
                                     from users 
                                     where user_id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var user = new Users
                                {
                                    User_id = Convert.ToInt32(reader["user_id"]),
                                    Firstname = reader["firstname"].ToString() ?? "",
                                    Surname = reader["surname"].ToString() ?? "",
                                    Role = reader["role"].ToString() ?? "",
                                    Profile_picture_url = reader["profile_picture_url"] == DBNull.Value ? null : reader["profile_picture_url"].ToString()
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

        }//end of get_user_public_profile()

        //method to update a user's profile
        public string update_user_profile(int user_id, string firstname, string surname, string? phone_number, DateOnly? date_of_birth)
        {//start of update_user_profile()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update users 
                                     set firstname = @Firstname, 
                                         surname = @Surname, 
                                         phone_number = @PhoneNumber, 
                                         date_of_birth = @DateOfBirth, 
                                         updated_at = GETDATE() 
                                     where user_id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Firstname", firstname);
                        cmd.Parameters.AddWithValue("@Surname", surname);
                        cmd.Parameters.AddWithValue("@PhoneNumber",
                            string.IsNullOrWhiteSpace(phone_number) ? (object)DBNull.Value : phone_number);
                        cmd.Parameters.AddWithValue("@DateOfBirth",
                            date_of_birth.HasValue
                                ? date_of_birth.Value.ToDateTime(TimeOnly.MinValue)
                                : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Profile updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_user_profile()

        //method to update a user's profile picture URL
        public string update_profile_picture(int user_id, string picture_url)
        {//start of update_profile_picture()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update users 
                                     set profile_picture_url = @PictureUrl, 
                                         updated_at = GETDATE() 
                                     where user_id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@PictureUrl", picture_url);
                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Profile picture updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_profile_picture()

    }//end of class

}//end of namespace