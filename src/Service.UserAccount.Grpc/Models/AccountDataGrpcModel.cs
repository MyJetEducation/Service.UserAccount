using System.Runtime.Serialization;

namespace Service.UserAccount.Grpc.Models
{
	[DataContract]
	public class AccountDataGrpcModel
	{
		[DataMember(Order = 1)]
		public string FirstName { get; set; }

		[DataMember(Order = 2)]
		public string LastName { get; set; }

		[DataMember(Order = 3)]
		public string Gender { get; set; }

		[DataMember(Order = 4)]
		public string Phone { get; set; }

		[DataMember(Order = 5)]
		public string Country { get; set; }
	}
}