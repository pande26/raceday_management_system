using BCrypt.Net;

namespace raceday_management_system.Services
{//start of namespace
    public class password_hasher
    {//start of class

        //method to hash a plain-text password
        public string hash_password(string password)
        {//start of hash_password()

            string hashed = BCrypt.Net.BCrypt.HashPassword(password);

            return hashed;

        }//end of hash_password()

        //method to verify a plain-text password against a stored hash
        public bool verify_password(string password, string stored_hash)
        {//start of verify_password()

            bool is_valid = BCrypt.Net.BCrypt.Verify(password, stored_hash);

            return is_valid;

        }//end of verify_password()

    }//end of class

}//end of namespace