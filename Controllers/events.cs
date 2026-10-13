using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class events : ControllerBase
    {//start of class

        private readonly events_connection _db;

        public events(events_connection db)
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

        //GET /api/events
        [HttpGet]
        public IActionResult get_all_events()
        {//start of get_all_events()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var all_events = _db.get_all_events();

            return Ok(all_events);

        }//end of get_all_events()

        //GET /api/events/upcoming
        [HttpGet("upcoming")]
        public IActionResult get_upcoming_events()
        {//start of get_upcoming_events()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var upcoming = _db.get_upcoming_events();

            return Ok(upcoming);

        }//end of get_upcoming_events()

        //GET /api/events/organiser
        [HttpGet("organiser")]
        public IActionResult get_organiser_events()
        {//start of get_organiser_events()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can access this." });

            }//end of if

            var my_events = _db.get_organiser_events(user_id.Value);

            return Ok(my_events);

        }//end of get_organiser_events()

        //GET /api/events/{eventId}
        [HttpGet("{eventId}")]
        public IActionResult get_event_by_id(int eventId)
        {//start of get_event_by_id()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            return Ok(event_item);

        }//end of get_event_by_id()

        //GET /api/events/{eventId}/categories
        [HttpGet("{eventId}/categories")]
        public IActionResult get_event_categories(int eventId)
        {//start of get_event_categories()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            var cats = _db.get_event_categories(eventId);

            return Ok(cats);

        }//end of get_event_categories()

        //GET /api/events/{eventId}/enrolments
        [HttpGet("{eventId}/enrolments")]
        public IActionResult get_event_enrolments(int eventId)
        {//start of get_event_enrolments()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can view event enrolments." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            if (event_item.Organiser_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only view enrolments for your own events." });

            }//end of if

            var enrols = _db.get_event_enrolments(eventId);

            return Ok(enrols);

        }//end of get_event_enrolments()

        //GET /api/events/{eventId}/results
        [HttpGet("{eventId}/results")]
        public IActionResult get_event_results(int eventId)
        {//start of get_event_results()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            var results = _db.get_event_results(eventId);

            return Ok(results);

        }//end of get_event_results()

        //POST /api/events
        [HttpPost]
        public IActionResult create_event(string eventName, string eventDescription, DateTime eventDate, string location, decimal distance, string eventType, int? maxParticipants = null, decimal? entryFee = null)
        {//start of create_event()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can create events." });

            }//end of if

            if (eventType != "Run" && eventType != "Walk" && eventType != "Cycle")
            {//start of if

                return BadRequest(new { message = "Event type must be 'Run', 'Walk', or 'Cycle'." });

            }//end of if

            string message = _db.create_event(
                user_id.Value, eventName, eventDescription, eventDate,
                location, distance, eventType, maxParticipants, entryFee);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return StatusCode(201, new
            {
                message = "Event created successfully.",
                event_name = eventName
            });

        }//end of create_event()

        //PUT /api/events/{eventId}
        [HttpPut("{eventId}")]
        public IActionResult update_event(int eventId, string eventName, string eventDescription, DateTime eventDate, string location, decimal distance, string eventType, string eventStatus, int? maxParticipants = null, decimal? entryFee = null)
        {//start of update_event()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can update events." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            //use model property now
            if (event_item.Organiser_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only update your own events." });

            }//end of if

            string message = _db.update_event(
                eventId, eventName, eventDescription, eventDate, location,
                distance, eventType, eventStatus, maxParticipants, entryFee);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Event updated successfully.",
                event_id = eventId
            });

        }//end of update_event()

        //DELETE /api/events/{eventId}
        [HttpDelete("{eventId}")]
        public IActionResult delete_event(int eventId)
        {//start of delete_event()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can delete events." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            if (event_item.Organiser_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only delete your own events." });

            }//end of if

            string message = _db.delete_event(eventId);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new { message = "Event deleted successfully." });

        }//end of delete_event()

        //POST /api/events/{eventId}/banner
        [HttpPost("{eventId}/banner")]
        public IActionResult set_event_banner(int eventId, string bannerUrl)
        {//start of set_event_banner()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can upload banners." });

            }//end of if

            var event_item = _db.get_event_by_id(eventId);

            if (event_item == null)
            {//start of if

                return NotFound(new { message = "Event not found." });

            }//end of if

            if (event_item.Organiser_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only set banners for your own events." });

            }//end of if

            string message = _db.set_event_banner(eventId, bannerUrl);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Banner updated successfully.",
                bannerImageUrl = bannerUrl
            });

        }//end of set_event_banner()

    }//end of class

}//end of namespace