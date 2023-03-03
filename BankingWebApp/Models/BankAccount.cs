using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace BankingWebApp.Models
{
    public class BankAccount
    {
        [Required]
        [MinLength(3)]
        public string AccountHolder { get; set; }
        [Key]
        [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Must be 5 digits")]
        public string BankAccountNumber { get; set; }
        [Precision(18, 2)]
        public decimal CurrentBalance { get; set; }

        [ForeignKey(nameof(AccountType))]
        public AccountTypes AccountTypeId { get; set; }
        public AccountType? AccountType { get; set; }
    }
}