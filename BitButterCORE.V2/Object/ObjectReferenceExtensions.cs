namespace BitButterCORE.V2.Object
{
	public static class ObjectReferenceExtensions
	{
		public static bool IsValid(this IObjectReference reference)
		{
			return reference != null && reference.IsValid;
		}
	}
}
