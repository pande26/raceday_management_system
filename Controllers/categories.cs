using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class categories : ControllerBase
    {//start of class

        //dependencies
        private readonly categories_connection _db;

        //constructor
        public categories(categories_connection db)
        {//start of constructor

            _db = db;

        }//end of constructor

        //helper: check if organiser
        private bool is_organiser()
        {//start of is_organiser()

            return HttpContext.Session.GetString("Role") == "Organiser";

        }//end of is_organiser()

        //helper: get logged-in user id
        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        //GET /api/categories/{categoryId}
        [HttpGet("{categoryId}")]
        public IActionResult get_category_by_id(int categoryId)
        {//start of get_category_by_id()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var category_item = _db.get_category_by_id(categoryId);

            if (category_item == null)
            {//start of if

                return NotFound(new { message = "Category not found." });

            }//end of if

            return Ok(category_item);

        }//end of get_category_by_id()

        //POST /api/categories/{eventId}
        [HttpPost("{eventId}")]
        public IActionResult create_category(int eventId, string categoryName, string? catDescription = null, int? minAge = null, int? maxAge = null, decimal? minDistance = null, decimal? maxDistance = null)
        {//start of create_category()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can create categories." });

            }//end of if

            //check that the logged-in user owns the event
            bool owns = _db.owns_event(eventId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only add categories to your own events." });

            }//end of if

            string message = _db.create_category(
                eventId, categoryName, catDescription,
                minAge, maxAge, minDistance, maxDistance);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return StatusCode(201, new
            {
                message = "Category created successfully.",
                category_name = categoryName
            });

        }//end of create_category()

        //PUT /api/categories/{categoryId}
        [HttpPut("{categoryId}")]
        public IActionResult update_category(int categoryId, string categoryName, string? catDescription = null, int? minAge = null, int? maxAge = null, decimal? minDistance = null, decimal? maxDistance = null)
        {//start of update_category()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can update categories." });

            }//end of if

            //check the category exists
            var category_item = _db.get_category_by_id(categoryId);

            if (category_item == null)
            {//start of if

                return NotFound(new { message = "Category not found." });

            }//end of if

            //check ownership of the parent event
            bool owns = _db.owns_category_event(categoryId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only update categories for your own events." });

            }//end of if

            string message = _db.update_category(categoryId, categoryName, catDescription, minAge, maxAge, minDistance, maxDistance);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Category updated successfully.",
                category_id = categoryId
            });

        }//end of update_category()

        //DELETE /api/categories/{categoryId}
        [HttpDelete("{categoryId}")]
        public IActionResult delete_category(int categoryId)
        {//start of delete_category()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can delete categories." });

            }//end of if

            //check the category exists
            var category_item = _db.get_category_by_id(categoryId);

            if (category_item == null)
            {//start of if

                return NotFound(new { message = "Category not found." });

            }//end of if

            //check ownership of the parent event
            bool owns = _db.owns_category_event(categoryId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only delete categories for your own events." });

            }//end of if

            //check if the category has enrolments
            int enrol_count = _db.count_category_enrolments(categoryId);

            if (enrol_count > 0)
            {//start of if

                return Conflict(new { message = "Cannot delete category with existing enrolments." });

            }//end of if

            string message = _db.delete_category(categoryId);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new { message = "Category deleted successfully." });

        }//end of delete_category()

    }//end of class

}//end of namespace