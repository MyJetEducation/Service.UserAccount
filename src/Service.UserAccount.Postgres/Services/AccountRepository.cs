using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.UserAccount.Postgres.Models;

namespace Service.UserAccount.Postgres.Services
{
	public class AccountRepository : IAccountRepository
	{
		private readonly ILogger<AccountRepository> _logger;
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

		public AccountRepository(ILogger<AccountRepository> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
		{
			_logger = logger;
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
		}

		public async ValueTask<AccountEntity> GetAccount(Guid? userId)
		{
			try
			{
				return await GetEntity(userId);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);
			}

			return await ValueTask.FromResult<AccountEntity>(null);
		}

		private async Task<AccountEntity> GetEntity(Guid? userId) => await GetContext()
			.Accounts
			.Where(entity => entity.UserId == userId)
			.FirstOrDefaultAsync();

		public async ValueTask<bool> SaveAccount(AccountEntity entity)
		{
			try
			{
				AccountEntity existingAccount = await GetEntity(entity.UserId);

				DatabaseContext context = GetContext();

				if (existingAccount == null)
					await context.Accounts.AddAsync(entity);
				else
					context.Update(entity);

				await context.SaveChangesAsync();

				return true;
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, exception.Message);
			}

			return false;
		}

		private DatabaseContext GetContext() => DatabaseContext.Create(_dbContextOptionsBuilder);
	}
}