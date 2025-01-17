using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using AppleStore.Models;
using AppleStore.Models.Momo;
using System.Security.Cryptography;
using System.Text;
using AppleStore.Controllers;

namespace AppleStore.Services.Momo
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfo model)
        {
            model.OrderId = DateTime.UtcNow.Ticks.ToString();
            model.OrderInformation = "Khách hàng: " + model.FullName + ". Nội dung: " + model.OrderInformation;
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={model.OrderId}" +
                $"&amount={model.Amount}" +
                $"&orderId={model.OrderId}" +
                $"&orderInfo={model.OrderInformation}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData=";
            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);
            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");
            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = model.OrderId,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInformation,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);


        }
        public async Task<MomoExecuteResponseModel> PaymentExecuteAsync(IQueryCollection collection)
        {
            var response = new MomoExecuteResponseModel();

            try
            {
                // Gán dữ liệu từ collection
                response.FullName = collection["fullname"];
                response.OrderId = collection["orderId"];
                response.Amount = collection["amount"];
                response.OrderInfo = collection["orderInfo"];

                // Thêm logic xử lý khác nếu cần
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                throw new Exception("Lỗi khi xử lý callback từ Momo: " + ex.Message);
            }

            // Trả về kết quả dưới dạng Task
            return await Task.FromResult(response);
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
