using System;

namespace BitButterCORE.V2
{
	public interface IBaseObject
	{
		uint ID { get; }
		IObjectReference Reference { get; }
		void OnObjectCreated();
	}

	public abstract class BaseObject<TObject> : IBaseObject where TObject : IBaseObject
	{
		protected BaseObject(uint id)
		{
			ID = id;
			Reference = CreateTypedReference();

			if (!ObjectFactory.Instance.IsObjectIDInUse(ID))
			{
				throw new InvalidOperationException(string.Format("{0} should not be instantiated using its constructor, use ObjectFactory.Instance.Create<{1}>() instead.", GetType().FullName, GetType().Name));
			}
		}

		IObjectReference<TObject> CreateTypedReference()
		{
			var referenceType = typeof(ObjectReference<>).MakeGenericType(GetType());
			return (IObjectReference<TObject>)Activator.CreateInstance(referenceType, ID);
		}

		void IBaseObject.OnObjectCreated()
		{
			OnObjectCreatedCore();
		}

		protected virtual void OnObjectCreatedCore()
		{
		}

		[SerializeProperty(ctorParameterOrder: 0)]
		public uint ID { get; private set; }

		public IObjectReference<TObject> Reference { get; }

		IObjectReference IBaseObject.Reference => Reference;

		public override string ToString()
		{
			return string.Format("{0}{1}", GetType().Name, ID);
		}
	}
}
