namespace BudgetTRacker.Models
{
    public class CategoryExpenditure
    {

        public int CategoryId {  get; set; }
        public string CategoryName { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
