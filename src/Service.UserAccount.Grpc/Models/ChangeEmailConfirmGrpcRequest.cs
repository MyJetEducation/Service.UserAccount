using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class ChangeEmailConfirmGrpcRequest
	{
		[DataMember(Order = 1)]
		public string Hash { get; set; }
	}
}