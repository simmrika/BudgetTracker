namespace BudgetTRacker.Service
{
    public class BankTransactionDataService
    {

        private readonly HttpClient _httpClient;

        public BankTransactionDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Set the base address from configuration
            var baseUrl = configuration["BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"api/transactions/user/{userId}");


            if (response.IsSuccessStatusCode)
            {
                var transactions = await response.Content.ReadFromJsonAsync<IEnumerable<TransactionDto>>();
                return transactions ?? new List<TransactionDto>();
            }

            return new List<TransactionDto>(); // Return empty list if no transactions found or error occurs
        }
    }
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Notes { get; set; }
    }
}
