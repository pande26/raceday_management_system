using System.Data.SqlClient;
using raceday_management_system.Models;

namespace raceday_management_system.Database
{//start of namespace
    public class categories_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //helper method to map SqlDataReader row to a Categories model
        private Categories map_category(SqlDataReader reader)
        {//start of map_category()

            return new Categories
            {
                Category_id = Convert.ToInt32(reader["category_id"]),
                Event_id = Convert.ToInt32(reader["event_id"]),
                Category_name = reader["category_name"].ToString() ?? "",
                Cat_description = reader["cat_description"] == DBNull.Value ? null : reader["cat_description"].ToString(),
                Min_age = reader["min_age"] == DBNull.Value ? null : Convert.ToInt32(reader["min_age"]),
                Max_age = reader["max_age"] == DBNull.Value ? null : Convert.ToInt32(reader["max_age"]),
                Min_distance = reader["min_distance"] == DBNull.Value ? null : Convert.ToDecimal(reader["min_distance"]),
                Max_distance = reader["max_distance"] == DBNull.Value ? null : Convert.ToDecimal(reader["max_distance"]),
                Created_at = Convert.ToDateTime(reader["created_at"])
            };

        }//end of map_category()

        //method to get a category by ID
        public Categories? get_category_by_id(int category_id)
        {//start of get_category_by_id()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from categories where category_id = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryId", category_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var cat = map_category(reader);
                                connect.Close();
                                return cat;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

            return null;

        }//end of get_category_by_id()

        //method to create a new category
        public string create_category(int event_id, string category_name, string? cat_description, int? min_age, int? max_age, decimal? min_distance, decimal? max_distance)
        {//start of create_category()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"insert into categories 
                                     (event_id, category_name, cat_description, min_age, max_age, 
                                      min_distance, max_distance, created_at) 
                                     values 
                                     (@EventId, @CategoryName, @CatDescription, @MinAge, @MaxAge, 
                                      @MinDistance, @MaxDistance, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);
                        cmd.Parameters.AddWithValue("@CategoryName", category_name);
                        cmd.Parameters.AddWithValue("@CatDescription",
                            string.IsNullOrWhiteSpace(cat_description) ? (object)DBNull.Value : cat_description);
                        cmd.Parameters.AddWithValue("@MinAge", min_age.HasValue ? (object)min_age.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaxAge", max_age.HasValue ? (object)max_age.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MinDistance", min_distance.HasValue ? (object)min_distance.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaxDistance", max_distance.HasValue ? (object)max_distance.Value : DBNull.Value);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Category created successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of create_category()

        //method to update an existing category
        public string update_category(int category_id, string category_name, string? cat_description, int? min_age, int? max_age, decimal? min_distance, decimal? max_distance)
        {//start of update_category()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update categories 
                                     set category_name = @CategoryName, 
                                         cat_description = @CatDescription, 
                                         min_age = @MinAge, 
                                         max_age = @MaxAge, 
                                         min_distance = @MinDistance, 
                                         max_distance = @MaxDistance 
                                     where category_id = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryName", category_name);
                        cmd.Parameters.AddWithValue("@CatDescription",
                            string.IsNullOrWhiteSpace(cat_description) ? (object)DBNull.Value : cat_description);
                        cmd.Parameters.AddWithValue("@MinAge", min_age.HasValue ? (object)min_age.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaxAge", max_age.HasValue ? (object)max_age.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MinDistance", min_distance.HasValue ? (object)min_distance.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaxDistance", max_distance.HasValue ? (object)max_distance.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CategoryId", category_id);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Category updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_category()

        //method to delete a category
        public string delete_category(int category_id)
        {//start of delete_category()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "delete from categories where category_id = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryId", category_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Category deleted successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of delete_category()

        //method to count enrolments for a category (to prevent deletion)
        public int count_category_enrolments(int category_id)
        {//start of count_category_enrolments()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from enrolments where category_id = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryId", category_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_category_enrolments()

        //method to check if a user owns the event a category belongs to
        public bool owns_category_event(int category_id, int user_id)
        {//start of owns_category_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select e.organiser_id 
                                     from categories c 
                                     join events e on c.event_id = e.event_id 
                                     where c.category_id = @CategoryId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryId", category_id);

                        var result = cmd.ExecuteScalar();

                        connect.Close();

                        if (result == null || result == DBNull.Value)
                        {//start of if

                            return false;

                        }//end of if

                        return Convert.ToInt32(result) == user_id;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of owns_category_event()

        //method to check if a user owns an event by event_id
        public bool owns_event(int event_id, int user_id)
        {//start of owns_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select organiser_id from events where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        var result = cmd.ExecuteScalar();

                        connect.Close();

                        if (result == null || result == DBNull.Value)
                        {//start of if

                            return false;

                        }//end of if

                        return Convert.ToInt32(result) == user_id;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of owns_event()

    }//end of class

}//end of namespace