using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Iresults_connection
    {//start of interface

        List<Models.Results> get_my_results(int participant_id);
        Models.Results? get_result_by_id(int result_id);
        List<Models.Results> get_leaderboard(int event_id);
        string record_result(int enrolment_id, int event_id, int participant_id, TimeSpan finish_time, int overall_position, int? category_position, int total_finishers, int? category_total, bool is_disqualified, string? disqualification_reason, int recorded_by);
        string update_result(int result_id, TimeSpan finish_time, int overall_position, int? category_position, int total_finishers, int? category_total, bool is_disqualified, string? disqualification_reason);
        string publish_result(int result_id);
        string delete_result(int result_id);
        bool result_exists_for_enrolment(int enrolment_id);
        int? get_participant_id_for_enrolment(int enrolment_id);
        bool enrolment_belongs_to_event(int enrolment_id, int event_id);
        bool owns_event(int event_id, int user_id);
        List<Models.Results> get_participant_results(int participant_id);

    }//end of interface

}//end of namespace