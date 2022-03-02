using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Service.Core.Client.Services;

namespace Service.UserAccount.Services
{
	public class TokenCache : ITokenCache
	{
		private readonly ISystemClock _systemClock;

		private static readonly ConcurrentDictionary<string, DateTime> Dictionary;

		static TokenCache() => Dictionary = new ConcurrentDictionary<string, DateTime>();

		public TokenCache(ISystemClock systemClock) => _systemClock = systemClock;

		public bool Exists(string token)
		{
			Clean();

			return Dictionary.ContainsKey(token);
		}

		public void Add(string token, DateTime expire)
		{
			Clean();

			Dictionary.AddOrUpdate(token, t => expire, (t, time) => expire);
		}

		private void Clean()
		{
			KeyValuePair<string, DateTime>[] expiredPairs = Dictionary
				.Where(pair => pair.Value < _systemClock.Now)
				.ToArray();

			foreach (KeyValuePair<string, DateTime> pair in expiredPairs)
				Dictionary.TryRemove(pair);
		}
	}
}