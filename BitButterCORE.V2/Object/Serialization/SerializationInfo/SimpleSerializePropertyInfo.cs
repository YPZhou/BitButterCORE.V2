namespace BitButterCORE.V2
{
	internal class SimpleSerializePropertyInfo : SerializePropertyInfo
	{
		public SimpleSerializePropertyInfo(string name, int ctorParameterOrder, object value)
			: base(name, ctorParameterOrder, value)
		{
		}

		public override string PopulateJsonString()
		{
			var ctorParameterOrderString = ConstructorParameterOrder >= 0 ? $"\"ConstructorParameterOrder\":{ConstructorParameterOrder}," : string.Empty;
			return $"{{\"Name\":\"{Name}\",\"Type\":\"{PropertyType}\",{ctorParameterOrderString}\"Value\":{Value}}}";
		}
	}
}
