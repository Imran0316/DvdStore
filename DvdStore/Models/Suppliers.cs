using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Suppliers
    {
        [Key]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain alphabets and spaces")]
        public string SupplierName { get; set; }


        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits")]
        public string? ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
