using System;

namespace BitButterCORE.V2
{
	public struct ObjectReference
	{
		public ObjectReference(Type type, uint id)
		{
			Type = type;
			ID = id;
		}

		public Type Type { get; }

		public uint ID { get; }

		public BaseObject Object => IsValid ? ObjectFactory.Instance.GetObjectByReference(this) : default(BaseObject);

		public T GetObject<T>() where T : BaseObject => IsValid && (Type == typeof(T) || typeof(T).IsAssignableFrom(Type)) ? Object as T : default; 

		public bool IsValid => Type != null && ID > 0 && ObjectFactory.Instance.HasObjectWithReference(this);

		public bool IsValidByTypeAndID<T>(uint id) where T : BaseObject
		{
			return (Type == typeof(T) || typeof(T).IsAssignableFrom(Type))
				&& ID == id
				&& IsValid;
		}

		public static bool operator == (ObjectReference ref1, ObjectReference ref2)
		{
			if ((object)ref1 == null)
			{
				return (object)ref2 == null;
			}

			return ref1.Equals(ref2);
		}

		public static bool operator != (ObjectReference ref1, ObjectReference ref2)
		{
			return !(ref1 == ref2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var ref2 = (ObjectReference)obj;
			return Type == ref2.Type && ID == ref2.ID;
		}

		public override int GetHashCode()
		{
			return Type.GetHashCode() ^ ID.GetHashCode();
		}
	}
}
