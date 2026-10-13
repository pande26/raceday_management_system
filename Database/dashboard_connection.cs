using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class dashboard_connection : Idashboard_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        //method to count events owned by an organiser
        public int count_organiser_events(int organiser_id)
        {//start of count_organiser_events()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from events where organiser_id = @OrganiserId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@OrganiserId", organiser_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_organiser_events()

        //method to count all enrolments across an organiser's events
        public int count_organiser_enrolments(int organiser_id)
        {//start of count_organiser_enrolments()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select count(*) from enrolments 
                                     where event_id in 
                                        (select event_id from events where organiser_id = @OrganiserId)";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@OrganiserId", organiser_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_organiser_enrolments()

        //method to get all events owned by an organiser
        public List<Events> get_organiser_events(int organiser_id)
        {//start of get_organiser_events()

            List<Events> events = new List<Events>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from events where organiser_id = @OrganiserId order by event_date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@OrganiserId", organiser_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                events.Add(new Events
                                {
                                    Event_id = Convert.ToInt32(reader["event_id"]),
                                    Organiser_id = Convert.ToInt32(reader["organiser_id"]),
                                    Event_name = reader["event_name"].ToString() ?? "",
                                    Event_description = reader["event_description"].ToString() ?? "",
                                    Event_date = Convert.ToDateTime(reader["event_date"]),
                                    Location = reader["location"].ToString() ?? "",
                                    Distance = Convert.ToDecimal(reader["distance"]),
                                    Event_type = reader["event_type"].ToString() ?? "",
                                    Banner_image_url = reader["banner_image_url"] == DBNull.Value ? null : reader["banner_image_url"].ToString(),
                                    Max_participants = reader["max_participants"] == DBNull.Value ? null : Convert.ToInt32(reader["max_participants"]),
                                    Entry_fee = reader["entry_fee"] == DBNull.Value ? null : Convert.ToDecimal(reader["entry_fee"]),
                                    Event_status = reader["event_status"].ToString() ?? "",
                                    Created_at = Convert.ToDateTime(reader["created_at"]),
                                    Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return events;

        }//end of get_organiser_events()

        //method to count enrolments for a specific event
        public int count_event_enrolments(int event_id)
        {//start of count_event_enrolments()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from enrolments where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_event_enrolments()

        //method to count enrolments for a participant
        public int count_participant_enrolments(int participant_id)
        {//start of count_participant_enrolments()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from enrolments where participant_id = @ParticipantId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_participant_enrolments()

        //method to get the events a participant is enrolled in (upcoming)
        public List<Events> get_participant_upcoming_events(int participant_id)
        {//start of get_participant_upcoming_events()

            List<Events> events = new List<Events>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select e.* from events e 
                                     inner join enrolments en on e.event_id = en.event_id 
                                     where en.participant_id = @ParticipantId 
                                     and e.event_date > GETDATE() 
                                     order by e.event_date";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                events.Add(new Events
                                {
                                    Event_id = Convert.ToInt32(reader["event_id"]),
                                    Organiser_id = Convert.ToInt32(reader["organiser_id"]),
                                    Event_name = reader["event_name"].ToString() ?? "",
                                    Event_description = reader["event_description"].ToString() ?? "",
                                    Event_date = Convert.ToDateTime(reader["event_date"]),
                                    Location = reader["location"].ToString() ?? "",
                                    Distance = Convert.ToDecimal(reader["distance"]),
                                    Event_type = reader["event_type"].ToString() ?? "",
                                    Banner_image_url = reader["banner_image_url"] == DBNull.Value ? null : reader["banner_image_url"].ToString(),
                                    Max_participants = reader["max_participants"] == DBNull.Value ? null : Convert.ToInt32(reader["max_participants"]),
                                    Entry_fee = reader["entry_fee"] == DBNull.Value ? null : Convert.ToDecimal(reader["entry_fee"]),
                                    Event_status = reader["event_status"].ToString() ?? "",
                                    Created_at = Convert.ToDateTime(reader["created_at"]),
                                    Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return events;

        }//end of get_participant_upcoming_events()

        //method to count results for a participant
        public int count_participant_results(int participant_id)
        {//start of count_participant_results()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from results where participant_id = @ParticipantId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return 0; }//end of catch

        }//end of count_participant_results()

        //method to get recent results for a participant (top 5)
        public List<Models.Results> get_recent_participant_results(int participant_id, int limit)
        {//start of get_recent_participant_results()

            List<Models.Results> results = new List<Models.Results>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select top (@Limit) * from results 
                                     where participant_id = @ParticipantId 
                                     order by recorded_at desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@Limit", limit);
                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                results.Add(new Models.Results
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
                                });

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return results;

        }//end of get_recent_participant_results()

    }//end of class

}//end of namespace