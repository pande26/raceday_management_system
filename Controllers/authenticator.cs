using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;
using raceday_management_system.Services;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class authenticator : ControllerBase
    {//start of class

        //enum for roles
        public enum roles
        {//start of enum

            Organiser,
            Participant

        }//end of enum

        //dependency for password hashing
        private readonly password_hasher _hasher;

        //constructor
        public authenticator(password_hasher hasher)
        {//start of constructor

            _hasher = hasher;

        }//end of constructor

        //POST /api/authenticator/register
        [HttpPost("register")]
        public IActionResult register_user( string email, string password, string firstname, string surname, roles role, DateOnly? dateOfBirth = null, string? phoneNumber = null)
        {//start of register_user()

            //create an instance to talk to the database
            auth_connection db = new auth_connection();

            //check if the email already exists
            if (db.email_exists(email))
            {//start of if

                return Conflict(new { message = "Email already registered." });

            }//end of if

            //hash the password before storing
            string hashed_password = _hasher.hash_password(password);

            //store the user
            string message = db.store_user_info( email, hashed_password, firstname, surname, role.ToString(), dateOfBirth, phoneNumber);

            //if the message contains "Error", it failed
            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            //return success
            return StatusCode(201, new
            {
                message = "Registration successful.",
                email = email,
                role = role.ToString()
            });

        }//end of register_user()

        //POST /api/authenticator/login
        [HttpPost("login")]
        public IActionResult login_user(string email, string password)
        {//start of login_user()

            //create an instance to talk to the database
            auth_connection db = new auth_connection();

            //find the user by email
            var user = db.get_user_by_email(email);

            //if user does not exist, return 401
            if (user == null)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            //check if the account is active
            if (!Convert.ToBoolean(user["is_active"]))
            {//start of if

                return Unauthorized(new { message = "Account is inactive." });

            }//end of if

            //get the stored hash
            string stored_hash = user["password_hash"]?.ToString() ?? "";

            //verify the password using BCrypt
            bool is_valid = _hasher.verify_password(password, stored_hash);

            //if the password is wrong, return 401
            if (!is_valid)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            //get the user's info
            int user_id = Convert.ToInt32(user["user_id"]);
            string role = user["role"]?.ToString() ?? "";

            //update the last login timestamp
            db.update_last_login(user_id);

            //store the session data
            HttpContext.Session.SetInt32("User_id", user_id);
            HttpContext.Session.SetString("Email", email);
            HttpContext.Session.SetString("Role", role);

            //return success
            return Ok(new
            {
                message = "Login successful.",
                role = role,
                userId = user_id
            });

        }//end of login_user()

        //POST /api/authenticator/logout
        [HttpPost("logout")]
        public IActionResult logout_user()
        {//start of logout_user()

            //check if the user is logged in
            if (HttpContext.Session.GetString("Email") == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            //clear the session
            HttpContext.Session.Clear();

            return Ok(new { message = "Logout successful." });

        }//end of logout_user()

        //GET /api/authenticator/current-user
        [HttpGet("current-user")]
        public IActionResult current_user()
        {//start of current_user()

            //check if the user is logged in
            var user_id = HttpContext.Session.GetInt32("User_id");

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            //return the user's info
            return Ok(new
            {
                user_id = user_id.Value,
                email = HttpContext.Session.GetString("Email"),
                role = HttpContext.Session.GetString("Role")
            });

        }//end of current_user()

    }//end of class

}//end of namespace