using System;

namespace Service.UserAccount.Services
{
	public interface ITokenCache
	{
		bool Exists(string token);

		void Add(string token, DateTime expire);
	}
}