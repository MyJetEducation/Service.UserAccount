using System.ServiceModel;
using System.Threading.Tasks;
using Service.Core.Client.Models;
using Service.UserAccount.Grpc.Models;

namespace Service.UserAccount.Grpc
{
	[ServiceContract]
	public interface IUserAccountService
	{
		[OperationContract]
		ValueTask<AccountGrpcResponse> GetAccount(GetAccountGrpcRequest request);

		[OperationContract]
		ValueTask<CommonGrpcResponse> SaveAccount(SaveAccountGrpcRequest request);
	}
}