namespace BankingWebApp.Models
{
    public class Transfer
    {
        public string FromBankAccountId { get; set; }
        public string ToBankAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
