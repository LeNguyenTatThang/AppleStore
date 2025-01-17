using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace AppleStore.Models
{
    [Table("Product", Schema = "dbo")]
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Thông tin")]
        public string Information { get; set; }

        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Hình ảnh là bắt buộc.")]
        [Display(Name = "Hình ảnh")]
        public IFormFile Image { get; set; }

    }
}
