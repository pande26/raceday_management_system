using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Idashboard_connection
    {//start of interface

        int count_organiser_events(int organiser_id);
        int count_organiser_enrolments(int organiser_id);
        List<Events> get_organiser_events(int organiser_id);
        int count_event_enrolments(int event_id);
        int count_participant_enrolments(int participant_id);
        List<Events> get_participant_upcoming_events(int participant_id);
        int count_participant_results(int participant_id);
        List<Models.Results> get_recent_participant_results(int participant_id, int limit);

    }//end of interface

}//end of namespace