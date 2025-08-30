using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace DvdStore.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public int AlbumId { get; set; }
        public int SuplierId { get; set; }
        public int ProducerId { get; set; }
    }
}
