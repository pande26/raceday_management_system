using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class results_connection : Iresults_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //helper to map a SqlDataReader row to a Results model
        private Models.Results map_result(SqlDataReader reader)
        {//start of map_result()

            return new Models.Results
            {
                Result_id = Convert.ToInt32(reader["result_id"]),
                Enrolment_id = Convert.ToInt32(reader["enrolment_id"]),
                Event_id = Convert.ToInt32(reader["event_id"]),
                Participant_id = Convert.ToInt32(reader["participant_id"]),
                Finish_time = (TimeSpan)reader["finish_time"],
                Overall_position = Convert.ToInt32(reader["overall_position"]),
                Category_position = reader["category_position"] == DBNull.Value ? null : Convert.ToInt32(reader["category_position"]),
                Total_finishers = Convert.ToInt32(reader["total_finishers"]),
                Category_total = reader["category_total"] == DBNull.Value ? null : Convert.ToInt32(reader["category_total"]),
                Is_disqualified = Convert.ToBoolean(reader["is_disqualified"]),
                Disqualification_reason = reader["disqualification_reason"] == DBNull.Value ? null : reader["disqualification_reason"].ToString(),
                Recorded_by = Convert.ToInt32(reader["recorded_by"]),
                Recorded_at = Convert.ToDateTime(reader["recorded_at"]),
                Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
            };

        }//end of map_result()

        //method to get all results for a participant
        public List<Models.Results> get_my_results(int participant_id)
        {//start of get_my_results()

            List<Models.Results> results = new List<Models.Results>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from results where participant_id = @ParticipantId order by recorded_at desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                results.Add(map_result(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return results;

        }//end of get_my_results()

        //method to get a result by ID
        public Models.Results? get_result_by_id(int result_id)
        {//start of get_result_by_id()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from results where result_id = @ResultId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ResultId", result_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var result = map_result(reader);
                                connect.Close();
                                return result;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

            return null;

        }//end of get_result_by_id()

        //method to get the leaderboard for an event
        public List<Models.Results> get_leaderboard(int event_id)
        {//start of get_leaderboard()

            List<Models.Results> results = new List<Models.Results>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from results where event_id = @EventId order by overall_position";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                results.Add(map_result(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return results;

        }//end of get_leaderboard()

        //method to record a result
        public string record_result(int enrolment_id, int event_id, int participant_id, TimeSpan finish_time, int overall_position, int? category_position, int total_finishers, int? category_total, bool is_disqualified, string? disqualification_reason, int recorded_by)
        {//start of record_result()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"insert into results 
                                     (enrolment_id, event_id, participant_id, finish_time, overall_position, 
                                      category_position, total_finishers, category_total, is_disqualified, 
                                      disqualification_reason, recorded_by, recorded_at) 
                                     values 
                                     (@EnrolmentId, @EventId, @ParticipantId, @FinishTime, @OverallPosition, 
                                      @CategoryPosition, @TotalFinishers, @CategoryTotal, @IsDisqualified, 
                                      @DisqualificationReason, @RecordedBy, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);
                        cmd.Parameters.AddWithValue("@EventId", event_id);
                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);
                        cmd.Parameters.AddWithValue("@FinishTime", finish_time);
                        cmd.Parameters.AddWithValue("@OverallPosition", overall_position);
                        cmd.Parameters.AddWithValue("@CategoryPosition", category_position.HasValue ? (object)category_position.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalFinishers", total_finishers);
                        cmd.Parameters.AddWithValue("@CategoryTotal", category_total.HasValue ? (object)category_total.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDisqualified", is_disqualified);
                        cmd.Parameters.AddWithValue("@DisqualificationReason",
                            string.IsNullOrWhiteSpace(disqualification_reason) ? (object)DBNull.Value : disqualification_reason);
                        cmd.Parameters.AddWithValue("@RecordedBy", recorded_by);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Result recorded successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of record_result()

        //method to update an existing result
        public string update_result(int result_id, TimeSpan finish_time, int overall_position, int? category_position, int total_finishers, int? category_total, bool is_disqualified, string? disqualification_reason)
        {//start of update_result()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update results 
                                     set finish_time = @FinishTime, 
                                         overall_position = @OverallPosition, 
                                         category_position = @CategoryPosition, 
                                         total_finishers = @TotalFinishers, 
                                         category_total = @CategoryTotal, 
                                         is_disqualified = @IsDisqualified, 
                                         disqualification_reason = @DisqualificationReason, 
                                         updated_at = GETDATE() 
                                     where result_id = @ResultId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@FinishTime", finish_time);
                        cmd.Parameters.AddWithValue("@OverallPosition", overall_position);
                        cmd.Parameters.AddWithValue("@CategoryPosition", category_position.HasValue ? (object)category_position.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalFinishers", total_finishers);
                        cmd.Parameters.AddWithValue("@CategoryTotal", category_total.HasValue ? (object)category_total.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsDisqualified", is_disqualified);
                        cmd.Parameters.AddWithValue("@DisqualificationReason",
                            string.IsNullOrWhiteSpace(disqualification_reason) ? (object)DBNull.Value : disqualification_reason);
                        cmd.Parameters.AddWithValue("@ResultId", result_id);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Result updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_result()

        //method to "publish" a result 
        public string publish_result(int result_id)
        {//start of publish_result()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "update results set updated_at = GETDATE() where result_id = @ResultId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ResultId", result_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Result published successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of publish_result()

        //method to delete a result
        public string delete_result(int result_id)
        {//start of delete_result()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "delete from results where result_id = @ResultId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ResultId", result_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Result deleted successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of delete_result()

        //method to check if a result already exists for an enrolment
        public bool result_exists_for_enrolment(int enrolment_id)
        {//start of result_exists_for_enrolment()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from results where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count > 0;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of result_exists_for_enrolment()

        //method to get the participant_id for an enrolment
        public int? get_participant_id_for_enrolment(int enrolment_id)
        {//start of get_participant_id_for_enrolment()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select participant_id from enrolments where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);

                        var result = cmd.ExecuteScalar();

                        connect.Close();

                        if (result == null || result == DBNull.Value)
                        {//start of if

                            return null;

                        }//end of if

                        return Convert.ToInt32(result);

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

        }//end of get_participant_id_for_enrolment()

        //method to check if an enrolment belongs to an event
        public bool enrolment_belongs_to_event(int enrolment_id, int event_id)
        {//start of enrolment_belongs_to_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from enrolments where enrolment_id = @EnrolmentId and event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);
                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count > 0;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of enrolment_belongs_to_event()

        //method to check if a user owns an event
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

        //method to get all results for a participant
        public List<Models.Results> get_participant_results(int participant_id)
        {//start of get_participant_results()

            List<Models.Results> results = new List<Models.Results>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from results where participant_id = @ParticipantId order by recorded_at desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                results.Add(map_result(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return results;

        }//end of get_participant_results()

    }//end of class

}//end of namespace