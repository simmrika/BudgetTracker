namespace BankApiProj.Model
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Notes { get; set; }

        public string Status { get; set; }
    }

}
