using System;
using System.Runtime.Serialization;

namespace Service.UserAccount.Domain.Models
{
	[DataContract]
	public class ChangeEmailGrpcModel
	{
		[DataMember(Order = 1)]
		public Guid? UserId { get; set; }

		[DataMember(Order = 2)]
		public string Email { get; set; }

		[DataMember(Order = 3)]
		public DateTime Expired { get; set; }
	}
}