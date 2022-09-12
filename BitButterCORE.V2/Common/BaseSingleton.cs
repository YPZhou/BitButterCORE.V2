using System;

namespace BitButterCORE.V2
{
	public abstract class BaseSingleton<TSingleton>
	{
		protected BaseSingleton() { }

		public static TSingleton Instance => Nested.instance;

		class Nested
		{
			static Nested() { }
			internal static readonly TSingleton instance = (TSingleton)Activator.CreateInstance(typeof(TSingleton), true);
		}
	}
}
