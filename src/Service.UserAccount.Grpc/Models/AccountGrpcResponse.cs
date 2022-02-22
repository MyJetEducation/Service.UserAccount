using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class AccountGrpcResponse
	{
		[DataMember(Order = 1)]
		public AccountDataGrpcModel Data { get; set; }
	}
}