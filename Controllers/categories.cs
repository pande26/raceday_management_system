using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class categories : ControllerBase
    {//start of class

        private readonly Icategories_connection _db;

        public categories(Icategories_connection db)
        {//start of constructor

            _db = db;

        }//end of constructor

        private bool is_organiser()
        {//start of is_organiser()

            return HttpContext.Session.GetString("Role") == "Organiser";

        }//end of is_organiser()

        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        /// <summary>
        /// Returns a specific category by ID.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <returns>200 OK. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Creates a new category for an event (Organiser event owner only).
        /// </summary>
        /// <param name="eventId">Parent event ID.</param>
        /// <param name="categoryName">Category name.</param>
        /// <param name="catDescription">Optional category description.</param>
        /// <param name="minAge">Optional minimum age.</param>
        /// <param name="maxAge">Optional maximum age.</param>
        /// <param name="minDistance">Optional minimum distance.</param>
        /// <param name="maxDistance">Optional maximum distance.</param>
        /// <returns>201 Created. 400 Bad Request. 403 Forbidden. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Updates an existing category (Organiser event owner only).
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <param name="categoryName">New category name.</param>
        /// <param name="catDescription">Optional new description.</param>
        /// <param name="minAge">Optional new minimum age.</param>
        /// <param name="maxAge">Optional new maximum age.</param>
        /// <param name="minDistance">Optional new minimum distance.</param>
        /// <param name="maxDistance">Optional new maximum distance.</param>
        /// <returns>200 OK. 400 Bad Request. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            var category_item = _db.get_category_by_id(categoryId);

            if (category_item == null)
            {//start of if

                return NotFound(new { message = "Category not found." });

            }//end of if

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

        /// <summary>
        /// Deletes a category (Organiser event owner only). Fails if the category has enrolments.
        /// </summary>
        /// <param name="categoryId">The category ID.</param>
        /// <returns>200 OK. 403 Forbidden. 404 Not Found. 409 Conflict if has enrolments. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            var category_item = _db.get_category_by_id(categoryId);

            if (category_item == null)
            {//start of if

                return NotFound(new { message = "Category not found." });

            }//end of if

            bool owns = _db.owns_category_event(categoryId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only delete categories for your own events." });

            }//end of if

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