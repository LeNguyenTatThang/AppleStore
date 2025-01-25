namespace AppleStore.Models
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; } // Danh sách sản phẩm
        public int CurrentPage { get; set; } // Trang hiện tại
        public int TotalPages { get; set; } // Tổng số trang
        public string Keyword { get; set; } // Từ khóa tìm kiếm
        public string SortOrder { get; set; } // Kiểu sắp xếp (theo tên, giá...)
    }
}
