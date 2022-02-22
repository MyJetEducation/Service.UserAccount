using Service.UserAccount.Postgres.Models;

namespace Service.UserAccount.Postgres.Services
{
	public interface IAccountRepository
	{
		ValueTask<bool> SaveAccount(AccountEntity entity);

		ValueTask<AccountEntity> GetAccount(Guid? userId);
	}
}