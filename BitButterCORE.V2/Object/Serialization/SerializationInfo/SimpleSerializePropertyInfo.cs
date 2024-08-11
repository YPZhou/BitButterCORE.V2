using System.Text.Json.Nodes;

namespace BitButterCORE.V2
{
	internal class SimpleSerializePropertyInfo : SerializePropertyInfo
	{
		public SimpleSerializePropertyInfo(string name, int ctorParameterOrder, object value)
			: base(name, ctorParameterOrder, value)
		{
		}

		public SimpleSerializePropertyInfo(JsonObject propertyObject)
			: base(propertyObject)
		{
		}

		public override string PopulateJsonString()
		{
			var ctorParameterOrderString = ConstructorParameterOrder >= 0 ? $"\"ConstructorParameterOrder\":{ConstructorParameterOrder}," : string.Empty;
			return $"{{\"Name\":\"{Name}\",\"Type\":\"{PropertyType}\",{ctorParameterOrderString}\"Value\":{Value}}}";
		}

		protected override object PopulateValue(JsonNode valueNode)
		{
			var result = default(object);
			if (PropertyType == "UInt32")
			{
				result = uint.Parse(valueNode.ToString());
			}
			else if (PropertyType == "Int32")
			{
				result = int.Parse(valueNode.ToString());
			}
			else if (PropertyType == "Single")
			{
				result = float.Parse(valueNode.ToString());
			}
			else if (PropertyType == "String")
			{
				result = valueNode.ToString();
			}
			else if (PropertyType == "Boolean")
			{
				result = bool.Parse(valueNode.ToString());
			}

			return result;
		}
	}
}
