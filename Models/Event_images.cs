using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.Models
{//start of namespace
    public class Event_images
    {//start of class

        [Key]
        public int Image_id { get; set; }

        [Required]
        public int Event_id { get; set; }

        [Required]
        [StringLength(500)]
        public string Image_url { get; set; } = string.Empty;

        public bool Is_primary { get; set; } = false;

        [StringLength(200)]
        public string? Caption { get; set; }

        public DateTime Uploaded_at { get; set; } = DateTime.UtcNow;

        [Required]
        public int Uploaded_by { get; set; }

    }//end of class

}//end of namespace
