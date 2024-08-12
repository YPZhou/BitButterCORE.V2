using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal abstract class SerializePropertyInfo
	{
		public SerializePropertyInfo(string name, string propertyType, int ctorParameterOrder, object value)
		{
			Name = name;
			PropertyType = propertyType;
			ConstructorParameterOrder = ctorParameterOrder;
			Value = value;
		}

		public SerializePropertyInfo(JsonObject propertyObject)
		{
			Name = propertyObject["Name"].ToString();
			PropertyType = propertyObject["Type"].ToString();

			if (!propertyObject.TryGetPropertyValue("ConstructorParameterOrder", out var ctorParameterOrderValue))
			{
				ctorParameterOrderValue = -1;
			}

			ConstructorParameterOrder = (int)ctorParameterOrderValue;

			Value = PopulateValue(propertyObject["Value"]);
		}

		public string Name { get; }

		public string PropertyType { get; }

		public int ConstructorParameterOrder { get; }

		public object Value { get; private set; }

		public string PopulateJsonString()
		{
			var ctorParameterOrderString = ConstructorParameterOrder >= 0
				? $"\"ConstructorParameterOrder\":{ConstructorParameterOrder},"
				: string.Empty;
			return $"{{\"Name\":\"{Name}\",\"Type\":\"{PropertyType}\",{ctorParameterOrderString}\"Value\":{PopulateValueJsonString()}}}";
		}

		protected abstract string PopulateValueJsonString();

		protected abstract object PopulateValue(JsonNode valueNode);
	}
}
