namespace BitButterCORE.V2
{
	internal abstract class SerializePropertyInfo
	{
		public SerializePropertyInfo(string name, int ctorParameterOrder, object value)
		{
			Name = name;
			PropertyType = value.GetType().Name;
			ConstructorParameterOrder = ctorParameterOrder;
			Value = value;
		}

		public string Name { get; }

		public string PropertyType { get; }

		public int ConstructorParameterOrder { get; }

		public object Value { get; }

		public abstract string PopulateJsonString();
	}
}
