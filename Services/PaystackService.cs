using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using NetflixClone.Models;
using NetflixClone.Data;

namespace NetflixClone.Services
{
    public class PaystackService

    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        public PaystackService(IHttpClientFactory httpClientFactory, ApplicationDbContext dbContext )
        {
            _httpClient = httpClientFactory.CreateClient("Paystack");
            _dbContext = dbContext;
        }
        public async Task<PaystackInitializeResponse> InitializeTransactionAsync(InitializeTransactionRequest request)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
            var json = JsonSerializer.Serialize(request, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");//https://api.paystack.co/transaction/initialize
            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var respnseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paystack API error:{respnseString}");
            }

            var respo = JsonSerializer.Deserialize<PaystackInitializeResponse>(respnseString)!;
            
            return respo;
        }
        public async Task<PaystackVerifyResponse> VerifyTransactionAsync(string reference)
        {
            var response = await _httpClient.GetAsync($"transaction/verify/{reference}");
            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Verification Failed: {responseString}");
            }
            return JsonSerializer.Deserialize<PaystackVerifyResponse>(responseString, new JsonSerializerOptions
            { PropertyNameCaseInsensitive = true })!;
        }
        public bool VerifyWebhookSignature(string incomingSignature, string rawData)
        {
            string secretKey = "sk_test_d776a7b05aceded2f997db9e7860bd18b2aa9149";
            byte[] keyByte = Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(rawData);

            using (var hmacsha512 = new HMACSHA512(keyByte))
            {
                byte[] hashmessage = hmacsha512.ComputeHash(messageBytes);
                var calculatedSignature = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
                return calculatedSignature == incomingSignature;
            }
        }

    }

    public class InitializeTransactionRequest
    {
        public string Email { get; set; } = string.Empty;
        public int Amount { get; set; }
        //public string? Plan { get; set; }
        //public string Reference { get; set; } = string.Empty;
        //[JsonPropertyName("callback_url")]
    //    public string CallbackUrl { get; set; } = string.Empty;
    }
    public class PaystackInitializeResponse
    {

        [JsonPropertyName("status")]
        public bool Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public PaystackData Data { get; set; } = new();
    }
    public class PaystackData
    {
        [JsonPropertyName("authorization_url")]
        public string AuthorizationUrl { get; set; } = string.Empty;
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;
        [JsonPropertyName("access_code")]
        
        public string AccessCode { get; set; }
    }
    public class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public string Meessage { get; set; } = string.Empty;
        public VerifyData Data { get; set; } = new();
    }
    public class VerifyData
    {
        public string Status { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
    }
}