namespace Pronia.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string? MainImgUrl { get; set; }
        public string? HoverImgUrl { get; set; }
        public string AdditionImgUrl { get; set; }
        public bool? IsPrime { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        [NotMapped]
        public IFormFile MainImageFile { get; set; }
        [NotMapped]
        public IFormFile? HoverImageFile { get; set; }
        [NotMapped]
        public IFormFile? AdditionImageFile { get; set; }
    }
}
