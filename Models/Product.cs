using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models
{
    public class Product 
    {
        [Display(Name = "Urun Id")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Urun Adi")]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage ="Gerekli bir alan")]
        [Range(0,100000)]
        [Display(Name = "Urun Fiyati")]
        public decimal? Price { get; set; }
        [Display(Name = "Urun Resmi")]
        public string? Image { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [Display(Name = "Kategori")]
        [Required]
        public int? CategoryId { get; set; }
    }
}