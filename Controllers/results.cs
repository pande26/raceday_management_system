using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database.Interfaces;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class results : ControllerBase
    {//start of class

        private readonly Iresults_connection _db;

        public results(Iresults_connection db)
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
        /// Returns all results for the logged-in Participant.
        /// </summary>
        /// <returns>200 OK. 403 Forbidden if not a Participant. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("me")]
        public IActionResult get_my_results()
        {//start of get_my_results()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_participant())
            {//start of if

                return StatusCode(403, new { message = "Only participants can access this." });

            }//end of if

            var my_results = _db.get_my_results(user_id.Value);

            return Ok(my_results);

        }//end of get_my_results()

        /// <summary>
        /// Returns the leaderboard for an event, sorted by overall position.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <returns>200 OK. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("leaderboard/{eventId}")]
        public IActionResult get_leaderboard(int eventId)
        {//start of get_leaderboard()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var leaderboard = _db.get_leaderboard(eventId);

            return Ok(leaderboard);

        }//end of get_leaderboard()

        /// <summary>
        /// Returns a single result by its ID.
        /// </summary>
        /// <param name="resultId">The result ID.</param>
        /// <returns>200 OK. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{resultId}")]
        public IActionResult get_result_by_id(int resultId)
        {//start of get_result_by_id()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            return Ok(result_item);

        }//end of get_result_by_id()

        /// <summary>
        /// Records a result for an enrolment (Organiser event owner only).
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="enrolmentId">The enrolment to record a result for.</param>
        /// <param name="finishTime">Finish time in HH:MM:SS format.</param>
        /// <param name="overallPosition">Overall finishing position.</param>
        /// <param name="totalFinishers">Total number of finishers.</param>
        /// <param name="categoryPosition">Optional category position.</param>
        /// <param name="categoryTotal">Optional total in category.</param>
        /// <param name="isDisqualified">Optional disqualification flag.</param>
        /// <param name="disqualificationReason">Optional reason if disqualified.</param>
        /// <returns>201 Created. 400 Bad Request. 403 Forbidden. 404 Not Found. 409 Conflict. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("{eventId}")]
        public IActionResult record_result(int eventId, int enrolmentId, string finishTime, int overallPosition, int totalFinishers, int? categoryPosition = null, int? categoryTotal = null, bool isDisqualified = false, string? disqualificationReason = null)
        {//start of record_result()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can record results." });

            }//end of if

            bool owns = _db.owns_event(eventId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only record results for your own events." });

            }//end of if

            bool enrol_ok = _db.enrolment_belongs_to_event(enrolmentId, eventId);

            if (!enrol_ok)
            {//start of if

                return NotFound(new { message = "Enrolment not found for this event." });

            }//end of if

            if (_db.result_exists_for_enrolment(enrolmentId))
            {//start of if

                return Conflict(new { message = "A result already exists for this enrolment." });

            }//end of if

            if (!TimeSpan.TryParse(finishTime, out TimeSpan parsed_time))
            {//start of if

                return BadRequest(new { message = "Finish time must be in format HH:MM:SS." });

            }//end of if

            int? participant_id = _db.get_participant_id_for_enrolment(enrolmentId);

            if (participant_id == null)
            {//start of if

                return BadRequest(new { message = "Could not find the participant for this enrolment." });

            }//end of if

            string message = _db.record_result(
                enrolmentId, eventId, participant_id.Value, parsed_time,
                overallPosition, categoryPosition, totalFinishers, categoryTotal,
                isDisqualified, disqualificationReason, user_id.Value);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return StatusCode(201, new
            {
                message = "Result recorded successfully.",
                event_id = eventId,
                participant_id = participant_id.Value
            });

        }//end of record_result()

        /// <summary>
        /// Updates an existing result (Organiser event owner only).
        /// </summary>
        /// <param name="resultId">The result ID.</param>
        /// <param name="finishTime">New finish time in HH:MM:SS format.</param>
        /// <param name="overallPosition">New overall position.</param>
        /// <param name="totalFinishers">New total finishers.</param>
        /// <param name="categoryPosition">Optional new category position.</param>
        /// <param name="categoryTotal">Optional new category total.</param>
        /// <param name="isDisqualified">Optional disqualification flag.</param>
        /// <param name="disqualificationReason">Optional reason if disqualified.</param>
        /// <returns>200 OK. 400 Bad Request. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{resultId}")]
        public IActionResult update_result(int resultId, string finishTime, int overallPosition, int totalFinishers, int? categoryPosition = null, int? categoryTotal = null, bool isDisqualified = false, string? disqualificationReason = null)
        {//start of update_result()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can update results." });

            }//end of if

            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            bool owns = _db.owns_event(result_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only update results for your own events." });

            }//end of if

            if (!TimeSpan.TryParse(finishTime, out TimeSpan parsed_time))
            {//start of if

                return BadRequest(new { message = "Finish time must be in format HH:MM:SS." });

            }//end of if

            string message = _db.update_result(
                resultId, parsed_time, overallPosition, categoryPosition,
                totalFinishers, categoryTotal, isDisqualified, disqualificationReason);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Result updated successfully.",
                result_id = resultId
            });

        }//end of update_result()

        /// <summary>
        /// Marks a result as published (Organiser event owner only).
        /// </summary>
        /// <param name="resultId">The result ID.</param>
        /// <returns>200 OK. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{resultId}/publish")]
        public IActionResult publish_result(int resultId)
        {//start of publish_result()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can publish results." });

            }//end of if

            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            bool owns = _db.owns_event(result_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only publish results for your own events." });

            }//end of if

            string message = _db.publish_result(resultId);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new
            {
                message = "Result published successfully.",
                result_id = resultId
            });

        }//end of publish_result()

        /// <summary>
        /// Deletes a result (Organiser event owner only).
        /// </summary>
        /// <param name="resultId">The result ID.</param>
        /// <returns>200 OK. 403 Forbidden. 404 Not Found. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{resultId}")]
        public IActionResult delete_result(int resultId)
        {//start of delete_result()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can delete results." });

            }//end of if

            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            bool owns = _db.owns_event(result_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only delete results for your own events." });

            }//end of if

            string message = _db.delete_result(resultId);

            if (message.Contains("Error"))
            {//start of if

                return BadRequest(new { message = message });

            }//end of if

            return Ok(new { message = "Result deleted successfully." });

        }//end of delete_result()

        /// <summary>
        /// Returns all results for a specific participant (Organiser only).
        /// </summary>
        /// <param name="participantId">The participant's user ID.</param>
        /// <returns>200 OK. 403 Forbidden. 401 Unauthorized.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("/api/participants/{participantId}/results")]
        public IActionResult get_participant_results(int participantId)
        {//start of get_participant_results()

            if (get_logged_in_user_id() == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can access this." });

            }//end of if

            var participant_results = _db.get_participant_results(participantId);

            return Ok(participant_results);

        }//end of get_participant_results()

    }//end of class

}//end of namespace