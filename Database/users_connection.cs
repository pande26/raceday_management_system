using System.Data.SqlClient;

namespace raceday_management_system.Database
{//start of namespace
    public class users_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //method to fetch a user's full profile by user_id
        public Dictionary<string, object>? get_user_profile(int user_id)
        {//start of get_user_profile()

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = @"select user_id, email, firstname, surname, role, 
                                            profile_picture_url, date_of_birth, phone_number, 
                                            created_at, updated_at, is_active 
                                     from users 
                                     where user_id = @UserId";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        //run reader
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {//start of for

                                    row[reader.GetName(i)] = reader.IsDBNull(i)
                                        ? null!
                                        : reader.GetValue(i);

                                }//end of for

                                connect.Close();
                                return row;

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
        public Dictionary<string, object>? get_user_public_profile(int user_id)
        {//start of get_user_public_profile()

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = @"select user_id, firstname, surname, role, profile_picture_url 
                                     from users 
                                     where user_id = @UserId";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        //run reader
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {//start of for

                                    row[reader.GetName(i)] = reader.IsDBNull(i)
                                        ? null!
                                        : reader.GetValue(i);

                                }//end of for

                                connect.Close();
                                return row;

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
        public string update_user_profile( int user_id, string firstname, string surname, string? phone_number, DateOnly? date_of_birth)
        {//start of update_user_profile()

            //temp message
            string message = "";

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = @"update users 
                                     set firstname = @Firstname, 
                                         surname = @Surname, 
                                         phone_number = @PhoneNumber, 
                                         date_of_birth = @DateOfBirth, 
                                         updated_at = GETDATE() 
                                     where user_id = @UserId";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Firstname", firstname);
                        cmd.Parameters.AddWithValue("@Surname", surname);
                        cmd.Parameters.AddWithValue("@PhoneNumber",
                            string.IsNullOrWhiteSpace(phone_number)
                                ? (object)DBNull.Value
                                : phone_number);
                        cmd.Parameters.AddWithValue("@DateOfBirth",
                            date_of_birth.HasValue
                                ? date_of_birth.Value.ToDateTime(TimeOnly.MinValue)
                                : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        //run non-query
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

            //temp message
            string message = "";

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = @"update users 
                                     set profile_picture_url = @PictureUrl, 
                                         updated_at = GETDATE() 
                                     where user_id = @UserId";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@PictureUrl", picture_url);
                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        //run non-query
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