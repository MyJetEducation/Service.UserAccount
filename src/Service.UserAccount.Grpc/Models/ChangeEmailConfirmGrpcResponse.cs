using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class ChangeEmailConfirmGrpcResponse
	{
		[DataMember(Order = 1)]
		public bool HashExpired { get; set; }

		[DataMember(Order = 2)]
		public bool HashAlreadyUsed { get; set; }

		[DataMember(Order = 3)]
		public bool Changed { get; set; }
	}
}