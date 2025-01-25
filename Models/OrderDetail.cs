namespace AppleStore.Models
{
    public class OrderDetail
    {
        public int Id { get; set; } // Mã chi tiết đơn hàng
        public string OrderId { get; set; } // Mã đơn hàng (khóa ngoại)
        public int ProductId { get; set; } // Mã sản phẩm
        public string ProductName { get; set; } // Tên sản phẩm
        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal Price { get; set; } // Giá của mỗi sản phẩm
        public decimal Total { get; set; } // Tổng tiền của sản phẩm (Quantity * Price)

        // Liên kết với bảng Order
        public virtual Order Order { get; set; }
    }
}
