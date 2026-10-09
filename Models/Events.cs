using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace raceday_management_system.Models
{//start of namespace
    public class Events
    {//start of class

        [Key]
        public int Event_id { get; set; }

        [Required]
        public int Organiser_id { get; set; }

        [Required]
        [StringLength(100)]
        public string Event_name { get; set; } = string.Empty;

        [Required]
        public string Event_description { get; set; } = string.Empty;

        [Required]
        public DateTime Event_date { get; set; }

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }

        [Required]
        [StringLength(20)]
        public string Event_type { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Banner_image_url { get; set; }

        public int? Max_participants { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Entry_fee { get; set; }

        [Required]
        [StringLength(20)]
        public string Event_status { get; set; } = "Upcoming";

        public DateTime Created_at { get; set; } = DateTime.UtcNow;

        public DateTime? Updated_at { get; set; }

    }//end of class

}//end of namespace
