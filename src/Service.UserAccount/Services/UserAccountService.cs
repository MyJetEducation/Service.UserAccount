using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Models;
using Service.Core.Client.Services;
using Service.ServiceBus.Models;
using Service.UserAccount.Grpc;
using Service.UserAccount.Grpc.Models;
using Service.UserAccount.Mappers;
using Service.UserAccount.Postgres.Models;
using Service.UserAccount.Postgres.Services;

namespace Service.UserAccount.Services
{
	public class UserAccountService : IUserAccountService
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IEncoderDecoder _encoderDecoder;
		private readonly IServiceBusPublisher<UserAccountFilledServiceBusModel> _publisher;

		public UserAccountService(IAccountRepository accountRepository, IEncoderDecoder encoderDecoder, IServiceBusPublisher<UserAccountFilledServiceBusModel> publisher)
		{
			_accountRepository = accountRepository;
			_encoderDecoder = encoderDecoder;
			_publisher = publisher;
		}

		public async ValueTask<CommonGrpcResponse> SaveAccount(SaveAccountGrpcRequest request)
		{
			bool saved = await _accountRepository.SaveAccount(request.ToEntity(_encoderDecoder));

			if (saved && request.IsFilled())
				await _publisher.PublishAsync(new UserAccountFilledServiceBusModel {UserId = request.UserId});

			return CommonGrpcResponse.Result(saved);
		}

		public async ValueTask<AccountGrpcResponse> GetAccount(GetAccountGrpcRequest request)
		{
			AccountEntity accountEntity = await _accountRepository.GetAccount(request.UserId);

			return new AccountGrpcResponse
			{
				Data = accountEntity?.ToGrpcModel(_encoderDecoder)
			};
		}
	}
}