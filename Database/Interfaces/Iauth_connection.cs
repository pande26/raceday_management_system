using raceday_management_system.Models;

namespace raceday_management_system.Database.Interfaces
{//start of namespace
    public interface Iauth_connection
    {//start of interface

        string store_user_info(
            string email,
            string password_hash,
            string firstname,
            string surname,
            string role,
            DateOnly? date_of_birth,
            string? phone_number);

        bool email_exists(string email);

        Users? get_user_by_email(string email);

        void update_last_login(int user_id);

    }//end of interface

}//end of namespace