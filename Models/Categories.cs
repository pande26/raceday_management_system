using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace raceday_management_system.Models
{//start of namespace
    public class Categories
    {//start of class

        [Key]
        public int Category_id { get; set; }

        [Required]
        public int Event_id { get; set; }

        [Required]
        [StringLength(50)]
        public string Category_name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Cat_description { get; set; }

        public int? Min_age { get; set; }

        public int? Max_age { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Min_distance { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Max_distance { get; set; }

        public DateTime Created_at { get; set; } = DateTime.UtcNow;

    }//end of class

}//end of namespace
