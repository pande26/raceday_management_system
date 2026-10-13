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

        /// <summary>
        /// Registers a new user account with the specified role (Organiser or Participant).
        /// The password is hashed with BCrypt before being stored.
        /// </summary>
        /// <param name="email">Unique email address.</param>
        /// <param name="password">Plain-text password (min 6 characters).</param>
        /// <param name="firstname">User's first name.</param>
        /// <param name="surname">User's surname.</param>
        /// <param name="role">Role: Organiser or Participant.</param>
        /// <param name="dateOfBirth">Optional date of birth.</param>
        /// <param name="phoneNumber">Optional phone number.</param>
        /// <returns>201 Created. 400 Bad Request. 409 Conflict if email already exists.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Authenticates a user and creates a server-side session.
        /// The session stores the user's id, email, and role.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's plain-text password.</param>
        /// <returns>200 OK with role and userId. 401 Unauthorized if credentials are invalid.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public IActionResult login_user(string email, string password)
        {//start of login_user()

            auth_connection db = new auth_connection();

            var user = db.get_user_by_email(email);

            if (user == null)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            if (!user.Is_active)
            {//start of if

                return Unauthorized(new { message = "Account is inactive." });

            }//end of if

            bool is_valid = _hasher.verify_password(password, user.Password_hash);

            if (!is_valid)
            {//start of if

                return Unauthorized(new { message = "Invalid email or password." });

            }//end of if

            db.update_last_login(user.User_id);

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

        /// <summary>
        /// Ends the current user session and clears session data.
        /// </summary>
        /// <returns>200 OK on success. 401 Unauthorized if no session exists.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Returns the currently authenticated user's id, email, and role.
        /// </summary>
        /// <returns>200 OK with user info. 401 Unauthorized if not logged in.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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