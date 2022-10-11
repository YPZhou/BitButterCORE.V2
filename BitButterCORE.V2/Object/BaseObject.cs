using System;

namespace BitButterCORE.V2
{
	public abstract class BaseObject
	{
		public abstract uint ID { get; }
		public abstract IObjectReference Reference { get; }
	}

	public abstract class BaseObject<TObject> : BaseObject where TObject : BaseObject
	{
		protected BaseObject(uint id)
		{
			ID = id;
			TypedReference = CreateTypedReference();

			if (!ObjectFactory.Instance.IsObjectIDUsed(GetType(), ID))
			{
				throw new InvalidOperationException(string.Format("{0} should not be instantiated using its constructor, use ObjectFactory.Instance.Create<{1}>() instead.", GetType().FullName, GetType().Name));
			}
		}

		IObjectReference<TObject> CreateTypedReference()
		{
			var referenceType = typeof(ObjectReference<>).MakeGenericType(GetType());
			return (IObjectReference<TObject>)Activator.CreateInstance(referenceType, ID);
		}

		public override uint ID { get; }

		public IObjectReference<TObject> TypedReference { get; }

		public override IObjectReference Reference => TypedReference;

		public override string ToString()
		{
			return string.Format("{0}{1}", GetType().Name, ID);
		}
	}
}
