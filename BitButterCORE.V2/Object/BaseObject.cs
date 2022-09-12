using System;

namespace BitButterCORE.V2
{
	public abstract class BaseObject
	{
		protected BaseObject(uint id)
		{
			ID = id;
			Reference = new ObjectReference(GetType(), ID);

			if (!ObjectFactory.Instance.IsObjectIDUsed(GetType(), ID))
			{
				throw new InvalidOperationException(string.Format("{0} should not be instantiated using its constructor, use ObjectFactory.Instance.Create<{1}>() instead.", GetType().FullName, GetType().Name));
			}
		}

		public uint ID { get; }

		public ObjectReference Reference { get; }
	}
}
