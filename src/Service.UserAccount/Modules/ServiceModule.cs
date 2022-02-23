using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.Core.Client.Services;
using Service.ServiceBus.Models;
using Service.UserAccount.Postgres.Services;
using Service.UserInfo.Crud.Client;

namespace Service.UserAccount.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<AccountRepository>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<SystemClock>().AsImplementedInterfaces().SingleInstance();
			
			builder.Register(context => new EncoderDecoder(Program.EncodingKey)).As<IEncoderDecoder>().SingleInstance();

			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl, Program.LogFactory.CreateLogger(typeof(UserInfoCrudClientFactory)));

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.UserAccount");

			builder
				.Register(context => new MyServiceBusPublisher<UserAccountFilledServiceBusModel>(tcpServiceBus, UserAccountFilledServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<UserAccountFilledServiceBusModel>>()
				.SingleInstance();

			tcpServiceBus.Start();
		}
	}
}