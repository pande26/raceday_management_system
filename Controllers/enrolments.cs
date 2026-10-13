using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class enrolments : ControllerBase
    {//start of class

        //dependencies
        private readonly enrolments_connection _db;

        //constructor
        public enrolments(enrolments_connection db)
        {//start of constructor

            _db = db;

        }//end of constructor

        //helper: check if organiser
        private bool is_organiser()
        {//start of is_organiser()

            return HttpContext.Session.GetString("Role") == "Organiser";

        }//end of is_organiser()

        //helper: check if participant
        private bool is_participant()
        {//start of is_participant()

            return HttpContext.Session.GetString("Role") == "Participant";

        }//end of is_participant()

        //helper: get logged-in user id
        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        //GET /api/enrolments/me
        [HttpGet("me")]
        public IActionResult get_my_enrolments()
        {//start of get_my_enrolments()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_participant())
            {//start of if

                return StatusCode(403, new { message = "Only participants can access this." });

            }//end of if

            var my_enrolments = _db.get_my_enrolments(user_id.Value);

            return Ok(my_enrolments);

        }//end of get_my_enrolments()

        //GET /api/enrolments/me/{eventId}
        [HttpGet("me/{eventId}")]
        public IActionResult check_my_enrolment(int eventId)
        {//start of check_my_enrolment()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_participant())
            {//start of if

                return StatusCode(403, new { message = "Only participants can access this." });

            }//end of if

            var enrolment_item = _db.get_my_enrolment_for_event(user_id.Value, eventId);

            return Ok(new
            {
                isEnrolled = enrolment_item != null,
                enrolment = enrolment_item
            });

        }//end of check_my_enrolment()

        //POST /api/enrolments/{eventId}
        [HttpPost("{eventId}")]
        public IActionResult enrol_in_event(int eventId, int categoryId, string? comments = null)
        {//start of enrol_in_event()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_participant())
            {//start of if

                return StatusCode(403, new { message = "Only participants can enrol." });

            }//end of if

            //check that the category belongs to the event
            bool category_ok = _db.category_belongs_to_event(categoryId, eventId);

            if (!category_ok)
            {//start of if

                return BadRequest(new { message = "Category does not exist for this event." });

            }//end of if

            //check if already enrolled
            var existing = _db.get_my_enrolment_for_event(user_id.Value, eventId);

            if (existing != null)
            {//start of if

                return BadRequest(new { message = "You are already enrolled in this event." });

            }//end of if

            //create the enrolment
            string message = _db.create_enrolment(user_id.Value, eventId, categoryId, comments);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return StatusCode(201, new
            {
                message = "Enrolment successful.",
                event_id = eventId,
                category_id = categoryId,
                status = "Pending"
            });

        }//end of enrol_in_event()

        //PUT /api/enrolments/{enrolmentId}/cancel
        [HttpPut("{enrolmentId}/cancel")]
        public IActionResult cancel_enrolment(int enrolmentId)
        {//start of cancel_enrolment()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            //find the enrolment
            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

            //check ownership
            if (enrolment_item.Participant_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only cancel your own enrolments." });

            }//end of if

            //check if the event already started
            bool started = _db.event_has_started(enrolment_item.Event_id);

            if (started)
            {//start of if

                return Conflict(new { message = "Cannot cancel after the event has started." });

            }//end of if

            string message = _db.cancel_enrolment(enrolmentId);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Enrolment cancelled successfully.",
                enrolment_id = enrolmentId,
                status = "Withdrawn"
            });

        }//end of cancel_enrolment()

        //PUT /api/enrolments/{enrolmentId}/status
        [HttpPut("{enrolmentId}/status")]
        public IActionResult update_enrolment_status(int enrolmentId, string enrStatus)
        {//start of update_enrolment_status()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can update enrolment status." });

            }//end of if

            //validate status
            if (enrStatus != "Pending" && enrStatus != "Confirmed" && enrStatus != "Completed" && enrStatus != "Withdrawn")
            {//start of if

                return BadRequest(new { message = "Status must be 'Pending', 'Confirmed', 'Completed', or 'Withdrawn'." });

            }//end of if

            //find the enrolment
            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

            //check ownership of the event
            bool owns = _db.owns_event(enrolment_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only update enrolments for your own events." });

            }//end of if

            string message = _db.update_enrolment_status(enrolmentId, enrStatus);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Enrolment status updated successfully.",
                enrolment_id = enrolmentId,
                status = enrStatus
            });

        }//end of update_enrolment_status()

        //PUT /api/enrolments/{enrolmentId}/payment
        [HttpPut("{enrolmentId}/payment")]
        public IActionResult update_payment_status(int enrolmentId, string paymentStatus)
        {//start of update_payment_status()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can update payment status." });

            }//end of if

            //validate payment status
            if (paymentStatus != "Pending" && paymentStatus != "Paid" && paymentStatus != "Refunded")
            {//start of if

                return BadRequest(new { message = "Payment status must be 'Pending', 'Paid', or 'Refunded'." });

            }//end of if

            //find the enrolment
            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

            //check ownership
            bool owns = _db.owns_event(enrolment_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only update payments for your own events." });

            }//end of if

            string message = _db.update_payment_status(enrolmentId, paymentStatus);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Payment status updated successfully.",
                enrolment_id = enrolmentId,
                payment_status = paymentStatus
            });

        }//end of update_payment_status()

        //GET /api/enrolments/event/{eventId}
        //organiser view of all enrolments for an event
        [HttpGet("event/{eventId}")]
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

            //check ownership
            bool owns = _db.owns_event(eventId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only view enrolments for your own events." });

            }//end of if

            var enrols = _db.get_event_enrolments(eventId);

            return Ok(enrols);

        }//end of get_event_enrolments()

    }//end of class

}//end of namespace