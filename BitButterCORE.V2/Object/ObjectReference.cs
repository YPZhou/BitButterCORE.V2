using System;

namespace BitButterCORE.V2
{
	public interface IObjectReference : IEventHandler
	{
		Type Type { get; }

		uint ID { get; }

		bool IsValid { get; }

		IBaseObject Object { get; }
	}

	public interface IObjectReference<out TObject> : IObjectReference
	{
		new TObject Object { get; }
	}

	public struct ObjectReference<TObject> : IObjectReference<TObject> where TObject : IBaseObject
	{
		public ObjectReference(uint id)
		{
			Type = typeof(TObject);
			ID = id;
		}

		public Type Type { get; }

		public uint ID { get; }

		public TObject Object => IsValid ? (TObject)ObjectFactory.Instance.GetObjectByReference(this) : default(TObject);

		IBaseObject IObjectReference.Object => Object;

		public bool IsValid => Type != null && ID > 0 && ObjectFactory.Instance.HasObjectWithReference(this);

		public bool IsValidHandler => IsValid;

		public static bool operator == (ObjectReference<TObject> ref1, ObjectReference<TObject> ref2)
		{
			if ((object)ref1 == null)
			{
				return (object)ref2 == null;
			}

			return ref1.Equals(ref2);
		}

		public static bool operator != (ObjectReference<TObject> ref1, ObjectReference<TObject> ref2)
		{
			return !(ref1 == ref2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var ref2 = (ObjectReference<TObject>)obj;
			return Type == ref2.Type && ID == ref2.ID;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ ID.GetHashCode();
		}
	}
}
