using System.Data.SqlClient;
using raceday_management_system.Models;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Database
{//start of namespace
    public class enrolments_connection : Ienrolments_connection
    {//start of class

        //connection string to connect to the raceday_db
        private readonly string connection_string = @"Data source=(localdb)\api_raceday;Database=raceday_db;Trusted_Connection=True;TrustServerCertificate=True;";

        // helper method to map SqlDataReader row to an Enrolments model
        private Enrolments map_enrolment(SqlDataReader reader)
        {//start of map_enrolment()

            return new Enrolments
            {
                Enrolment_id = Convert.ToInt32(reader["enrolment_id"]),
                Participant_id = Convert.ToInt32(reader["participant_id"]),
                Event_id = Convert.ToInt32(reader["event_id"]),
                Category_id = Convert.ToInt32(reader["category_id"]),
                Enrolment_date = Convert.ToDateTime(reader["enrolment_date"]),
                Enr_status = reader["enr_status"].ToString() ?? "",
                Payment_status = reader["payment_status"] == DBNull.Value ? null : reader["payment_status"].ToString(),
                Payment_amount = reader["payment_amount"] == DBNull.Value ? null : Convert.ToDecimal(reader["payment_amount"]),
                Comments = reader["comments"] == DBNull.Value ? null : reader["comments"].ToString(),
                Updated_at = reader["updated_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["updated_at"])
            };

        }//end of map_enrolment()

        //method to get all enrolments for a participant
        public List<Enrolments> get_my_enrolments(int participant_id)
        {//start of get_my_enrolments()

            List<Enrolments> enrols = new List<Enrolments>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from enrolments where participant_id = @ParticipantId order by enrolment_date desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                enrols.Add(map_enrolment(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return enrols;

        }//end of get_my_enrolments()

        //method to check if a participant is enrolled in an event
        public Enrolments? get_my_enrolment_for_event(int participant_id, int event_id)
        {//start of get_my_enrolment_for_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"select * from enrolments 
                                     where participant_id = @ParticipantId and event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);
                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var enrol = map_enrolment(reader);
                                connect.Close();
                                return enrol;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

            return null;

        }//end of get_my_enrolment_for_event()

        //method to enrol a participant in an event
        public string create_enrolment(
            int participant_id,
            int event_id,
            int category_id,
            string? comments)
        {//start of create_enrolment()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"insert into enrolments 
                                     (participant_id, event_id, category_id, enrolment_date, 
                                      enr_status, payment_status, comments) 
                                     values 
                                     (@ParticipantId, @EventId, @CategoryId, GETDATE(), 
                                      'Pending', 'Pending', @Comments)";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@ParticipantId", participant_id);
                        cmd.Parameters.AddWithValue("@EventId", event_id);
                        cmd.Parameters.AddWithValue("@CategoryId", category_id);
                        cmd.Parameters.AddWithValue("@Comments",
                            string.IsNullOrWhiteSpace(comments) ? (object)DBNull.Value : comments);

                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Enrolment successful.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of create_enrolment()

        //method to cancel an enrolment (set status to Withdrawn)
        public string cancel_enrolment(int enrolment_id)
        {//start of cancel_enrolment()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update enrolments 
                                     set enr_status = 'Withdrawn', updated_at = GETDATE() 
                                     where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Enrolment cancelled successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of cancel_enrolment()

        //method to update enrolment status (organiser)
        public string update_enrolment_status(int enrolment_id, string enr_status)
        {//start of update_enrolment_status()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update enrolments 
                                     set enr_status = @EnrStatus, updated_at = GETDATE() 
                                     where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrStatus", enr_status);
                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Enrolment status updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_enrolment_status()

        //method to update payment status
        public string update_payment_status(int enrolment_id, string payment_status)
        {//start of update_payment_status()

            string message = "";

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = @"update enrolments 
                                     set payment_status = @PaymentStatus, updated_at = GETDATE() 
                                     where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@PaymentStatus", payment_status);
                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);
                        cmd.ExecuteNonQuery();

                    }//end of using

                    connect.Close();

                    message = "Payment status updated successfully.";

                }//end of using

            }//end of try
            catch (Exception error)
            {//start of catch

                message = "Database Error: " + error.Message;

            }//end of catch

            return message;

        }//end of update_payment_status()

        //method to fetch a single enrolment by ID
        public Enrolments? get_enrolment_by_id(int enrolment_id)
        {//start of get_enrolment_by_id()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from enrolments where enrolment_id = @EnrolmentId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EnrolmentId", enrolment_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            if (reader.Read())
                            {//start of if

                                var enrol = map_enrolment(reader);
                                connect.Close();
                                return enrol;

                            }//end of if

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { return null; }//end of catch

            return null;

        }//end of get_enrolment_by_id()

        //method to check if the logged-in user owns the event
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

        //method to check if an event has already started
        public bool event_has_started(int event_id)
        {//start of event_has_started()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select event_date from events where event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        var result = cmd.ExecuteScalar();

                        connect.Close();

                        if (result == null || result == DBNull.Value)
                        {//start of if

                            return false;

                        }//end of if

                        DateTime event_date = Convert.ToDateTime(result);
                        return event_date <= DateTime.UtcNow;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of event_has_started()

        //method to check if a category belongs to an event
        public bool category_belongs_to_event(int category_id, int event_id)
        {//start of category_belongs_to_event()

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select count(*) from categories where category_id = @CategoryId and event_id = @EventId";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@CategoryId", category_id);
                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        connect.Close();

                        return count > 0;

                    }//end of using

                }//end of using

            }//end of try
            catch (Exception) { return false; }//end of catch

        }//end of category_belongs_to_event()

        //method to get all enrolments for an event
        public List<Enrolments> get_event_enrolments(int event_id)
        {//start of get_event_enrolments()

            List<Enrolments> enrols = new List<Enrolments>();

            try
            {//start of try

                using (SqlConnection connect = new SqlConnection(connection_string))
                {//start of using

                    connect.Open();

                    string query = "select * from enrolments where event_id = @EventId order by enrolment_date desc";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {//start of using

                        cmd.Parameters.AddWithValue("@EventId", event_id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {//start of using

                            while (reader.Read())
                            {//start of while

                                enrols.Add(map_enrolment(reader));

                            }//end of while

                        }//end of using

                    }//end of using

                    connect.Close();

                }//end of using

            }//end of try
            catch (Exception) { }//end of catch

            return enrols;

        }//end of get_event_enrolments()

    }//end of class

}//end of namespace