using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using raceday_management_system.Database;

namespace raceday_management_system.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class dashboard : ControllerBase
    {//start of class

        //dependencies
        private readonly dashboard_connection _db;

        //constructor
        public dashboard(dashboard_connection db)
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

        //helper method to get logged_in user id
        private int? get_logged_in_user_id()
        {//start of get_logged_in_user_id()

            return HttpContext.Session.GetInt32("User_id");

        }//end of get_logged_in_user_id()

        //GET /api/dashboard/organiser
        [HttpGet("organiser")]
        public IActionResult get_organiser_dashboard()
        {//start of get_organiser_dashboard()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_organiser())
            {//start of if

                return StatusCode(403, new { message = "Only organisers can access this." });

            }//end of if

            //get counts
            int total_events = _db.count_organiser_events(user_id.Value);
            int total_enrolments = _db.count_organiser_enrolments(user_id.Value);

            //get all events
            var my_events = _db.get_organiser_events(user_id.Value);

            //filter upcoming
            var upcoming = my_events.Where(e => e.Event_date > DateTime.UtcNow).ToList();

            //build the per-event summary
            var events_summary = new List<object>();
            foreach (var ev in my_events)
            {//start of foreach

                int enrol_count = _db.count_event_enrolments(ev.Event_id);

                events_summary.Add(new
                {
                    event_id = ev.Event_id,
                    event_name = ev.Event_name,
                    event_date = ev.Event_date,
                    event_status = ev.Event_status,
                    total_enrolments = enrol_count
                });

            }//end of foreach

            return Ok(new
            {
                totalEvents = total_events,
                totalEnrolments = total_enrolments,
                upcomingEvents = upcoming,
                eventsSummary = events_summary
            });

        }//end of get_organiser_dashboard()

        //GET /api/dashboard/participant
        [HttpGet("participant")]
        public IActionResult get_participant_dashboard()
        {//start of get_participant_dashboard()

            var user_id = get_logged_in_user_id();

            if (user_id == null)
            {//start of if

                return Unauthorized(new { message = "Not logged in." });

            }//end of if

            if (!is_participant())
            {//start of if

                return StatusCode(403, new { message = "Only participants can access this." });

            }//end of if

            //get counts
            int total_enrolments = _db.count_participant_enrolments(user_id.Value);
            int results_count = _db.count_participant_results(user_id.Value);

            //get upcoming events they're enrolled in
            var upcoming = _db.get_participant_upcoming_events(user_id.Value);

            //get recent results (top 5)
            var recent = _db.get_recent_participant_results(user_id.Value, 5);

            return Ok(new
            {
                totalEnrolments = total_enrolments,
                upcomingEvents = upcoming,
                resultsCount = results_count,
                recentResults = recent
            });

        }//end of get_participant_dashboard()

    }//end of class

}//end of namespace