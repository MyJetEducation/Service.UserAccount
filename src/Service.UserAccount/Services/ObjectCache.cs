using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Service.Core.Client.Services;

namespace Service.UserAccount.Services
{
	public interface IObjectCache<in T>
	{
		bool Exists(T data);

		void Add(T data, DateTime expire);
	}

	public class ObjectCache<T> : IObjectCache<T>
	{
		private readonly ISystemClock _systemClock;

		private static readonly ConcurrentDictionary<T, DateTime> Dictionary;

		static ObjectCache() => Dictionary = new ConcurrentDictionary<T, DateTime>();

		public ObjectCache(ISystemClock systemClock) => _systemClock = systemClock;

		public bool Exists(T data)
		{
			Clean();

			return Dictionary.ContainsKey(data);
		}

		public void Add(T data, DateTime expire)
		{
			Clean();

			Dictionary.AddOrUpdate(data, t => expire, (t, time) => expire);
		}

		private void Clean()
		{
			KeyValuePair<T, DateTime>[] expiredPairs = Dictionary
				.Where(pair => pair.Value < _systemClock.Now)
				.ToArray();

			foreach (KeyValuePair<T, DateTime> pair in expiredPairs)
				Dictionary.TryRemove(pair);
		}
	}
}