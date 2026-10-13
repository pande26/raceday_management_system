using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class users : ControllerBase
    {//start of class

        private readonly users_connection _db;

        public users(users_connection db)
        {//start of constructor

            _db = db;

        }//end of constructor

        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        //GET /api/users/profile
        [HttpGet("profile")]
        public IActionResult get_own_profile()
        {//start of get_own_profile()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var user = _db.get_user_profile(user_id.Value);

            if (user == null)
            {//start of if

                return NotFound(new { message = "User not found." });

            }//end of if

            //return without the password_hash
            return Ok(new
            {
                user_id = user.User_id,
                email = user.Email,
                firstname = user.Firstname,
                surname = user.Surname,
                role = user.Role,
                profile_picture_url = user.Profile_picture_url,
                date_of_birth = user.Date_of_birth,
                phone_number = user.Phone_number,
                created_at = user.Created_at,
                updated_at = user.Updated_at,
                is_active = user.Is_active
            });

        }//end of get_own_profile()

        //PUT /api/users/profile
        [HttpPut("profile")]
        public IActionResult update_own_profile(string firstname, string surname, string? phoneNumber = null, DateOnly? dateOfBirth = null)
        {//start of update_own_profile()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            string message = _db.update_user_profile(
                user_id.Value, firstname, surname, phoneNumber, dateOfBirth);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Profile updated successfully.",
                user_id = user_id.Value
            });

        }//end of update_own_profile()

        //POST /api/users/profile-picture
        [HttpPost("profile-picture")]
        public IActionResult set_profile_picture(string pictureUrl)
        {//start of set_profile_picture()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var role = HttpContext.Session.GetString("Role");

            if (role != "Participant")
            {//start of if

                return StatusCode(403, new { message = "Only participants can upload profile pictures." });

            }//end of if

            string message = _db.update_profile_picture(user_id.Value, pictureUrl);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Profile picture updated successfully.",
                profilePictureUrl = pictureUrl
            });

        }//end of set_profile_picture()

        //GET /api/users/{userId}/profile
        [HttpGet("{userId}/profile")]
        public IActionResult get_user_public_profile(int userId)
        {//start of get_user_public_profile()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var user = _db.get_user_public_profile(userId);

            if (user == null)
            {//start of if

                return NotFound(new { message = "User not found." });

            }//end of if

            return Ok(new
            {
                user_id = user.User_id,
                firstname = user.Firstname,
                surname = user.Surname,
                role = user.Role,
                profile_picture_url = user.Profile_picture_url
            });

        }//end of get_user_public_profile()

    }//end of class

}//end of namespace