using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Service
{
    public class AccountDetailsDataService
    {
        private readonly HttpClient _httpClient;

        public AccountDetailsDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Set the base address from configuration
            var baseUrl = configuration["BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        // Get User by account number
        public async Task<UserDto?> GetUserByAccountNumberAsync(string accountNumber)
        {
            var response = await _httpClient.GetAsync($"api/User/GetUserByAccountNumber/{accountNumber}");

            if (response.IsSuccessStatusCode)
            {
                // Deserialize response into UserDto
                var user = await response.Content.ReadFromJsonAsync<UserDto>();
                return user;
            }

            // Handle errors (log or throw exception if needed)
            return null;
        }


    }

    public class UserDto
    {
        public string BankName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Balance { get; set; }
        public bool IsApprovedToBudgetTraker { get; set; }
    }


}
