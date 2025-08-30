using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public string Title { get; set; }
    }
}
