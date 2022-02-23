using System;
using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class ChangeEmailRequestGrpcRequest
	{
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public string Email { get; set; }
	}
}