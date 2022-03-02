using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class ChangeEmailGrpcResponse
	{
		[DataMember(Order = 1)]
		public bool EmailAlreadyRegistered { get; set; }
		
		[DataMember(Order = 2)]
		public bool CantChangeToSameEmail { get; set; }
	}
}