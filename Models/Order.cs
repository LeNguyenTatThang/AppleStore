using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models
{
    public class Order
    {
        public int Id { get; set; } // Mã đơn hàng
        public string OrderId { get; set; }
        public string CustomerName { get; set; } // Tên khách hàng
        public string Address { get; set; } // Địa chỉ giao hàng
        public string Phone { get; set; } // Số điện thoại
        public string PaymentMethod { get; set; } // Phương thức thanh toán
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền
        public DateTime OrderDate { get; set; } // Ngày đặt hàng

        // Danh sách các sản phẩm trong đơn hàng (liên kết với OrderDetails)
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
