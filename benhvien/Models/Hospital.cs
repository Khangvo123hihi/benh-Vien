namespace benhvien.Models
{
    public class Hospital
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string?Address { get; set; }

        public string?Phone { get; set; }

        public string?Email { get; set; }

        public string?Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
