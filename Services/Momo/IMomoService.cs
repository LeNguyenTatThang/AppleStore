using AppleStore.Models;
using AppleStore.Models.Momo;
namespace AppleStore.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model);
        Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection collection);
    }
}
