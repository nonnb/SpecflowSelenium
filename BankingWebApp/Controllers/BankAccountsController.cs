using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankingWebApp.Models;
using BankingWebApp.Repo;

namespace BankingWebApp.Controllers
{
    public class BankAccountsController : Controller
    {
        private readonly BankDbContext _context;

        public BankAccountsController(BankDbContext context)
        {
            _context = context;
        }

        // GET: BankAccounts
        public async Task<IActionResult> Index()
        {
            var bankDbContext = _context.BankAccounts.Include(b => b.AccountType);
            return View(await bankDbContext.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.BankAccounts == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.AccountType)
                .FirstOrDefaultAsync(m => m.BankAccountNumber == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public IActionResult Create()
        {
            ViewData["AccountTypeId"] = new SelectList(_context.AccountType, "AccountTypeId", "AccountTypeId");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountHolder,BankAccountNumber,AccountTypeId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bankAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountType, "AccountTypeId", "AccountTypeId", bankAccount.AccountTypeId);
            return View(bankAccount);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.BankAccounts == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountType, "AccountTypeId", "AccountTypeId", bankAccount.AccountTypeId);
            return View(bankAccount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AccountHolder,BankAccountNumber,AccountTypeId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var existing =
                    await _context.BankAccounts.FirstOrDefaultAsync(ba =>
                        ba.BankAccountNumber == bankAccount.BankAccountNumber);
                if (existing == default)
                {
                    return NotFound();
                }
                try
                {
                    existing.AccountHolder = bankAccount.AccountHolder;
                    existing.AccountTypeId = bankAccount.AccountTypeId;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankAccountExists(bankAccount.BankAccountNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountTypeId"] = new SelectList(_context.AccountType, "AccountTypeId", "AccountTypeId", bankAccount.AccountTypeId);
            return View(bankAccount);
        }

        [HttpGet]
        public async Task<IActionResult> Transfer(string id)
        {
            var existing =
                await _context.BankAccounts.FirstOrDefaultAsync(ba =>
                    ba.BankAccountNumber == id);
            if (existing == default)
            {
                return NotFound();
            }

            return View(new Transfer
            {
                FromBankAccountId = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(Transfer transfer)
        {
            if (!ModelState.IsValid)
            {
                return View(transfer);
            }
            var from = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.BankAccountNumber == transfer.FromBankAccountId);
            var to = await _context.BankAccounts.FirstOrDefaultAsync(ba => ba.BankAccountNumber == transfer.ToBankAccountId);
            if (from == default)
            {
                ModelState.AddModelError(nameof(Models.Transfer.FromBankAccountId), "Can't find from Account");
            }
            if (to == default)
            {
                ModelState.AddModelError(nameof(Models.Transfer.ToBankAccountId), "Can't find to Account");
            }
            if (transfer.Amount > from.CurrentBalance)
            {
                ModelState.AddModelError(nameof(Models.Transfer.ToBankAccountId), $"From has insufficient funds ({from.CurrentBalance})");
            }
            if (ModelState.IsValid)
            {
                from.CurrentBalance -= transfer.Amount;
                to.CurrentBalance += transfer.Amount;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transfer);
        }

        // GET: BankAccounts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.BankAccounts == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.AccountType)
                .FirstOrDefaultAsync(m => m.BankAccountNumber == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.BankAccounts == null)
            {
                return Problem("Entity set 'BankDbContext.BankAccounts'  is null.");
            }
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount != null)
            {
                _context.BankAccounts.Remove(bankAccount);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankAccountExists(string id)
        {
          return _context.BankAccounts.Any(e => e.BankAccountNumber == id);
        }
    }
}
