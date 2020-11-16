using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotcc.Services;
using Spotcc.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace SpotCC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IRepository<Artist> _artistRepo;
        private readonly IRepository<Account> _accountRepo;
        private readonly ILogger<ArtistController> _logger;

        public AccountController(ILogger<ArtistController> logger, IRepository<Artist> artistRepo, IRepository<Account> accountRepo)
        {
            _artistRepo = artistRepo;
            _accountRepo = accountRepo;

            _logger = logger;
        }
        [HttpGet("streamtype/{type}")]
        public IEnumerable<Account> GetAccountAndArstistByType(int type)
        {
            var accounts = _accountRepo.Get();
            var resuts = accounts.Select(a => new Account
            {
                Id = a.Id,
                Name = a.Name,
                OrdeFreeStreams = a.OrdeFreeStreams,
                OrderPreStreams = a.OrderPreStreams,
                TotalStreams = a.TotalStreams,
                Artists = a.Artists.Where(at => (int)at.StreamType == type).ToList()
            }).Where(d => d.Artists.Count() > 0);
            return resuts;
        }

        [HttpGet("{name}")]
        public Account GetAccount(string name)
        {
            var account = _accountRepo.GetAll().FirstOrDefault(a => a.Name == name);
            return account;
        }

        [HttpPost("update")]
        public IActionResult UpdateAccount(Account account)
        {
            var error = string.Empty;
            if (account == null)
            {
                error = $"Parameter '{nameof(account)}' should not be null.";
            }
            else if (_accountRepo.FindFirst(a => a.Name == account.Name) == null)
            {
                error = $"Account with name: '{account.Name}' is not existed.";
            }

            var currentAccount = GetAccount(account.Name);

            if (currentAccount == null)
            {
                error = $"Account didn't existed.";
            }
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogDebug(error);
                return BadRequest(new { error });
            }

            //fill data
            currentAccount.OrdeFreeStreams = account.OrdeFreeStreams;
            currentAccount.OrderPreStreams = account.OrderPreStreams;

            var result = _accountRepo.Update(currentAccount);
            return CreatedAtAction(nameof(UpdateAccount), new { result }, result);
        }
    }
}