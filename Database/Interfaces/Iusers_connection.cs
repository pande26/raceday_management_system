using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Iusers_connection
    {//start of interface

        Users? get_user_profile(int user_id);
        Users? get_user_public_profile(int user_id);
        string update_user_profile(int user_id, string firstname, string surname, string? phone_number, DateOnly? date_of_birth);
        string update_profile_picture(int user_id, string picture_url);

    }//end of interface

}//end of namespace