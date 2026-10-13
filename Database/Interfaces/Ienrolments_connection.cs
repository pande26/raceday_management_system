using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Ienrolments_connection
    {//start of interface

        List<Enrolments> get_my_enrolments(int participant_id);
        Enrolments? get_my_enrolment_for_event(int participant_id, int event_id);
        string create_enrolment(int participant_id, int event_id, int category_id, string? comments);
        string cancel_enrolment(int enrolment_id);
        string update_enrolment_status(int enrolment_id, string enr_status);
        string update_payment_status(int enrolment_id, string payment_status);
        Enrolments? get_enrolment_by_id(int enrolment_id);
        bool owns_event(int event_id, int user_id);
        bool event_has_started(int event_id);
        bool category_belongs_to_event(int category_id, int event_id);
        List<Enrolments> get_event_enrolments(int event_id);

    }//end of interface

}//end of namespace