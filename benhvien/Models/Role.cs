using System.ComponentModel.DataAnnotations;

namespace benhvien.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}