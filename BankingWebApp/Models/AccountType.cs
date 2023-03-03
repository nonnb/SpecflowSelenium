using System.ComponentModel.DataAnnotations;

namespace BankingWebApp.Models;

public class AccountType
{
    [Key]
    public AccountTypes AccountTypeId { get; set; }
    public string Name { get; set; }
}