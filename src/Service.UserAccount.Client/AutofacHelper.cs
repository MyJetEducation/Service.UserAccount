using Autofac;
using Microsoft.Extensions.Logging;
using Service.Grpc;
using Service.UserAccount.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.UserAccount.Client
{
	public static class AutofacHelper
	{
		public static void RegisterUserAccountClient(this ContainerBuilder builder, string grpcServiceUrl, ILogger logger)
		{
			var factory = new UserAccountClientFactory(grpcServiceUrl, logger);

			builder.RegisterInstance(factory.GetUserAccountService()).As<IGrpcServiceProxy<IUserAccountService>>().SingleInstance();
		}
	}
}