using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.Models
{//start of namepace
    public class Results
    {//start of class

        [Key]
        public int Result_id { get; set; }

        [Required]
        public int Enrolment_id { get; set; }

        [Required]
        public int Event_id { get; set; }

        [Required]
        public int Participant_id { get; set; }

        [Required]
        public TimeSpan Finish_time { get; set; }

        [Required]
        public int Overall_position { get; set; }

        public int? Category_position { get; set; }

        [Required]
        public int Total_finishers { get; set; }

        public int? Category_total { get; set; }

        public bool Is_disqualified { get; set; } = false;

        [StringLength(200)]
        public string? Disqualification_reason { get; set; }

        [Required]
        public int Recorded_by { get; set; }

        public DateTime Recorded_at { get; set; } = DateTime.UtcNow;

        public DateTime? Updated_at { get; set; }
    }//end of class

}//end of namespace
