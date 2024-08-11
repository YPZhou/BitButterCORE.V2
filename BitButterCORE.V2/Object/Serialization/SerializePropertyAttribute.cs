using System;

namespace BitButterCORE.V2
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class SerializePropertyAttribute : Attribute
	{
		public SerializePropertyAttribute(int ctorParameterOrder = -1)
		{
			ConstructorParameterOrder = ctorParameterOrder;
		}

		public int ConstructorParameterOrder { get; }
	}
}
