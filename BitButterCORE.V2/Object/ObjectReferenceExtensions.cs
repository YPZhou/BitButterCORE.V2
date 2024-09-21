namespace BitButterCORE.V2
{
	public static class ObjectReferenceExtensions
	{
		public static bool IsValid(this IObjectReference reference)
		{
			return reference != null && reference.IsValid;
		}
	}
}
