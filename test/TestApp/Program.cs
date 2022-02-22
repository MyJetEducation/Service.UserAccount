using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Core.Client.Models;
using Service.Grpc;
using Service.UserAccount.Client;
using Service.UserAccount.Grpc;
using Service.UserAccount.Grpc.Models;
using GrpcClientFactory = ProtoBuf.Grpc.Client.GrpcClientFactory;

namespace TestApp
{
	internal class Program
	{
		private static async Task Main()
		{
			GrpcClientFactory.AllowUnencryptedHttp2 = true;
			ILogger<Program> logger = LoggerFactory.Create(x => x.AddConsole()).CreateLogger<Program>();

			Console.Write("Press enter to start");
			Console.ReadLine();

			var factory = new UserAccountClientFactory("http://localhost:5001", logger);
			IGrpcServiceProxy<IUserAccountService> client = factory.GetUserAccountService();
			IUserAccountService clientService = client.Service;

			//Save account
			Console.WriteLine($"{Environment.NewLine}Save new account");
			var userId = Guid.NewGuid();
			CommonGrpcResponse saveAccountResponse1 = await clientService.SaveAccount(new SaveAccountGrpcRequest
			{
				UserId = userId,
				FirstName = "Name1",
				LastName = "Name2",
				Gender = "male",
				Phone = "911",
				Country = "EU"
			});
			LogData(saveAccountResponse1);

			//Get this account
			Console.WriteLine($"{Environment.NewLine}Get saved account");
			AccountGrpcResponse getAccountResponse1 = await clientService.GetAccount(new GetAccountGrpcRequest {UserId = userId});
			LogData(getAccountResponse1);

			//Update account
			Console.WriteLine($"{Environment.NewLine}Update existing account");
			CommonGrpcResponse saveAccountResponse2 = await clientService.SaveAccount(new SaveAccountGrpcRequest
			{
				UserId = userId,
				FirstName = "Name10",
				LastName = "Name20",
				Gender = "female",
				Phone = "02",
				Country = "USA"
			});
			LogData(saveAccountResponse2);

			//Get this account
			Console.WriteLine($"{Environment.NewLine}Get updated account");
			AccountGrpcResponse getAccountResponse2 = await clientService.GetAccount(new GetAccountGrpcRequest {UserId = userId});
			LogData(getAccountResponse2);

			Console.WriteLine("End");
			Console.ReadLine();
		}

		private static void LogData(object data) => Console.WriteLine(JsonSerializer.Serialize(data));
	}
}