using BankingWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankingWebApp.Repo
{
    public class BankDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<AccountType> AccountType { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }

        public BankDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SurveyContextConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            TurnOffCascadeDeleteGlobally(modelBuilder);
            base.OnModelCreating(modelBuilder);

            SeedBankAccountTypes(modelBuilder);
        }

        private static void TurnOffCascadeDeleteGlobally(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
            {
                fk.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }

        private static void SeedBankAccountTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountType>().HasData(new AccountType { AccountTypeId = AccountTypes.Savings, Name = "Savings" });
            modelBuilder.Entity<AccountType>().HasData(new AccountType { AccountTypeId = AccountTypes.Cheque, Name = "Cheque" });
        }

        private static void SeedBankAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BankAccount>().HasData(new BankAccount { AccountTypeId = AccountTypes.Savings, BankAccountNumber = "00001", AccountHolder = "Bill Gates", CurrentBalance = 1000000.0m});
        }
    }
}
