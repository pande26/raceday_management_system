using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class results : ControllerBase
    {//start of class

        //dependencies
        private readonly results_connection _db;

        //constructor
        public results(results_connection db)
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

        //GET /api/results/me
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

        // GET /api/results/leaderboard/{eventId}
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

        // GET /api/results/{resultId}
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

        // POST /api/results/{eventId}
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

            //check ownership
            bool owns = _db.owns_event(eventId, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only record results for your own events." });

            }//end of if

            //check enrolment belongs to event
            bool enrol_ok = _db.enrolment_belongs_to_event(enrolmentId, eventId);

            if (!enrol_ok)
            {//start of if

                return NotFound(new { message = "Enrolment not found for this event." });

            }//end of if

            //check duplicate
            if (_db.result_exists_for_enrolment(enrolmentId))
            {//start of if

                return Conflict(new { message = "A result already exists for this enrolment." });

            }//end of if

            //parse the finish time
            if (!TimeSpan.TryParse(finishTime, out TimeSpan parsed_time))
            {//start of if

                return BadRequest(new { message = "Finish time must be in format HH:MM:SS." });

            }//end of if

            //get the participant id
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

        //PUT /api/results/{resultId}
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

            //find the result
            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            //check ownership
            bool owns = _db.owns_event(result_item.Event_id, user_id.Value);

            if (!owns)
            {//start of if

                return StatusCode(403, new { message = "You can only update results for your own events." });

            }//end of if

            //parse finish time
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

        //PUT /api/results/{resultId}/publish
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

            //find the result
            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            //check ownership
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

        //DELETE /api/results/{resultId}
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

            //find the result
            var result_item = _db.get_result_by_id(resultId);

            if (result_item == null)
            {//start of if

                return NotFound(new { message = "Result not found." });

            }//end of if

            //check ownership
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

        //GET /api/participants/{participantId}/results
        //organiser view of participant results
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