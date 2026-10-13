using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Icategories_connection
    {//start of interface

        Categories? get_category_by_id(int category_id);
        string create_category(int event_id, string category_name, string? cat_description, int? min_age, int? max_age, decimal? min_distance, decimal? max_distance);
        string update_category(int category_id, string category_name, string? cat_description, int? min_age, int? max_age, decimal? min_distance, decimal? max_distance);
        string delete_category(int category_id);
        int count_category_enrolments(int category_id);
        bool owns_category_event(int category_id, int user_id);
        bool owns_event(int event_id, int user_id);

    }//end of interface

}//end of namespace