namespace BitButterCORE.V2
{
	internal class SimpleSerializePropertyInfo : SerializePropertyInfo
	{
		public SimpleSerializePropertyInfo(string name, object value)
			: base(name, value)
		{
		}

		public override string PopulateJsonString()
		{
			return $"{{\"Name\":\"{Name}\",\"Type\":\"{PropertyType}\",\"Value\":{Value}}}";
		}
	}
}
