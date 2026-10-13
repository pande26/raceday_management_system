using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class enrolments : ControllerBase
    {//start of class

        private readonly Ienrolments_connection _db;

        public enrolments(Ienrolments_connection db)
        {//start of constructor

            _db = db;

        }//end of constructor

        private bool is_organiser()
        {//start of is_organiser()

            return HttpContext.Session.GetString("Role") == "Organiser";

        }//end of is_organiser()

        private bool is_participant()
        {//start of is_participant()

            return HttpContext.Session.GetString("Role") == "Participant";

        }//end of is_participant()

        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        /// <summary>
        /// Returns all enrolments for the logged-in Participant.
        /// </summary>
        /// <returns>200 OK. 403 Forbidden if not a Participant. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Checks whether the logged-in Participant is enrolled in a specific event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <returns>200 OK with isEnrolled flag. 403 Forbidden. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        /// <summary>
        /// Enrols the logged-in Participant in an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="categoryId">The category to enrol in.</param>
        /// <param name="comments">Optional comments.</param>
        /// <returns>201 Created. 400 Bad Request. 403 Forbidden. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            bool category_ok = _db.category_belongs_to_event(categoryId, eventId);

            if (!category_ok)
            {//start of if

                return BadRequest(new { message = "Category does not exist for this event." });

            }//end of if

            var existing = _db.get_my_enrolment_for_event(user_id.Value, eventId);

            if (existing != null)
            {//start of if

                return BadRequest(new { message = "You are already enrolled in this event." });

            }//end of if

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

        /// <summary>
        /// Cancels an enrolment (Participant owner only). Cannot cancel after the event starts.
        /// </summary>
        /// <param name="enrolmentId">The enrolment ID.</param>
        /// <returns>200 OK. 403 Forbidden. 404 Not Found. 409 Conflict. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{enrolmentId}/cancel")]
        public IActionResult cancel_enrolment(int enrolmentId)
        {//start of cancel_enrolment()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

            if (enrolment_item.Participant_id != user_id.Value)
            {//start of if

                return StatusCode(403, new { message = "You can only cancel your own enrolments." });

            }//end of if

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

        /// <summary>
        /// Updates the enrolment status (Organiser event owner only).
        /// </summary>
        /// <param name="enrolmentId">The enrolment ID.</param>
        /// <param name="enrStatus">New status: Pending, Confirmed, Completed, or Withdrawn.</param>
        /// <returns>200 OK. 400 Bad Request. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            if (enrStatus != "Pending" && enrStatus != "Confirmed" && enrStatus != "Completed" && enrStatus != "Withdrawn")
            {//start of if

                return BadRequest(new { message = "Status must be 'Pending', 'Confirmed', 'Completed', or 'Withdrawn'." });

            }//end of if

            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

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

        /// <summary>
        /// Updates the payment status for an enrolment (Organiser event owner only).
        /// </summary>
        /// <param name="enrolmentId">The enrolment ID.</param>
        /// <param name="paymentStatus">New payment status: Pending, Paid, or Refunded.</param>
        /// <returns>200 OK. 400 Bad Request. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            if (paymentStatus != "Pending" && paymentStatus != "Paid" && paymentStatus != "Refunded")
            {//start of if

                return BadRequest(new { message = "Payment status must be 'Pending', 'Paid', or 'Refunded'." });

            }//end of if

            var enrolment_item = _db.get_enrolment_by_id(enrolmentId);

            if (enrolment_item == null)
            {//start of if

                return NotFound(new { message = "Enrolment not found." });

            }//end of if

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

        /// <summary>
        /// Returns all enrolments for a specific event (Organiser owner only).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <returns>200 OK. 403 Forbidden. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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