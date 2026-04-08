using System.Text;
using System.Text.Json;

namespace NetflixClone.Services
{
    public class PaystackService
    {
        private readonly HttpClient _httpClient;
        public PaystackService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<PaystackInitializeResponse> InitializeTransactionAsync(InitializeTransactionRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var respnseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paystack API error:{respnseString}");
            }
            return JsonSerializer.Deserialize<PaystackInitializeResponse>(respnseString)!;
        }
    }

    public class InitializeTransactionRequest
    {
        public string Email { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string? Plan { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
    }
    public class PaystackInitializeResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackData Data { get; set; } = new();
    }
    public class PaystackData
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}