namespace BitButterCORE.V2
{
	internal abstract class SerializePropertyInfo
	{
		public SerializePropertyInfo(string name, object value)
		{
			Name = name;
			PropertyType = value.GetType().Name;
			Value = value;
		}

		public string Name { get; }

		public string PropertyType { get; }

		public object Value { get; }

		public abstract string PopulateJsonString();
	}
}
