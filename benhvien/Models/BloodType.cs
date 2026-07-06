namespace benhvien.Models
{
    public class BloodType
    {
        public int Id { get; set; }
        public string ABO { get; set; } // Ví dụ: "A", "B", "O", "AB"
        public string Rh { get; set; }  // Ví dụ: "+", "-"
    }
}
