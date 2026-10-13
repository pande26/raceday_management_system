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

        public enum roles
        {//start of enum

            Organiser,
            Participant

        }//end of enum

        private readonly password_hasher _hasher;

        public authenticator(password_hasher hasher)
        {//start of constructor

            _hasher = hasher;

        }//end of constructor

        //POST /api/authenticator/register
        [HttpPost("register")]
        public IActionResult register_user(string email, string password, string firstname, string surname, roles role, DateOnly? dateOfBirth = null, string? phoneNumber = null)
        {//start of register_user()

            auth_connection db = new auth_connection();

            if (db.email_exists(email))
            {//start of if

                return Conflict(new { message = "Email already registered." });

            }//end of if

            string hashed_password = _hasher.hash_password(password);

            string message = db.store_user_info(
                email, hashed_password, firstname, surname,
                role.ToString(), dateOfBirth, phoneNumber
            );

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

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

            auth_connection db = new auth_connection();

            //find the user — now returns a Users model
            var user = db.get_user_by_email(email);

            if (user == null)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            if (!user.Is_active)
            {//start of if

                return Unauthorized(new { message = "Account is inactive." });

            }//end of if

            //verify the password
            bool is_valid = _hasher.verify_password(password, user.Password_hash);

            if (!is_valid)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            //update last login
            db.update_last_login(user.User_id);

            //store the session
            HttpContext.Session.SetInt32("User_id", user.User_id);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            return Ok(new
            {
                message = "Login successful.",
                role = user.Role,
                userId = user.User_id
            });

        }//end of login_user()

        //POST /api/authenticator/logout
        [HttpPost("logout")]
        public IActionResult logout_user()
        {//start of logout_user()

            if (HttpContext.Session.GetString("Email") == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            HttpContext.Session.Clear();

            return Ok(new { message = "Logout successful." });

        }//end of logout_user()

        //GET /api/authenticator/current-user
        [HttpGet("current-user")]
        public IActionResult current_user()
        {//start of current_user()

            var user_id = HttpContext.Session.GetInt32("User_id");

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            return Ok(new
            {
                user_id = user_id.Value,
                email = HttpContext.Session.GetString("Email"),
                role = HttpContext.Session.GetString("Role")
            });

        }//end of current_user()

    }//end of class

}//end of namespace