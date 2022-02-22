using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Service.Grpc;
using Service.UserAccount.Grpc;

namespace Service.UserAccount.Client
{
	[UsedImplicitly]
	public class UserAccountClientFactory : GrpcClientFactory
	{
		public UserAccountClientFactory(string grpcServiceUrl, ILogger logger) : base(grpcServiceUrl, logger)
		{
		}

		public IGrpcServiceProxy<IUserAccountService> GetUserAccountService() => CreateGrpcService<IUserAccountService>();
	}
}