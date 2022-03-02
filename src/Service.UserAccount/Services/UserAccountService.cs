using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.ServiceBus;
using Service.Core.Client.Models;
using Service.Core.Client.Services;
using Service.Grpc;
using Service.ServiceBus.Models;
using Service.UserAccount.Domain.Models;
using Service.UserAccount.Grpc;
using Service.UserAccount.Grpc.Models;
using Service.UserAccount.Mappers;
using Service.UserAccount.Postgres.Models;
using Service.UserAccount.Postgres.Services;
using Service.UserInfo.Crud.Grpc;
using Service.UserInfo.Crud.Grpc.Models;

namespace Service.UserAccount.Services
{
	public class UserAccountService : IUserAccountService
	{
		private readonly IAccountRepository _accountRepository;
		private readonly IEncoderDecoder _encoderDecoder;
		private readonly IServiceBusPublisher<UserAccountFilledServiceBusModel> _accoutFilledPublisher;
		private readonly IServiceBusPublisher<ChangeEmailServiceBusModel> _changeEmailPublisher;
		private readonly ILogger<UserAccountService> _logger;
		private readonly ISystemClock _systemClock;
		private readonly IGrpcServiceProxy<IUserInfoService> _userInfoService;
		private readonly ITokenCache _tokenCache;

		public UserAccountService(IAccountRepository accountRepository,
			IEncoderDecoder encoderDecoder,
			IServiceBusPublisher<UserAccountFilledServiceBusModel> accoutFilledPublisher,
			ILogger<UserAccountService> logger,
			ISystemClock systemClock,
			IServiceBusPublisher<ChangeEmailServiceBusModel> changeEmailPublisher,
			IGrpcServiceProxy<IUserInfoService> userInfoService,
			ITokenCache tokenCache)
		{
			_accountRepository = accountRepository;
			_encoderDecoder = encoderDecoder;
			_accoutFilledPublisher = accoutFilledPublisher;
			_logger = logger;
			_systemClock = systemClock;
			_changeEmailPublisher = changeEmailPublisher;
			_userInfoService = userInfoService;
			_tokenCache = tokenCache;
		}

		public async ValueTask<CommonGrpcResponse> SaveAccount(SaveAccountGrpcRequest request)
		{
			bool saved = await _accountRepository.SaveAccount(request.ToEntity(_encoderDecoder));

			if (saved && request.IsFilled())
			{
				var accountFilledServiceBusModel = new UserAccountFilledServiceBusModel {UserId = request.UserId};

				_logger.LogDebug($"Publish UserAccountFilledServiceBusModel into to service bus: {JsonSerializer.Serialize(accountFilledServiceBusModel)}");

				await _accoutFilledPublisher.PublishAsync(accountFilledServiceBusModel);
			}

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

		public async ValueTask<ChangeEmailGrpcResponse> ChangeEmailRequest(ChangeEmailRequestGrpcRequest request)
		{
			request.UserId = new Guid("95b1ee44-e2ee-4761-a872-dd73336888ae");
			string email = request.Email;

			UserInfoGrpcModel userInfo = (await _userInfoService.Service.GetUserInfoByLoginAsync(new UserInfoAuthRequest {UserName = email}))?.UserInfo;
			if (userInfo != null)
			{
				bool fromCurrentUser = userInfo.UserId == request.UserId;

				return new ChangeEmailGrpcResponse
				{
					EmailAlreadyRegistered = !fromCurrentUser,
					CantChangeToSameEmail = fromCurrentUser
				};
			}

			int timeoutMinutes = Program.ReloadedSettings(model => model.ChangeEmailHashTimeoutMinutes).Invoke();

			string token = _encoderDecoder.EncodeProto(new ChangeEmailGrpcModel
			{
				UserId = request.UserId,
				Email = email,
				Expired = _systemClock.Now.AddMinutes(timeoutMinutes)
			});

			var changeEmailServiceBusModel = new ChangeEmailServiceBusModel
			{
				Email = email,
				Hash = token
			};

			_logger.LogDebug($"Publish ChangeEmailServiceBusModel into to service bus: {JsonSerializer.Serialize(changeEmailServiceBusModel)}");

			await _changeEmailPublisher.PublishAsync(changeEmailServiceBusModel);

			return new ChangeEmailGrpcResponse();
		}

		public async ValueTask<ChangeEmailConfirmGrpcResponse> ChangeEmailConfirm(ChangeEmailConfirmGrpcRequest request)
		{
			string hash = request.Hash;
			ChangeEmailGrpcModel changeEmail;

			if (_tokenCache.Exists(hash))
				return new ChangeEmailConfirmGrpcResponse {HashAlreadyUsed = true};

			try
			{
				changeEmail = _encoderDecoder.DecodeProto<ChangeEmailGrpcModel>(hash);
			}
			catch (Exception exception)
			{
				_logger.LogError("Can't decode change email token ({token}), with message {message}", hash, exception.Message);

				return new ChangeEmailConfirmGrpcResponse {Changed = false};
			}

			DateTime expired = changeEmail.Expired;
			if (expired <= _systemClock.Now)
			{
				_logger.LogWarning("Change email hash ({token}) is out of date: {date} for user: {user}", hash, expired, changeEmail.UserId);

				return new ChangeEmailConfirmGrpcResponse {HashExpired = true};
			}

			_tokenCache.Add(hash, expired);

			CommonGrpcResponse response = await _userInfoService.TryCall(service => service.ChangeUserNameAsync(new ChangeUserNameRequest
			{
				UserId = changeEmail.UserId,
				Email = changeEmail.Email
			}));

			return new ChangeEmailConfirmGrpcResponse
			{
				Changed = response?.IsSuccess == true
			};
		}
	}
}