using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.Models
{//start of namespace
    public class Users
    {//start of class

        [Key]
        public int User_id { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password_hash { get; set; } = string.Empty;

        [Required]
        [StringLength(55)]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Surname { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Profile_picture_url { get; set; }

        public DateOnly? Date_of_birth { get; set; }

        [StringLength(20)]
        public string? Phone_number { get; set; }

        public DateTime Created_at { get; set; } = DateTime.UtcNow;

        public DateTime? Updated_at { get; set; }

        public bool Is_active { get; set; } = true;

    }//end of class

}//end of namespace
