using System;

namespace BitButterCORE.V2
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExcludeSerializeObjectAttribute : Attribute
	{
	}
}
