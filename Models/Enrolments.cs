using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace raceday_management_system.Models
{//start of namespace
    public class Enrolments
    {//start of class

        [Key]
        public int Enrolment_id { get; set; }

        [Required]
        public int Participant_id { get; set; }

        [Required]
        public int Event_id { get; set; }

        [Required]
        public int Category_id { get; set; }

        public DateTime Enrolment_date { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Enr_status { get; set; } = "Pending";

        [StringLength(20)]
        public string? Payment_status { get; set; } = "Pending";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Payment_amount { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime? Updated_at { get; set; }

    }//end of class

}//end of namespace
