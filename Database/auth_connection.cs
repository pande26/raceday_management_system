using System.Data.SqlClient;
using raceday_management_system.Services;

namespace raceday_management_system.Database
{//start of namespace
    public class auth_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        // method to store a new user in the database
        public string store_user_info( string email, string password_hash, string firstname, string surname, string role, DateOnly? date_of_birth, string? phone_number)
        {//start of store_user_info()

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

                    //query to insert the user's info (parameterised to prevent SQL injection)
                    string query = @"insert into users 
                                     (email, password_hash, firstname, surname, role, date_of_birth, phone_number, created_at, is_active) 
                                     values 
                                     (@Email, @PasswordHash, @Firstname, @Surname, @Role, @DateOfBirth, @PhoneNumber, GETDATE(), 1);";

                    //creating an instance to run the query using SqlCommand
                    using (SqlCommand run_query = new SqlCommand(query, connect))
                    {//start of using

                        //add the parameters safely
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

                        //run non-query
                        run_query.ExecuteNonQuery();

                    }//end of using

                    //message
                    message = "User registered successfully.";

                    //then close the connection
                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                //catch error message
                message = "Database Error: " + error.Message;

            }//end of catch

            //return message
            return message;

        }//end of store_user_info()

        //method to check if an email already exists
        public bool email_exists(string email)
        {//start of email_exists()

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = "select count(*) from users where email = @Email";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Email", email);

                        //run scalar
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        //close the connection
                        connect.Close();

                        //return true if at least one row
                        return count > 0;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                //if error, assume no match
                return false;

            }//end of catch

        }//end of email_exists()

        // method to fetch a user by email (for login)
        // returns a dictionary of column -> value, or null if not found
        public Dictionary<string, object>? get_user_by_email(string email)
        {//start of get_user_by_email()

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = @"select user_id, email, password_hash, firstname, surname, role, is_active 
                                     from users 
                                     where email = @Email";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Email", email);

                        //run reader
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            //if a row exists, read it
                            if (reader.Read())
                            {//start of if

                                //build a dictionary of column -> value
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {//start of for

                                    row[reader.GetName(i)] = reader.IsDBNull(i)
                                        ? null!
                                        : reader.GetValue(i);

                                }//end of for

                                //close the connection
                                connect.Close();

                                return row;

                            }//end of if

                        }//end of using

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                //if error, return null
                return null;

            }//end of catch

            //if nothing found, return null
            return null;

        }//end of get_user_by_email()

        // method to update the last login timestamp
        public void update_last_login(int user_id)
        {//start of update_last_login()

            //try and catch
            try
            {//start of try

                //using the "using" method to prevent memory leaks
                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    //open the connection
                    connect.Open();

                    //query
                    string query = "update users set updated_at = GETDATE() where user_id = @UserId";

                    //creating an instance to run the query
                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@UserId", user_id);

                        //run non-query
                        cmd.ExecuteNonQuery();

                    }//end of using

                    //close the connection
                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception)
            {//start of catch

                //silent fail — not critical

            }//end of catch

        }//end of update_last_login()

    }//end of class

}//end of namespace