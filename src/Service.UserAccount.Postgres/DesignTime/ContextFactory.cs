using MyJetWallet.Sdk.Postgres;

namespace Service.UserAccount.Postgres.DesignTime
{
	public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
	{
		public ContextFactory() : base(options => new DatabaseContext(options))
		{
		}
	}
}